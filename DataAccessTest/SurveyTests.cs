using DataAccess.Models.Create;
using DataAccess.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DataAccess.Models.Create.CreateSurvey;
using static DataAccess.Models.Create.CreateSurvey.SurveyQuestion;

namespace DataAccessTest
{
    [TestClass]
    public class SurveyTests
    {
        private SurveyRepository surveyRepo;

        [TestInitialize]
        public void initialize()
        {
            surveyRepo = new SurveyRepository();
        }

        [TestMethod]
        public void GetSurveyFromEvent()
        {
            var res = surveyRepo.GetFromEventAsync(1).Result;
            Assert.AreEqual(2,res.Count());
            Assert.AreEqual("Rate do evento", res.ToArray()[1].question);
            Assert.AreEqual(2, res.ToArray()[1].surveyChoice.Count());
        }
            /*
                    [TestMethod]
                    public void CreateSurvey()
                    {
                        int newSessionID = 0;

                        try
                        {
                            var q = new SurveyQuestion[]
                            {
                                new SurveyQuestion()
                                {
                                    type="open_text",
                                    question="olaola"
                                },
                                new SurveyQuestion()
                                {
                                    type="multiple_choice",
                                    question="rate",
                                    choices = new Choice[]
                                    {
                                        new Choice() {message="1" },
                                        new Choice() {message="2" }
                                    }
                                }
                            };
                            var a = new CreateSurvey() { eventId = 2, questions= q };
                            newSessionID = surveyRepo.PostAsync(a).Result;
                        }
                        catch (Exception ex)
                        {
                            Assert.IsInstanceOfType(ex.InnerException, typeof(ArgumentException));
                        }
                        var res = surveyRepo.GetByIdAsync(newSessionID).Result;
                        Assert.AreEqual(res.title, "sessao fixe");
                        var id = surveyRepo.DeleteAsync(res).Result;

                    }*/
        }
}
