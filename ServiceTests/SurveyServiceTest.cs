using Microsoft.VisualStudio.TestTools.UnitTesting;
using Services.Services;
using DataAccess.Models.Create;
using static DataAccess.Models.Create.CreateSurvey;
using System.Linq;
using static DataAccess.Models.Create.CreateSurveyAnswer;

namespace ServiceTests
{
    [TestClass]
    public class SurveyServiceTest
    {
        private SurveyService surveyService;

        [TestInitialize]
        public void initialize()
        {
            surveyService = SurveyService.GetInstance();
        }

        [TestMethod]
        public void Get_Survey()
        {
            var result = surveyService.GetEventSurvey(1, "2").Result;
            var res = result.Result;
            Assert.IsTrue(result.Success);
            Assert.AreEqual(3, res.Count());
            Assert.AreEqual(SurveyQuestion.open_text, res.Where(e=> (e.eventId==1) && (e.questionId==1)).FirstOrDefault().type);
        }
        
        [TestMethod]
        public void Get_Survey_Answer()
        {
            var result = surveyService.GetSurveyResponses(1, "2").Result;
            Assert.IsTrue(result.Success);
            var res = result.Result.Where(e => (e.eventId == 1) && (e.questionId == 1) && (e.authorId=="2")).FirstOrDefault();
            Assert.IsNotNull(res.surveyTextAnswer);
            Assert.AreEqual(res.surveyTextAnswer.response, "Tá fixe.");
            Assert.AreEqual(res.survey.type, "open_text");
        }

        [TestMethod]
        public void Post_Survey_Success()
        {
            var q = new SurveyQuestion[] {
                new SurveyQuestion { type="open_text",question="abriga"},
                new SurveyQuestion { type="single_choice",question="olaola",choices_messages=new string[] {"ola1","ola2","ola3" } }
            };
            var param = new CreateSurvey() { authorId = "2", eventId = 2, questions = q };

            var result = surveyService.PostSurvey(param).Result;

            Assert.IsTrue(result.Success);

            var result2 = surveyService.GetEventSurvey(2, "2").Result;
            Assert.AreEqual(SurveyQuestion.single_choices, result2.Result.Where(e => (e.eventId == 2) && (e.questionId == 2)).FirstOrDefault().type);

            surveyService.DeleteSurvey(result2.Result);
        }

        [TestMethod]
        public void Post_Survey_No_Question()
        {
            var cont = new SurveyQuestion[]{};

            var param = new CreateSurvey() { eventId = 2, authorId = "2", questions = cont };
            var result = surveyService.PostSurvey(param).Result;
            Assert.IsFalse(result.Success);
            Assert.AreEqual(SurveyService.Messages.PARAMETERS_NOT_NULL, result.Message);
        }
        
        [TestMethod]
        public void Post_Survey_No_Content_Question()
        {
            var cont = new SurveyQuestion[]
            {
                new SurveyQuestion() { type = SurveyQuestion.open_text } 
            };
            var param = new CreateSurvey() { eventId = 2, authorId = "2", questions = cont };
            var result = surveyService.PostSurvey(param).Result;
            Assert.IsFalse(result.Success);
            Assert.AreEqual(SurveyService.Messages.PARAMETERS_NOT_NULL, result.Message);
        }
        
        [TestMethod]
        public void Post_Survey_No_Choice_Question()
        {
            var cont = new SurveyQuestion[]
            {
                new SurveyQuestion() { type = SurveyQuestion.single_choices, question = "tas bom?"  }
            };
            var param = new CreateSurvey() { eventId = 2, authorId = "2", questions = cont };
            var result = surveyService.PostSurvey(param).Result;
            Assert.IsFalse(result.Success);
            Assert.AreEqual(SurveyService.Messages.NO_CHOICES, result.Message);
        }
        
        [TestMethod]
        public void Post_Survey_No_Choice_Message()
        {
            var cont = new SurveyQuestion[]
            {
                new SurveyQuestion() { type = SurveyQuestion.single_choices, question = "tas bom?", choices_messages=new string[] { }  }
            };
            var param = new CreateSurvey() { eventId = 2, authorId = "2", questions = cont };
            var result = surveyService.PostSurvey(param).Result;
            Assert.IsFalse(result.Success);
            Assert.AreEqual(SurveyService.Messages.NO_CHOICES, result.Message);
        }
                        

        [TestMethod]
        public void Post_Survey_Answer_Success()
        {
            var cont = new SurveyQuestionAnswer[]
            {
                new SurveyQuestionAnswer{text="ta mt fixe",choices=new int[] { 2 }},
                new SurveyQuestionAnswer{ choices=new int[] { 2 } },
                new SurveyQuestionAnswer{ choices=new int[] { 1,2,3,3,3 } }
            };
            var param = new CreateSurveyAnswer() { eventId = 1, authorId = "4", questions = cont };
            var result = surveyService.PostSurveyAnswer(param).Result;
            
            Assert.IsTrue(result.Success);

            var result2 = surveyService.GetSurveyResponses(1, "2").Result.Result.Where(elem => elem.authorId=="4").ToList();
            Assert.AreEqual(result2.Count(), 3);

            var questionAnswer = result2.Where(e => e.questionId == 1).FirstOrDefault();
            Assert.IsNotNull(questionAnswer.surveyTextAnswer);
            Assert.AreEqual(questionAnswer.surveyTextAnswer.response, "ta mt fixe");

            surveyService.DeleteSurveyAnswer(result2);
        }
        
        [TestMethod]
        public void Post_Answer_User_Already_Posted()
        {
            var cont = new SurveyQuestionAnswer[]{
                                                    new SurveyQuestionAnswer{text="ta mt fixe"},
                                                    new SurveyQuestionAnswer { choices=new int[] { 2 } },
                                                    new SurveyQuestionAnswer{ choices=new int[] { 1,2,3,3,3 } } };
            var param = new CreateSurveyAnswer() { eventId = 1, authorId = "2", questions = cont };
            var result = surveyService.PostSurveyAnswer(param).Result;
            Assert.IsFalse(result.Success);
            Assert.AreEqual(SurveyService.Messages.USER_ALREADY_SUBMITED, result.Message);
        }

        [TestMethod]
        public void Post_Answer_Number_Not_The_Same()
        {
            var cont = new SurveyQuestionAnswer[]{                                                    
                                                    new SurveyQuestionAnswer { choices=new int[] { 2 } }};
            var param = new CreateSurveyAnswer() { eventId = 1, authorId = "4", questions = cont };
            var result = surveyService.PostSurveyAnswer(param).Result;
            Assert.IsFalse(result.Success);
            Assert.AreEqual(SurveyService.Messages.ERROR_VALIDATE_ANSWER + SurveyService.Messages.INVALID_NUMBER_QUESTIONS, result.Message);
        }

        [TestMethod]
        public void Post_Answer_Does_Not_Has_Text_Response()
        {
            var cont = new SurveyQuestionAnswer[]{
                                                    new SurveyQuestionAnswer {choices=new int[] { 2 }},
                                                    new SurveyQuestionAnswer {choices=new int[] { 2 } },
                                                    new SurveyQuestionAnswer{ choices=new int[] { 1,2,3,3,3 }} };
            var param = new CreateSurveyAnswer() { eventId = 1, authorId = "4", questions = cont };
            var result = surveyService.PostSurveyAnswer(param).Result;
            Assert.IsFalse(result.Success);
            Assert.AreEqual("the question (nr.1) must have a text answer", result.Message);
        }

        [TestMethod]
        public void Post_Answer_Bad_Single_Choice()
        {
            var cont = new SurveyQuestionAnswer[]{
                                                    new SurveyQuestionAnswer {text="ya"},
                                                    new SurveyQuestionAnswer {choices=new int[] { 2,3 } },
                                                    new SurveyQuestionAnswer{ choices=new int[] { 1,2,3,3,3 }} };
            var param = new CreateSurveyAnswer() { eventId = 1, authorId = "4", questions = cont };
            var result = surveyService.PostSurveyAnswer(param).Result;
            Assert.IsFalse(result.Success);
            Assert.AreEqual(SurveyService.Messages.NO_CHOICES, result.Message);
        }

        [TestMethod]
        public void Post_Answer_Bad_Single_Choice_v2()
        {
            var cont = new SurveyQuestionAnswer[]{
                                                    new SurveyQuestionAnswer {text="ya"},
                                                    new SurveyQuestionAnswer {choices=new int[] { 25 } },
                                                    new SurveyQuestionAnswer{ choices=new int[] { 1,2,3,3,3 }} };
            var param = new CreateSurveyAnswer() { eventId = 1, authorId = "4", questions = cont };
            var result = surveyService.PostSurveyAnswer(param).Result;
            Assert.IsFalse(result.Success);
            Assert.AreEqual("choice doesn't match the question group of possible choices", result.Message);
        }

        [TestMethod]
        public void Post_Answer_Bad_Multiple_Choice()
        {
            var cont = new SurveyQuestionAnswer[]{
                                                    new SurveyQuestionAnswer {text="ya"},
                                                    new SurveyQuestionAnswer {choices=new int[] { 2 } },
                                                    new SurveyQuestionAnswer{ choices=new int[] {  }} };
            var param = new CreateSurveyAnswer() { eventId = 1, authorId = "4", questions = cont };
            var result = surveyService.PostSurveyAnswer(param).Result;
            Assert.IsFalse(result.Success);
            Assert.AreEqual(SurveyService.Messages.NO_CHOICES, result.Message);
        }

        [TestMethod]
        public void Post_Answer_Bad_Multiple_Choice_v2()
        {
            var cont = new SurveyQuestionAnswer[]{
                                                    new SurveyQuestionAnswer {text="ya"},
                                                    new SurveyQuestionAnswer {choices=new int[] { 2 } },
                                                    new SurveyQuestionAnswer{ choices=new int[] { 1,45 }} };
            var param = new CreateSurveyAnswer() { eventId = 1, authorId = "4", questions = cont };
            var result = surveyService.PostSurveyAnswer(param).Result;
            Assert.IsFalse(result.Success);
            Assert.AreEqual("choice doesn't match the question group of possible choices", result.Message);
        }

    }
}
