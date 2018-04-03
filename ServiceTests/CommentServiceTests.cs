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
    public class CommentServiceTest
    {
        private CommentService commentService;

        [TestInitialize]
        public void initialize()
        {

            commentService = CommentService.GetInstance();
        }

        [TestMethod]
        public void GetCommentById()
        {
            var result = commentService.GetCommentByIdAsync(1).Result;
            Assert.AreEqual(true, result.Success);
        }


        [TestMethod]
        public void GetAllcommentFromCommunity()
        {
            var result = commentService.GetAllCommentsFromCommunity(2).Result;
            Assert.AreEqual(true, result.Success);
            Assert.AreEqual(2, result.Result.Count());
        }
        [TestMethod]
        public void GetAllcommentFromEvent()
        {
            var result = commentService.GetAllCommentsFromEvent(1).Result;
            Assert.AreEqual(true, result.Success);
            Assert.AreEqual(1, result.Result.Count());
        }

        [TestMethod]
        public void InsertCommentIntoEvent()
        {/*
            var result = commentService.PostComment(new CreateComment { authorId = "2", message = "grande evento", eventId = 2 }).Result;
            var res2 = commentService.GetCommentByIdAsync(result.Result).Result;
            Assert.AreEqual(res2.Result.message, "grande evento");
            var id = commentService.DeleteComment(new CreateComment()
            {
                id = res2.Result.id,
                authorId = "2"
            }).Result;*/
        }

        [TestMethod]
        public void InsertCommentIntoEvent_with_community()
        {/*
            var result = commentService.PostComment(new CreateComment { authorId = "2", message = "grande evento", eventId = 2, communityId=1 }).Result;

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommentService.Messages.PARAMETERS_NOT_NULL, result.Message);*/
        }
    }
}

