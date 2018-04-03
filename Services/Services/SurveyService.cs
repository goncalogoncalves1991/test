using DataAccess.Models.Create;
using DataAccess.Models.DTOs;
using DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static DataAccess.Models.Create.CreateSurvey;

namespace Services.Services
{
    public class SurveyService
    {
        private SurveyAnswerRepository surveyAnswerRepo;
        private SurveyRepository surveyRepo;
        private static SurveyService surveyService;
        private static EventService eventService;

        public class Messages
        {
            public static string PARAMETERS_NOT_NULL = "Some Parameters must not be null";
            public static string USER_NOT_ALLOWED = "This user is not an admin of this event";
            public static string QUESTION_NO_CONTENT = "Question must have a content with a message question.";
            public static string SURVEY_NO_QUESTION = "Survey must have at least one question";
            public static string CHOICE_NO_MESSAGE = "Choice must have a message";
            public static string TEMPLATE_INSERTED = "Survey Inserted with success";
            public static string NO_CHOICES = "Question has no Choices for this type";
            public static string USER_CANT_SEE_SURVEY = "User not allowed to see the surveyQuestion of this event";
            public static string RETURNED_SURVEY_SUCCESS = "Survey returned with success";
            public static string USER_CANT_ANSWER = "This user is not allowed to answer this Survey";
            public static string USER_ALREADY_SUBMITED = "This user already submited the surveyQuestion answer for this event";
            public static string ERROR_VALIDATE_ANSWER = "Error validating user surveyQuestion answer: ";
            public static string INVALID_NUMBER_QUESTIONS = "questions number does not match the surveyQuestion template";
            public static string QUESTION_ANSWER_INVALID_TYPE = "the answer to this question does not match the templated";
            public static string ANSWER_NOT_RESPECTING_TEMPLATE = "this answer does not respect the template";
        }

        private SurveyService()
        {
            surveyRepo = new SurveyRepository();
            surveyAnswerRepo = new SurveyAnswerRepository();
        }

        public static SurveyService GetInstance()
        {
            if (surveyService == null)
            {
                surveyService = new SurveyService();
                eventService = EventService.GetInstance();
            }
            return surveyService;
        }

        public async Task<OperationResult<IEnumerable<surveyQuestion>>> GetEventSurvey(int eventId, string userId)
        {            
            var eventRes = await eventService.GetByIdAsync(eventId);
            if (!eventRes.Success) return new OperationResult<IEnumerable<surveyQuestion>>() { Success = false, Message = eventRes.Message };
            if (!eventRes.Result.eventSubscribers.Any(elem => (elem.userId == userId) && elem.checkIn.Value))
            {
                if (!eventRes.Result.community.admins.Any(elem => (elem.id == userId)))
                    return new OperationResult<IEnumerable<surveyQuestion>>() { Success = false, Message = Messages.USER_CANT_SEE_SURVEY };
            };

            try
            {
                var surv = await surveyRepo.GetFromEventAsync(eventId);
                return new OperationResult<IEnumerable<surveyQuestion>>() { Success = true, Message = Messages.RETURNED_SURVEY_SUCCESS, Result = surv };
            }
            catch (Exception ex)
            {
                return new OperationResult<IEnumerable<surveyQuestion>>() { Success = false, Message = ex.InnerException.Message };
            }
            
        }

        public async Task<OperationResult<IEnumerable<surveyAnswer>>> GetSurveyResponses(int eventId, string userId)
        {
            var eventRes = await eventService.GetByIdAsync(eventId);
            if (!eventRes.Success) return new OperationResult<IEnumerable<surveyAnswer>>() { Success = false, Message = eventRes.Message };
            if (!eventRes.Result.community.admins.Any(elem => (elem.id == userId))) return new OperationResult<IEnumerable<surveyAnswer>>() { Success = false, Message = Messages.USER_CANT_SEE_SURVEY };

            try
            {
                var surv = await surveyAnswerRepo.GetFromEventAsync(eventId);
                return new OperationResult<IEnumerable<surveyAnswer>>() { Success = true, Message = Messages.RETURNED_SURVEY_SUCCESS, Result = surv };
            }
            catch (Exception ex)
            {
                return new OperationResult<IEnumerable<surveyAnswer>>() { Success = false, Message = ex.InnerException.Message };
            }            
        }

        public async Task<OperationResult<surveyQuestion>> PostSurvey(CreateSurvey sur)
        {
            if (!sur.ParameterValid()) return new OperationResult<surveyQuestion>() { Success = false, Message = Messages.PARAMETERS_NOT_NULL }; ;
            
            var eventRes = await eventService.GetByIdAsync(sur.eventId);
            if (!eventRes.Success) return new OperationResult<surveyQuestion>() { Success = false, Message = eventRes.Message };
            if (!eventRes.Result.community.admins.Any(elem => elem.id == sur.authorId)) return new OperationResult<surveyQuestion>() { Success = false, Message = Messages.USER_NOT_ALLOWED };

            var survey = await GetEventSurvey(sur.eventId, sur.authorId);
            if(survey.Success && survey.Result.Count()!=0) return new OperationResult<surveyQuestion>() { Success = false, Message = "This event already has posted a survey" };
            var template = checkTemplate(sur.questions);
            if (!template.Success) return new OperationResult<surveyQuestion>() { Success = false, Message = template.Message };

            try
            {
                var surv = await surveyRepo.PostAsync(sur);
                return new OperationResult<surveyQuestion>() { Success = true, Message = Messages.TEMPLATE_INSERTED, Result = null };
            }
            catch (Exception ex)
            {
                return new OperationResult<surveyQuestion>() { Success = false, Message = ex.InnerException.Message };
            }
        }

        public async Task<OperationResult<bool>> PostSurveyAnswer(CreateSurveyAnswer sur)
        {
            if (!sur.ParameterValid()) return new OperationResult<bool>() { Success = false, Message = Messages.PARAMETERS_NOT_NULL }; ;
            
            var eventRes = await eventService.GetByIdAsync(sur.eventId);
            if (!eventRes.Success) return new OperationResult<bool>() { Success = false, Message = eventRes.Message };
            if (!eventRes.Result.eventSubscribers.Any(elem => (elem.userId == sur.authorId) && elem.checkIn.Value)) return new OperationResult<bool>() { Success = false, Message = Messages.USER_CANT_ANSWER };

            try
            {
                var answerRes = await surveyAnswerRepo.GetByIdAsync(sur.eventId, sur.authorId);

                if (answerRes) return new OperationResult<bool>() { Success = false, Message = Messages.USER_ALREADY_SUBMITED };

                var templateRes = await GetEventSurvey(sur.eventId, sur.authorId);
                var answer = ValidateUserAnswer(templateRes.Result, sur);
                if (!answer.Success) return answer;
            
                var surv = await surveyAnswerRepo.PostAsync(sur);

                return new OperationResult<bool>() { Success = true, Message = Messages.TEMPLATE_INSERTED, Result = true };
            }
            catch (Exception ex)
            {
                return new OperationResult<bool>() { Success = false, Message = ex.InnerException.Message };
            }
            
        }

        private OperationResult<bool> checkTemplate(SurveyQuestion[] content)
        {
            foreach (SurveyQuestion sq in content)
            {
                if(!sq.ParameterValid()) return new OperationResult<bool>() { Success = false, Message = Messages.PARAMETERS_NOT_NULL }; ;

                switch (sq.type)
                {
                    case SurveyQuestion.multiple_choices:
                        if(sq.choices_messages==null || sq.choices_messages.Count()==0) return new OperationResult<bool>() { Success = false, Message = Messages.NO_CHOICES }; ;
                        break;
                    case SurveyQuestion.single_choices:
                        if (sq.choices_messages == null || sq.choices_messages.Count() == 0) return new OperationResult<bool>() { Success = false, Message = Messages.NO_CHOICES }; ;
                        break;
                    case SurveyQuestion.open_text:
                        sq.choices_messages = null;
                        break;
                    default:
                        return new OperationResult<bool>() { Success = false, Message = "type must be: 'open_text' || 'single_choice' || 'multiple_choice'" };                         
                }
            }
            return new OperationResult<bool>() { Success = true}; 
        }
        
        private OperationResult<bool> ValidateUserAnswer(IEnumerable<surveyQuestion> result, CreateSurveyAnswer content)
        {
            var answerQuestionCount = content.questions.Count();
            var questionTemplate = result.Count();
            if (questionTemplate != answerQuestionCount) return new OperationResult<bool>() { Success = false, Message = Messages.ERROR_VALIDATE_ANSWER + Messages.INVALID_NUMBER_QUESTIONS };
            var survQuestionArr = result.ToArray();
            for (int i = 0; i < answerQuestionCount; i++)
            {
                content.questions[i].questionId = (i+1);

                switch (survQuestionArr[i].type)
                {
                    case SurveyQuestion.multiple_choices:
                        if (content.questions[i].choices==null || content.questions[i].choices.Count()==0) return new OperationResult<bool>() { Success = false, Message = Messages.NO_CHOICES }; ;
                        content.questions[i].choices = content.questions[i].choices.Distinct().ToArray();
                        if (!content.questions[i].choices.All(elem => survQuestionArr[i].surveyChoice.Any(e => elem==e.choiceId))) return new OperationResult<bool>() { Success = false, Message = "choice doesn't match the question group of possible choices" }; ;
                        content.questions[i].text = null;
                        break;
                    case SurveyQuestion.single_choices:
                        if (content.questions[i].choices == null || content.questions[i].choices.Count() != 1) return new OperationResult<bool>() { Success = false, Message = Messages.NO_CHOICES };
                        if (!content.questions[i].choices.All(elem => survQuestionArr[i].surveyChoice.Any(e => elem == e.choiceId))) return new OperationResult<bool>() { Success = false, Message = "choice doesn't match the question group of possible choices" }; ;
                        content.questions[i].text = null;
                        break;
                    case SurveyQuestion.open_text:
                        if (content.questions[i].text == null) return new OperationResult<bool>() { Success = false, Message = "the question (nr."+ (i+1) +") must have a text answer" };
                        content.questions[i].choices = null;
                        break;
                }
            }
            return new OperationResult<bool>() { Success = true };
        }
        
        /**
         *   !!!!!!!!!!!!!!!!!!!!!!!
         *   Não usável para UI. Não está a fazer verificações. 
         *   Só para Testes.  
         */
        public void DeleteSurvey(IEnumerable<surveyQuestion> sur)
        {            
            try{
                var surv = surveyRepo.DeleteAsync(sur).Result;                  
            }
            catch (Exception ex){}            
        }


        public void DeleteSurveyAnswer(IEnumerable<surveyAnswer> sur)
        {
            try{
                var surv = surveyAnswerRepo.DeleteAsync(sur).Result;
            }
            catch (Exception ex){}
        }


        /**
        *   {      
        *       questions:  [
        *                       {
        *                           type:"open_text" || "multiple_choice" || "single_choice",
        *                           id: int,
        *                           content:    {
        *                                           question:"string",
        *                                           choices_messages: [{id:int, message:"string"}] //mandatory on choice type
        *                                       }
        *                       },...
        *                   ]
        *   }    
        */
    }
}
