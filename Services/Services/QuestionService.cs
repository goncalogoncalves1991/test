using DataAccess.Models.Create;
using DataAccess.Models.DTOs;
using DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Services.Services
{
    public class QuestionService
    {
        private QuestionRepository questionRepo;
        private static SessionService sessionService;
        private static QuestionService questionService;
        private static UserService userService;

        private QuestionService()
        {
            questionRepo = new QuestionRepository();
        }
        public class Messages
        {
            public static string PARAMETERS_NOT_NULL = "Some parameters must not be null";
            public static string QUESTION_SUCCESS = "The question was retrieved with success";
            public static string QUESTION_NOT_EXIST = "Does not exist any question with this id";
            public static string QUESTIONS_SUCCESS = "All questions retrieved with success";
            public static string QUESTION_CREATED = "The question was created with success";
            public static string USER_NO_PERMISSION = "User is not checked-in in this session";
            public static string QUESTION_UPDATED = "The question was updated with success";
            public static string USER_NO_QUESTION = "This user isn´t the author of this question";
            public static string QUESTION_DELETED = "The question was deleted with success";
            public static string LIKE_ERROR = "this user already has a like or is neither subscribed on this event and checked-in";
        }


        public static QuestionService GetInstance()
        {
            if (questionService == null) {
                questionService = new QuestionService();
                sessionService = SessionService.GetInstance();
                userService = UserService.GetInstance();
            }
            return questionService;
        }
        public async Task<OperationResult<question>> GetByIdAsync(int id)
        {
            question question = await questionRepo.GetByIdAsync(id);
            if (question != null)
            {
                return new OperationResult<question>() { Success = true, Message = Messages.QUESTION_SUCCESS, Result = question };
            }
            return new OperationResult<question>() { Success = false, Message = Messages.QUESTION_NOT_EXIST };
        }

        public async Task<OperationResult<IEnumerable<question>>> GetAllAsync()
        {
            IEnumerable<question> questions = await questionRepo.GetAllAsync();
            return new OperationResult<IEnumerable<question>>() { Success = true, Message = Messages.QUESTIONS_SUCCESS, Result = questions };
        }
        public async Task<OperationResult<IEnumerable<question>>> GetQuestionsFromSession(int sessioID)
        {
            var session = await sessionService.GetByIdAsync(sessioID);
            if (session.Success)
            {
                return new OperationResult<IEnumerable<question>>() { Success = true, Message = Messages.QUESTIONS_SUCCESS, Result = session.Result.question };
            }
            return new OperationResult<IEnumerable<question>>() { Success = false, Message = session.Message };
        }

        public async Task<OperationResult<int>> PostQuestionAsync(CreateQuestion item)
        {
            return await Post(item);
        }

        public async Task<OperationResult<int>> DeleteQuestion(CreateQuestion item)
        {
            return await Delete(item);
        }

        public async Task<OperationResult<int>> UpdateQuestion(CreateQuestion item)
        {
            return await Put(item);
        }

        public async Task<OperationResult<question>> LikeQuestion(CreateQuestion item)
        {
            return await PutLike(item);
        }


        //<------------------------------------------------------------------------------------------------------>
        //<------------------------------------------------------------------------------------------------------>


        private async Task<OperationResult<int>> Post(CreateQuestion item)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                if (!item.ParameterValid()) return new OperationResult<int>() { Success = false, Message = Messages.PARAMETERS_NOT_NULL };

                var session = await sessionService.GetByIdAsync(item.sessionId);
                if (!session.Success) return new OperationResult<int>() { Success = false, Message = session.Message };
                if (!session.Result.@event.eventSubscribers.Any(elem => (elem.userId == item.authorId) && elem.checkIn==true)) return new OperationResult<int>() { Success = false, Message = Messages.USER_NO_PERMISSION };

                try
                {
                    var id = await questionRepo.PostAsync(item);
                    scope.Complete();
                    return new OperationResult<int>() { Success = true, Message = Messages.QUESTION_CREATED , Result = id };
                }
                catch (Exception ex)
                {
                    return new OperationResult<int>() { Success = false, Message = ex.Message };
                }
            }
        }

        private async Task<OperationResult<int>> Put(CreateQuestion item)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                                
                var question = await questionRepo.GetByIdAsync(item.id);
                
                if (question == null) return new OperationResult<int>() { Success = false, Message = Messages.QUESTION_NOT_EXIST };
                if (question.authorId!=item.authorId) return new OperationResult<int>() { Success = false, Message = Messages.USER_NO_QUESTION };
               
                try
                {
                    question.message = item.message == null ? question.message : item.message;
                    var id = await questionRepo.PutAsync(question);
                    scope.Complete();
                    return new OperationResult<int>() { Success = true, Message = Messages.QUESTION_UPDATED, Result = id };
                }
                catch (Exception ex)
                {
                    return new OperationResult<int>() { Success = false, Message = ex.Message };
                }
            }
        }
        /**
        *   NOT SAFE!!!!!!!!!!!!!!!!
        */
        public async Task<OperationResult<int>> DislikeQuestion(string likedAuthor, int questionId)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var question = await questionRepo.GetByIdAsync(questionId);
                var user = await userService.GetByIdAsync(likedAuthor);               

                try
                {
                    await questionRepo.deleteLiker(user.Result, question);
                    scope.Complete();
                    return new OperationResult<int>() { Success = true, Message = Messages.QUESTION_UPDATED, Result = 0 };
                }
                catch (Exception ex)
                {
                    return new OperationResult<int>() { Success = false, Message = ex.Message };
                }
            }
        }

        private async Task<OperationResult<question>> PutLike(CreateQuestion item)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var question = await questionRepo.GetByIdAsync(item.id);
                var user = await userService.GetByIdAsync(item.liked_user);

                if (question == null) return new OperationResult<question>() { Success = false, Message = Messages.QUESTION_NOT_EXIST };
                if (!user.Success) return new OperationResult<question>() { Success = false, Message = user.Message };

                var userRes = user.Result;
                if (question.user_like.Any(elem => userRes.id==elem.id) || !question.session.@event.eventSubscribers.Any(elem => ((elem.userId == userRes.id) && (elem.checkIn.Value))))
                {
                    return new OperationResult<question>() { Success = false, Message = Messages.LIKE_ERROR };
                }

                try
                {
                    await questionRepo.insertLiker(userRes, question);
                    scope.Complete();
                    return new OperationResult<question>() { Success = true, Message = Messages.QUESTION_UPDATED, Result = question };
                }
                catch (Exception ex)
                {
                    return new OperationResult<question>() { Success = false, Message = ex.Message };
                }
            }
        }

        private async Task<OperationResult<int>> Delete(CreateQuestion item)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {                
                var question = await questionRepo.GetByIdAsync(item.id);
                if (question == null) return new OperationResult<int>() { Success = false, Message = Messages.QUESTION_NOT_EXIST };
                if (question.authorId != item.authorId) return new OperationResult<int>() { Success = false, Message = Messages.USER_NO_QUESTION };

                try
                {
                    var id = await questionRepo.DeleteAsync(question);
                    scope.Complete();
                    return new OperationResult<int>() { Success = true, Message = Messages.QUESTION_DELETED, Result = id };
                }
                catch (Exception ex)
                {
                    return new OperationResult<int>() { Success = false, Message = ex.Message };
                }
            }
        }
    }
}
