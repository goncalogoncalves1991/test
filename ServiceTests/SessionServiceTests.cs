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
    public class SessionServiceTests
    {
        private SessionService sessionService;

        [TestInitialize]
        public void initialize()
        {
            sessionService = SessionService.GetInstance();
        }

        [TestMethod]
        public void GetSessionById()
        {
            var result = sessionService.GetByIdAsync(1).Result;
            Assert.AreEqual(true, result.Success);
        }
        [TestMethod]
        public void GetAllSessionsAsync()
        {
            var result = sessionService.GetAllAsync().Result;
            Assert.IsTrue(result.Success);
            Assert.AreEqual(4, result.Result.Count());
        }
        [TestMethod]
        public void GetAllSessionFromEvent()
        {
            var sessions = sessionService.GetSessionsFromEvent(1).Result;
            Assert.IsTrue(sessions.Success);
            Assert.AreEqual(1, sessions.Result.Count());
        }
        [TestMethod]
        public void GetAllSessionsFromEvent_NotSuccefull()
        {
            var sessions = sessionService.GetSessionsFromEvent(100).Result;
            Assert.IsFalse(sessions.Success);

        }

        [TestMethod]
        public void Create_Session_Success()
        {

            var res = sessionService.PostSessionAsync(new CreateSession()
            {
                eventId = 2,
                userId = "2",
                initialDate = DateTime.Now,
                title = "super sessao",
                description = "grande descrição.................",
                speakerName="Gonçalo",
                lastName="Gonçalves",
                linkOfSpeaker="linkedin.com"
            }).Result;

            var res2 = sessionService.GetByIdAsync(res.Result).Result;
            Assert.AreEqual(res2.Result.title, "super sessao");
            Assert.AreEqual(res2.Result.speakerName, "Gonçalo");
            var id = sessionService.DeleteSession(new CreateSession()
            {
                id = res2.Result.id,
                userId = "2",
            }).Result;
        }
        
        [TestMethod]
        public void Create_Session_Event_Not_Exist()
        {
            var res = sessionService.PostSessionAsync(new CreateSession()
            {
                eventId = 50,
                userId = "2",
                initialDate = DateTime.Now,
                title = "super sessao",
                description = "grande descrição.................",
                speakerName = "Gonçalo",
                lastName = "Gonçalves",
                linkOfSpeaker = "linkedin.com"
            }).Result;

            Assert.IsFalse(res.Success);
            Assert.AreEqual(EventService.Messages.EVENT_NOT_EXIST, res.Message);
        }
        
        [TestMethod]
        public void Create_Session_User_No_Permission()
        {
            var res = sessionService.PostSessionAsync(new CreateSession()
            {
                eventId = 2,
                userId = "1",
                initialDate = DateTime.Now,
                title = "super sessao",
                description = "grande descrição.................",
                speakerName = "Gonçalo",
                lastName = "Gonçalves",
                linkOfSpeaker = "linkedin.com"
            }).Result;

            Assert.IsFalse(res.Success);
            Assert.AreEqual(SessionService.Messages.USER_NO_PERMISSION, res.Message);
        }

        
        [TestMethod]
        public void Update_Session()
        {
            var res = sessionService.UpdateSession(new CreateSession()
            {
                id = 1,
                userId = "2",
                title = "super sessao"
            }).Result;

            var res2 = sessionService.GetByIdAsync(res.Result).Result;
            Assert.AreEqual(res2.Result.title, "super sessao");

            var res3 = sessionService.UpdateSession(new CreateSession()
            {
                id = 1,
                userId = "2",
                title = "Get started RavenDb"
            }).Result;
        }
        
        [TestMethod]
        public void Update_Session_No_Permission()
        {
            var res = sessionService.UpdateSession(new CreateSession()
            {
                id = 1,
                userId = "1",
                title = "super sessao"
            }).Result;

            Assert.IsFalse(res.Success);
            Assert.AreEqual(SessionService.Messages.USER_NO_PERMISSION, res.Message);
        }
    }
}
