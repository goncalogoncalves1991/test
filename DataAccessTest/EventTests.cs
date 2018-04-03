using DataAccess.Models.Create;
using DataAccess.Models.DTOs;
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
    public class EventTests
    {
        EventRepository eventRepo;

        [TestInitialize]
        public void initialize()
        {
            eventRepo = new EventRepository();
        }

        [TestMethod]
        public void GetEventById()
        {
            var res = eventRepo.GetByIdAsync(1).Result;

            Assert.AreEqual("azure", res.title);
            Assert.AreEqual(2, res.communityId);
        }

        [TestMethod]
        public void GetEventById_Null()
        {
            Assert.AreEqual(null, eventRepo.GetByIdAsync(100).Result);
        }
        [TestMethod]
        public void GetAllEvents()
        {
            IEnumerable<@event> events = eventRepo.GetAllAsync().Result;
            Assert.AreEqual(5 , events.Count());
        }
        
       
        
        
        [TestMethod]
        public void GetEventsByLocal()
        {
            var res = eventRepo.GetByLocationAsync("Lisboa").Result;
            Assert.AreEqual(4, res.Count());
        }

        [TestMethod]
        public void CreateEvent()
        {
            int newEventID = 0;

            try
            {
                newEventID = eventRepo.PostAsync(new CreateEvent() { communityId = 3, title = "java Beans", local = "albufeira", description = "grande java", nrOfTickets = 20, initDate = DateTime.Now, endDate = DateTime.Parse("2015-07-20T00:00:00.0000000"),Tags=new int[]{6} }).Result;
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex.InnerException, typeof(ArgumentException));
            }
            var res = eventRepo.GetByIdAsync(newEventID).Result;
            Assert.AreEqual(res.title, "java Beans");
            Assert.AreEqual(1, res.tag.Count());
            var id = eventRepo.DeleteAsync(res).Result;

        }
        
        
        [TestMethod]
        public void UpdateEvent()
        {
            var community = eventRepo.GetByIdAsync(2).Result;
            community.title = "storage";
            var id = eventRepo.PutAsync(community).Result;
            var community2 = eventRepo.GetByIdAsync(2).Result;
            Assert.AreEqual(community.title, "storage");
            Assert.AreEqual(community.description, "grande descriçao de azure storage...");

            community.title = "azure storage";
            var final = eventRepo.PutAsync(community).Result;

        }
       
        [TestMethod]
        public void InsertEventSubscriber()
        {
            string id = eventRepo.InsertSubscriber(5, "2").Result;
            var eventRes = eventRepo.GetByIdAsync(5).Result;
            Assert.AreEqual(2, eventRes.eventSubscribers.Count);
            Assert.AreEqual(14, eventRes.nrOfTickets);//check if nrticket trigger is working

            string id2 = eventRepo.DeleteMember(5, "2").Result;
            eventRes.nrOfTickets = 15;
            var final = eventRepo.PutAsync(eventRes).Result;

        }
        
        [TestMethod]
        public void InsertEventTag()
        {
            bool success = eventRepo.InsertTag(5, new int[] { 3, 4 }).Result;
            var eventRes = eventRepo.GetByIdAsync(5).Result;
            Assert.AreEqual(4, eventRes.tag.Count);
            Assert.IsNotNull(eventRes.tag.First(t => t.name == "visual studio"));
            Assert.IsNotNull(eventRes.tag.First(t => t.name == ".net"));

            bool id2 = eventRepo.DeleteTag(5, new int[] { 3, 4 }).Result;
        }

    }
}
