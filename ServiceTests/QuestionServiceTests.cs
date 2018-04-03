using DataAccess.Models.Create;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Services.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceTests
{
    [TestClass]
    public class QuestionServiceTests
    {
        private QuestionService questionService;

        [TestInitialize]
        public void initialize()
        {
            questionService = QuestionService.GetInstance();
        }

        [TestMethod]
        public void GetEventById()
        {
            var result = questionService.GetByIdAsync(1).Result;
            Assert.AreEqual(true, result.Success);
        }
        [TestMethod]
        public void GetAllQuestionsAsync()
        {
            var result = questionService.GetAllAsync().Result;
            Assert.IsTrue(result.Success);
            Assert.AreEqual(1, result.Result.Count());
        }
        [TestMethod]
        public void GetAllQuestionsFromSession()
        {
            var questions = questionService.GetQuestionsFromSession(4).Result;
            Assert.IsTrue(questions.Success);
            Assert.AreEqual(1, questions.Result.Count());
        }
        [TestMethod]
        public void GetAllQuestionsFromSession_NoQuestions()
        {
            var questions = questionService.GetQuestionsFromSession(1).Result;
            Assert.IsTrue(questions.Success);
            Assert.AreEqual(0, questions.Result.Count());
        }

        [TestMethod]
        public void GetAllSessionsFromEvent_NotSuccefull()
        {
            var sessions = questionService.GetQuestionsFromSession(100).Result;
            Assert.IsFalse(sessions.Success);

        }

        [TestMethod]
        public void Create_Question_Success()
        {

            var res = questionService.PostQuestionAsync(new CreateQuestion()
            {
                message="duvida grande???",
                sessionId=4,
                authorId="2"
            }).Result;

            var res2 = questionService.GetByIdAsync(res.Result).Result;
            Assert.AreEqual(res2.Result.message, "duvida grande???");
            var id = questionService.DeleteQuestion(new CreateQuestion()
            {
                id = res2.Result.id,
                authorId = "2"
            }).Result;
        }
        
        [TestMethod]
        public void Create_Question_User_Not_CheckIn()
        {
            var res = questionService.PostQuestionAsync(new CreateQuestion()
            {
                message = "duvida grande???",
                sessionId = 2,
                authorId = "1"
            }).Result;


            Assert.IsFalse(res.Success);
            Assert.AreEqual(QuestionService.Messages.USER_NO_PERMISSION, res.Message);
        }
       


        [TestMethod]
        public void Update_Question()
        {
            var res = questionService.UpdateQuestion(new CreateQuestion()
            {
                message = "duvida grande???",
                id =1,
                authorId = "2"
            }).Result;

            var res2 = questionService.GetByIdAsync(res.Result).Result;
            Assert.AreEqual(res2.Result.message, "duvida grande???");

            var res3 = questionService.UpdateQuestion(new CreateQuestion()
            {
                message = "What is the price of DocumentDb?",
                id = 1,
                authorId = "2"
            }).Result;
        }
        
        [TestMethod]
        public void Update_Question_No_Permission()
        {
            var res = questionService.UpdateQuestion(new CreateQuestion()
            {
                message = "duvida grande???",
                id = 1,
                authorId = "1"
            }).Result;

            Assert.IsFalse(res.Success);
            Assert.AreEqual(QuestionService.Messages.USER_NO_QUESTION, res.Message);
        }
        
        [TestMethod]
        public void Update_Question_already_liked_error()
        {
            var res = questionService.LikeQuestion(new CreateQuestion()
            {
                id = 1,
                liked_user="2",
            }).Result;

            Assert.IsFalse(res.Success);
            Assert.AreEqual(QuestionService.Messages.LIKE_ERROR, res.Message);
        }

        [TestMethod]
        public void Update_Question_like_but_not_checkin()
        {
            var res = questionService.LikeQuestion(new CreateQuestion()
            {
                id = 1,
                liked_user = "3",
            }).Result;

            Assert.IsFalse(res.Success);
            Assert.AreEqual(QuestionService.Messages.LIKE_ERROR, res.Message);
        }

        [TestMethod]
        public void Update_Question_like_but_not_subscribed()
        {
            var res = questionService.LikeQuestion(new CreateQuestion()
            {
                id = 1,
                liked_user = "1",
            }).Result;

            Assert.IsFalse(res.Success);
            Assert.AreEqual(QuestionService.Messages.LIKE_ERROR, res.Message);
        }

        [TestMethod]
        public void Update_Like()
        {
            var res = questionService.LikeQuestion(new CreateQuestion()
            {
                id = 1,
                liked_user = "4"
            }).Result;

            var res2 = questionService.GetByIdAsync(1).Result;
            Assert.AreEqual(res2.Result.user_like.Count, 2);

            var res3 = questionService.DislikeQuestion("4",1).Result;
        }
    }
}
