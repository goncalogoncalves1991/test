using DataAccess.Models.Create;
using DataAccess.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessTest
{
    [TestClass]
    public class QuestionTests
    {
        private QuestionRepository questionRepo;

        [TestInitialize]
        public void initialize()
        {
            questionRepo = new QuestionRepository();
        }

        [TestMethod]
        public void GetQuestions()
        {
            var res = questionRepo.GetAllAsync().Result;

            int count = res.Count();
            Assert.AreEqual(1, count);
        }

        [TestMethod]
        public void GetQuestion()
        {
            var res = questionRepo.GetByIdAsync(1).Result;

            Assert.AreEqual("What is the price of DocumentDb?", res.message);
        }

        [TestMethod]
        public void CreateQuestion()
        {
            int newQuestionID = 0;

            try
            {
                newQuestionID = questionRepo.PostAsync(new CreateQuestion() { authorId="1", sessionId=1, message="entao?" }).Result;
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex.InnerException, typeof(ArgumentException));
            }
            var res = questionRepo.GetByIdAsync(newQuestionID).Result;
            Assert.AreEqual(res.message, "entao?");
            var id = questionRepo.DeleteAsync(res).Result;

        }

        /*
        [TestMethod]
        public void UpdateQuestion()
        {
            var question = questionRepo.GetByIdAsync(1).Result;
            question.likes++;
            var id = questionRepo.PutAsync(question).Result;
            var question2 = questionRepo.GetByIdAsync(1).Result;
            Assert.AreEqual(question2.likes, 1);

            question.likes--;
            var final = questionRepo.PutAsync(question).Result;

        }*/
    }
}
