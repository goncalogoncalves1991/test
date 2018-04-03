using DataAccess.Models.Create;
using DataAccess.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DataAccess.Models.Create.CreateSurveyAnswer;

namespace DataAccessTest
{
    [TestClass]
    public class SurveyAnswerTests
    {
        private SurveyAnswerRepository surveyAnswerRepo;

        [TestInitialize]
        public void initialize()
        {
            surveyAnswerRepo = new SurveyAnswerRepository();
        }

        [TestMethod]
        public void GetSurveyFromEvent()
        {
            var res = surveyAnswerRepo.GetFromEventAsync(1).Result;
            Assert.AreEqual(2, res.Count());
            Assert.AreEqual(1, res.Where(elem=> elem.questionId==2).FirstOrDefault().surveyChoiceAnswer.Count());
            Assert.AreEqual("Tá fixe.", res.Where(elem => elem.questionId == 1).FirstOrDefault().surveyTextAnswer.response);
        }
        /*
        [TestMethod]
        public void CreateSession()
        {
            int newSessionID = 0;

            try
            {
                var quest = new SurveyQuestionAnswer[] { new SurveyQuestionAnswer() { questionId=1,text="blavla" }, new SurveyQuestionAnswer() { questionId=1, text="eeee"} };
                var r = new CreateSurveyAnswer() { authorId="4",eventId=1,questions=quest};
                newSessionID = surveyAnswerRepo.PostAsync(r).Result;
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex.InnerException, typeof(ArgumentException));
            }
            var res = sessionRepo.GetByIdAsync(newSessionID).Result;
            Assert.AreEqual(res.title, "sessao fixe");
            var id = sessionRepo.DeleteAsync(res).Result;

        }*/
    }
}
