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
    public class EventServiceTests
    {
        private EventService eventService;

        [TestInitialize]
        public void initialize()
        {

            eventService = EventService.GetInstance();
        }

        [TestMethod]
        public void GetEventById()
        {
            var result = eventService.GetByIdAsync(1).Result;
            Assert.AreEqual(true, result.Success);
        }
        [TestMethod]
        public void GetAllEventsAsync()
        {
            var result = eventService.GetAllAsync().Result;
            Assert.AreEqual(5, result.Result.Count());
        }
        
        [TestMethod]
        public void GetAllEventsOfUser()
        {
            var result = eventService.GetAllEventsFromUserAsync("1",EventService.Time.All).Result;
            Assert.AreEqual(2, result.Result.Count());
        }
        [TestMethod]
        public void GetPastEventByCommunity()
        {
            var res = eventService.GetAllEventsFromCommunityInTime(3,EventService.Time.Past).Result;
            Assert.IsTrue(res.Success);
            Assert.AreEqual(1, res.Result.Count());

        }

        [TestMethod]
        public void GetFutureEventByCommunity()
        {
            var res = eventService.GetAllEventsFromCommunityInTime(3, EventService.Time.Future).Result;
            Assert.IsTrue(res.Success);
            Assert.AreEqual(1, res.Result.Count());
        }

        [TestMethod]
        public void GetEventsByCommunityId()
        {
            var events = eventService.GetEventsByCommunityId(3).Result;
            Assert.IsTrue(events.Success);
            Assert.AreEqual(2, events.Result.Count());
        }
        [TestMethod]
        public void GetEventsByCommunityId_NotSuccefull()
        {
            var events = eventService.GetEventsByCommunityId(100).Result;
            Assert.IsFalse(events.Success);
        }

        [TestMethod]
        public void Get_Events_To_Come_From_Communities_Which_User_Belongs_Success()
        {
            var events = eventService.Get_Events_To_Come_From_Communities_Which_User_Belongs("3").Result;
            Assert.AreEqual(2, events.Result.Count());
        }

        [TestMethod]
        public void Create_Event_Success()
        {
            var res = eventService.PostEventAsync(new CreateEvent()
            {
               communityId=1,
               description="grande evento",
               title="super evento",
               local="Lisboa",
               nrOfTickets=20,
               UserId = "1",
               initDate = Convert.ToDateTime("1/06/2016"),
               endDate=Convert.ToDateTime("10/06/2016"),
               Tags = new int[] {1,1,1,8,2 }
            }).Result;

            //var res2 = eventService.GetByIdAsync(res.Result).Result;
            Assert.AreEqual(res.Result.title, "super evento");
            Assert.AreEqual(3, res.Result.tag.Count());
            var id = eventService.DeleteEvent(new CreateEvent()
            {
                UserId = "1",
                Id = res.Result.id
            }).Result;
        }        

        [TestMethod]
        public void Create_Event_User_No_Permission()
        {
            var res = eventService.PostEventAsync(new CreateEvent()
            {
                communityId = 3,
                title = "super evento",
                nrOfTickets = 20,
                UserId = "2",
                initDate = Convert.ToDateTime("1/06/2016"),
                endDate = Convert.ToDateTime("10/06/2016")
            }).Result;

            Assert.IsFalse(res.Success);
            Assert.AreEqual(EventService.Messages.USER_NO_PERMISSION, res.Message);
        }

        [TestMethod]
        public void Create_Event_Tag_Not_Exists()
        {
            var res = eventService.PostEventAsync(new CreateEvent()
            {
                communityId = 1,
                title = "super evento",
                nrOfTickets = 20,
                UserId = "1",
                initDate = Convert.ToDateTime("1/06/2016"),
                endDate = Convert.ToDateTime("10/06/2016"),
               Tags = new int[] {50 }
            }).Result;

            Assert.IsFalse(res.Success);
            Assert.AreEqual("This tag id(50) does not exist", res.Message);
        }

        [TestMethod]
        public void Update_Event()
        {
            var res = eventService.UpdateEvent(new CreateEvent()
            {
                title = "cenas",
                Id = 3,
                UserId = "1"
            }).Result;

            var res2 = eventService.GetByIdAsync(res.Result).Result;
            Assert.AreEqual(res2.Result.title, "cenas");

            var res3 = eventService.UpdateEvent(new CreateEvent()
            {
                title = "oracle",
                Id = 3,
                UserId = "1"
            }).Result;
        }

        [TestMethod]
        public void Update_Event_Not_Exist()
        {
            var res = eventService.UpdateEvent(new CreateEvent()
            {
                title = "cenas",
                Id = 50,
                UserId = "1"
            }).Result;

            Assert.IsFalse(res.Success);
            Assert.AreEqual(EventService.Messages.EVENT_NOT_EXIST, res.Message);
        }
        [TestMethod]
        public void Update_Event_User_No_Permission()
        {
            var res = eventService.UpdateEvent(new CreateEvent()
            {
                title = "cenas",
                Id = 1,
                UserId = "1"
            }).Result;

            Assert.IsFalse(res.Success);
            Assert.AreEqual(EventService.Messages.USER_NO_PERMISSION, res.Message);
        }

        [TestMethod]
        public void Insert_Event_Subscriber()
        {
            var res = eventService.InsertSubscriber(4, "2").Result;

            var result = eventService.GetByIdAsync(4).Result.Result;

            Assert.IsTrue(result.eventSubscribers.Select(x => x.userId).Contains("2"));

            var remove = eventService.RemoveSubscriber(4, "2").Result;
        }
        
        [TestMethod]
        public void Insert_Event_Subscriber_Already_There()
        {
            var res = eventService.InsertSubscriber(1, "2").Result;

            Assert.IsFalse(res.Success);
            Assert.AreEqual(EventService.Messages.EVENT_HAS_SUBSCRIBER, res.Message);
        }

        [TestMethod]
        public void Insert_Event_Tag()
        {
            var res = eventService.InsertTag(1, "2", new int[] { 1, 2, 2 }).Result;

            var result = eventService.GetByIdAsync(1).Result.Result;

            Assert.AreEqual(3, result.tag.Count);
            Assert.IsTrue(result.tag.Select(x => x.id).Contains(1));
            Assert.IsTrue(result.tag.Select(x => x.id).Contains(2));

            var remove = eventService.RemoveTag(1, "2", new int[] { 1, 1, 2, 2 }).Result;
        }
        
        [TestMethod]
        public void Insert_Event_Tag_Already_Exist()
        {
            var res = eventService.InsertTag(1, "2", new int[] { 7, 2 }).Result;

            Assert.IsFalse(res.Success);
            Assert.AreEqual(EventService.Messages.EVENT_TAG_EXIST, res.Message);
        }
        
        [TestMethod]
        public void Insert_Event_Tag_No_Permission()
        {
            var res = eventService.InsertTag(1, "1", new int[] { 2 }).Result;

            Assert.IsFalse(res.Success);
            Assert.AreEqual(EventService.Messages.USER_NO_PERMISSION, res.Message);
        }

        [TestMethod]
        public void Check_In_Success()
        {
            var res = eventService.SelfCheckIn("3", 1, "123").Result;
            Assert.IsTrue(res.Result);

            var res2 = eventService.GetByIdAsync(1).Result;
            var subscriber = res2.Result.eventSubscribers.FirstOrDefault(elem => (elem.userId == "3" && elem.checkIn==true));
            Assert.IsTrue(subscriber.checkIn.Value);

            var res3 = eventService.UpdateCheckInSubscriber(subscriber).Result;
        }


    }
}
