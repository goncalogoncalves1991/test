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
    public class CommentTests
    {
        private CommentRepository commentRepo;

        [TestInitialize]
        public void initialize()
        {
            commentRepo = new CommentRepository();
        }

        

        [TestMethod]
        public void CreateEventComment()
        {
            int newCommentEventID = 0;

            try
            {
                newCommentEventID = commentRepo.PostAsync(new CreateComment() { authorId = "1", message = "ola ola", initialDate = DateTime.Now, eventId = 1 }).Result;
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex.InnerException, typeof(ArgumentException));
            }
            var res = commentRepo.GetByIdAsync(newCommentEventID).Result;
            Assert.AreEqual(res.message, "ola ola");
            var id = commentRepo.DeleteAsync(res).Result;

        }

        
        [TestMethod]
        public void UpdateEventComment()
        {
            var eventcomment = commentRepo.GetByIdAsync(1).Result;
            eventcomment.message = "mau evento";
            var id = commentRepo.PutAsync(eventcomment).Result;
            var eventcomment2 = commentRepo.GetByIdAsync(1).Result;
            Assert.AreEqual(eventcomment2.message, "mau evento");

            eventcomment.message = "Fantático evento fico a espera do proximo, grandes oradores";
            var final = commentRepo.PutAsync(eventcomment).Result;

        }

        [TestMethod]
        public void CreateCommunityComment()
        {
            int newCommentCommunityID = 0;

            try
            {
                newCommentCommunityID = commentRepo.PostAsync(new CreateComment() { authorId = "2", message = "somos bons", initialDate = DateTime.Now, communityId = 2 }).Result;
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex.InnerException, typeof(ArgumentException));
            }
            var res = commentRepo.GetByIdAsync(newCommentCommunityID).Result;
            Assert.AreEqual(res.message, "somos bons");
            var id = commentRepo.DeleteAsync(res).Result;

        }


        [TestMethod]
        public void UpdateCommunityComment()
        {
            var communitycomment = commentRepo.GetByIdAsync(1).Result;
            communitycomment.message = "cenas";
            var id = commentRepo.PutAsync(communitycomment).Result;
            var eventcomment2 = commentRepo.GetByIdAsync(1).Result;
            Assert.AreEqual(eventcomment2.message, "cenas");

            communitycomment.message = "Grande comunidade cheia de grande eventos";
            var final = commentRepo.PutAsync(communitycomment).Result;

        }
       
    }
}
