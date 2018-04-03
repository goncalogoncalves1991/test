
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
    public class CommunityTests
    {
        CommunityRepository com;

        [TestInitialize]
        public void initialize()
        {
            com = new CommunityRepository();
        }

        
        [TestMethod]
        public void GetCommunities()
        {
            IEnumerable<community> res = com.GetAllAsync().Result;

            int count = res.Count();
            Assert.AreEqual(3, count);
        }
        
        [TestMethod]
        public void GetCommunity()
        {
            community res = com.GetByIdAsync(1).Result;

            Assert.AreEqual("cenas", res.name);
        }        
        
        /*
        [TestMethod]
        public void GetCommunityByName()
        {
            DataAccess.Models.DetailModels.Communities res = com.GetByNameAsync("tugaflix").Result;

            Assert.AreEqual("Algarve", res.Local);
        }

        [TestMethod]
        public void GetCommunityByTags()
        {
            IEnumerable<DataAccess.Models.DetailModels.Communities> res = com.GetByTagsAsync(new string[] { "coisas", "java", "1234" }).Result;

            Assert.AreEqual(2, res.Count());
        }

        [TestMethod]
        public void GetCommunityByTagsAndLocation()
        {
            IEnumerable<Communities> res = com.GetByTagsAndLocationAsync("Porto", new string[] { "coisas", "java", "1234" }, e => ((Community)e).Local).Result;

            Assert.AreEqual(1, res.Count());
            Assert.AreEqual("netponto", res.First().Name);
        }

        */

        [TestMethod]
        public void CreateCommunity()
        {            
            int newCommunityID=0;

            try
            {
                newCommunityID = com.PostAsync(new CreateCommunity() { Name = "Break", Local = "rua street", Description = "somos breakers", FoundationDate = DateTime.Now, UserId = "2", Tags = new int[] { 4, 3 } }).Result;
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex.InnerException,typeof(ArgumentException));
            }
            var res = com.GetByIdAsync(newCommunityID).Result;
            Assert.AreEqual(res.name, "Break");
            Assert.AreEqual(2, res.tag.Count());
            var id = com.DeleteAsync(res).Result;

        }

        [TestMethod]
        public void CreateCommunityError()
        {// unique name error, because is the only one validated in BD the others must be validate in services
            int newCommunityID = 0;

            try
            {
                newCommunityID = com.PostAsync(new CreateCommunity() { Name = "cenas", Local = "rua street", Description = "somos breakers", FoundationDate = DateTime.Now, UserId = "2", Tags = new int[] { 4, 88 } }).Result;
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex.InnerException, typeof(ArgumentException));
            }

        }
        
        [TestMethod]
        public void UpdateCommunity()
        {
            var community = com.GetByIdAsync(2).Result;
            community.description = "somos netpontanos";
            var id=com.PutAsync( community).Result;
            var community2 = com.GetByIdAsync(2).Result;
            Assert.AreEqual(community.name, "netponto");
            Assert.AreEqual(community.description, "somos netpontanos");

            community.description = "somos fixes";
            var final = com.PutAsync(community).Result;

        }
        
        [TestMethod]
        public void InsertCommunityMember()
        {
            string id = com.InsertMember(3, "2").Result;
            var community = com.GetByIdAsync(3).Result;
            Assert.AreEqual(2, community.members.Count);

            string id2 = com.DeleteMember(3, "2").Result;         
        }

        [TestMethod]
        public void InsertCommunityAdmin()
        {
            string id = com.InsertAdmin(3, "2").Result;
            var community = com.GetByIdAsync(3).Result;
            Assert.AreEqual(2, community.admins.Count);

            string id2 = com.DeleteAdmin(3, "2").Result;

        }

        [TestMethod]
        public void InsertCommunityTag()
        {
            bool success = com.InsertTag(1, new int[]{1,2}).Result;
            var community = com.GetByIdAsync(1).Result;
            Assert.AreEqual(4, community.tag.Count);
            Assert.IsNotNull(community.tag.First(t => t.name == "java"));
            Assert.IsNotNull(community.tag.First(t => t.name == "eclipse"));

            bool id2 = com.DeleteTag(1, new int[] { 1, 2 }).Result;
        }

        [TestMethod]
        public void InsertCommunitySponsor()
        {
            bool success = com.InsertSponsor(1, new int[] { 1 }).Result;
            var community = com.GetByIdAsync(1).Result;
            Assert.AreEqual(2, community.sponsor.Count);
            Assert.IsNotNull(community.sponsor.First(t => t.name == "Microsoft"));

            bool id2 = com.DeleteSponsor(1, new int[] { 1}).Result; ;
        }

    }
}