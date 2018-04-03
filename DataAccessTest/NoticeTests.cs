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
    public class NoticeTests
    {
        private NoticeRepository noticeRepo;

        [TestInitialize]
        public void initialize()
        {
            noticeRepo = new NoticeRepository();
        }

        [TestMethod]
        public void GetNotices()
        {
            var res = noticeRepo.GetAllAsync().Result;

            int count = res.Count();
            Assert.AreEqual(3, count);
        }

        [TestMethod]
        public void GetNotice()
        {
            var res = noticeRepo.GetByIdAsync(1).Result;

            Assert.AreEqual("Lançamento da nova versão de JAVA", res.title);
        }

        [TestMethod]
        public void CreateNotice()
        {
            int newNoticeID = 0;

            try
            {
                newNoticeID = noticeRepo.PostAsync(new CreateNotice() { title="noticia boa", description="grande noticia.....",initialDate=DateTime.Now,communityId=3 }).Result;
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex.InnerException, typeof(ArgumentException));
            }
            var res = noticeRepo.GetByIdAsync(newNoticeID).Result;
            Assert.AreEqual(res.title, "noticia boa");
            var id = noticeRepo.DeleteAsync(res).Result;

        }


        [TestMethod]
        public void UpdateSession()
        {
            var notice = noticeRepo.GetByIdAsync(1).Result;
            notice.title = "hahahahahaha";
            var id = noticeRepo.PutAsync(notice).Result;
            var notice2 = noticeRepo.GetByIdAsync(1).Result;
            Assert.AreEqual(notice2.title, "hahahahahaha");

            notice.title = "Lançamento da nova versão de JAVA";
            var final = noticeRepo.PutAsync(notice).Result;

        }
    }
}
