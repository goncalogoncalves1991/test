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
    public class SessionTests
    {
        private SessionRepository sessionRepo;

        [TestInitialize]
        public void initialize()
        {
            sessionRepo = new SessionRepository();
        }

        [TestMethod]
        public void GetSessions()
        {
            var res = sessionRepo.GetAllAsync().Result;

            int count = res.Count();
            Assert.AreEqual(4, count);
        }

        [TestMethod]
        public void GetSession()
        {
            var res = sessionRepo.GetByIdAsync(1).Result;

            Assert.AreEqual("Get started RavenDb", res.title);
        }        

        [TestMethod]
        public void CreateSession()
        {
            int newSessionID = 0;

            try
            {
                newSessionID = sessionRepo.PostAsync(new CreateSession() {eventId=1, title="sessao fixe", description="sessao muita fixe", initialDate=DateTime.Now, endDate=DateTime.Now.AddHours(5) , speakerName="Raposo",lastName="lugar",linkOfSpeaker="www.google.com" }).Result;
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex.InnerException, typeof(ArgumentException));
            }
            var res = sessionRepo.GetByIdAsync(newSessionID).Result;
            Assert.AreEqual(res.title, "sessao fixe");
            var id = sessionRepo.DeleteAsync(res).Result;

        }

        
        [TestMethod]
        public void UpdateSession()
        {
            var session = sessionRepo.GetByIdAsync(1).Result;
            session.speakerName = "Artur";
            var id = sessionRepo.PutAsync(session).Result;
            var session2 = sessionRepo.GetByIdAsync(1).Result;
            Assert.AreEqual(session2.speakerName, "Artur");

            session.speakerName = "geraldo";
            var final = sessionRepo.PutAsync(session).Result;

        }
    }
}
