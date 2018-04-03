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
    public class NoticeServiceTests
    {
        private NoticeService noticeService;

        [TestInitialize]
        public void initialize()
        {
            noticeService = NoticeService.GetInstance();
        }

        [TestMethod]
        public void GetEventById()
        {
            var result = noticeService.GetByIdAsync(1).Result;
            Assert.AreEqual(true, result.Success);
        }
        [TestMethod]
        public void GetAllEventsAsync()
        {
            var result = noticeService.GetAllAsync().Result;
            Assert.IsTrue(result.Success);
            Assert.AreEqual(3, result.Result.Count());
        }
        [TestMethod]
        public void GetAllNoticesFromCommunity()
        {
            var notices = noticeService.GetNoticesFromCommunity(2).Result;
            Assert.IsTrue(notices.Success);
            Assert.AreEqual(2, notices.Result.Count());
        }
        [TestMethod]
        public void GetAllNoticesFromCommunity_NotSuccefull()
        {
            var notices = noticeService.GetNoticesFromCommunity(100).Result;
            Assert.IsFalse(notices.Success);
            
        }

        [TestMethod]
        public void Create_Notice_Success()
        {
            var res = noticeService.PostNoticeAsync(new CreateNotice()
            {
               communityId=1,
               userId="1",
               initialDate=DateTime.Now,
               title="grande noticia",
               description="grande descrição................."
            }).Result;

            var res2 = noticeService.GetByIdAsync(res.Result).Result;
            Assert.AreEqual(res2.Result.title, "grande noticia");
            Assert.AreEqual(res2.Result.description, "grande descrição.................");
            var id = noticeService.DeleteNotice(new CreateNotice()
            {
                userId="1",
                id = res2.Result.id
            }).Result;
        }
        
        [TestMethod]
        public void Create_Notice_Community_Not_Exist()
        {
            var res = noticeService.PostNoticeAsync(new CreateNotice()
            {
                communityId = 50,
                userId = "1",
                initialDate = DateTime.Now,
                title = "grande noticia",
                description = "grande descrição................."
            }).Result;

            Assert.IsFalse(res.Success);
            Assert.AreEqual(CommunityService.Messages.COMMUNITY_NOT_EXIST, res.Message);
        }
        [TestMethod]
        public void Create_Notice_User_No_Permission()
        {
            var res = noticeService.PostNoticeAsync(new CreateNotice()
            {
                communityId = 1,
                userId = "2",
                initialDate = DateTime.Now,
                title = "grande noticia",
                description = "grande descrição................."
            }).Result;

            Assert.IsFalse(res.Success);
            Assert.AreEqual(NoticeService.Messages.USER_NO_PERMISSION, res.Message);
        }

        
        [TestMethod]
        public void Update_Notice()
        {
            var res = noticeService.UpdateNotice(new CreateNotice()
            {
                id = 1,
                userId = "2",
                title = "grande noticia"
            }).Result;

            var res2 = noticeService.GetByIdAsync(res.Result).Result;
            Assert.AreEqual(res2.Result.title, "grande noticia");

            var res3 = noticeService.UpdateNotice(new CreateNotice()
            {
                id = 1,
                userId = "2",
                title = "Lançamento da nova versão de JAVA"
            }).Result;
        }
        
        [TestMethod]
        public void Update_Notice_No_Permission()
        {
            var res = noticeService.UpdateNotice(new CreateNotice()
            {
                id = 1,
                userId = "1",
                title = "grande noticia"
            }).Result;

            Assert.IsFalse(res.Success);
            Assert.AreEqual(NoticeService.Messages.USER_NO_PERMISSION, res.Message);
        }

    }
}
