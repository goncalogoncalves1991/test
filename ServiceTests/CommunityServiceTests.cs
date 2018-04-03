using DataAccess.Models.Create;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Services.Models.Roles;
using Services.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceTests
{
    [TestClass]
    public class CommunityServiceTests
    {
        private CommunityService communityService;

        [TestInitialize]
        public void initialize()
        {
            communityService = CommunityService.GetInstance();
        }
        [TestMethod]
        public void GetCommunityById()
        {
            var res = communityService.GetByIdAsync(1).Result;
            Assert.IsTrue(res.Success);
        }
        [TestMethod]
        public void GetAllCommunities()
        {
            var res = communityService.GetAllAsync().Result;
            Assert.IsTrue(res.Success);
            Assert.AreEqual(3, res.Result.Count());
        }
        [TestMethod]
        public void GetCommunitiesFromUserByRoleMember()
        {
            var res = communityService.GetCommunitiesFromUserByRole("1", Role.Member).Result;
            Assert.IsTrue(res.Success);
            Assert.AreEqual(2, res.Result.Count());
        }
        [TestMethod]
        public void GetCommunitiesFromUserByRoleAdmin()
        {
            var res = communityService.GetCommunitiesFromUserByRole("3",Role.Admin).Result;
            Assert.IsTrue(res.Success);
            Assert.AreEqual(1, res.Result.Count());
           
        }
        [TestMethod]
        public void GetCommunitiesFromUserByRoleAdmin_NotSuccefull()
        {
            var res = communityService.GetCommunitiesFromUserByRole("100", Role.Admin).Result;
            Assert.IsFalse(res.Success);
        }


        [TestMethod]
        public void Create_Community_Success()
        {
            var res = communityService.CreateCommunity(new CreateCommunity()
            {
                Local = "Lisboa",
                UserId = "1",
                Name="silicon valey",
                Tags=new int[]{3,7},
                Sponsors = new int[] {1}
            }).Result;

            var res2 = communityService.GetByIdAsync(res.Result).Result;
            Assert.AreEqual(res2.Result.name, "silicon valey");
            Assert.AreEqual(2, res2.Result.tag.Count());
            var id = communityService.DeleteCommunity(new CreateCommunity()
            {
                UserId = "1",
                Id= res2.Result.id
            }).Result;
        }

        [TestMethod]
        public void Create_Community_Name_Exists()
        {
            
            var res = communityService.CreateCommunity(new CreateCommunity()
            {
                Local = "Lisboa",
                UserId = "2",
                Name = "cenas"
            }).Result;

            Assert.IsFalse(res.Success);
            Assert.AreEqual("This name already exist",res.Message);
            
        }

        [TestMethod]
        public void Create_Community_User_Not_Exists()
        {
            var res = communityService.CreateCommunity(new CreateCommunity()
            {
                Local = "Lisboa",
                UserId = "1000",
                Name = "silicon valey"
            }).Result;

            Assert.IsFalse(res.Success);
            Assert.AreEqual(UserService.Messages.USER_NOT_EXIST+" (1000)", res.Message);
        }

        [TestMethod]
        public void Create_Community_Tag_Not_Exists()//same to sponsors
        {
            var res = communityService.CreateCommunity(new CreateCommunity()
            {
                Local = "Lisboa",
                UserId = "1",
                Name = "silicon valey",
                Tags = new int[] { 100 }
            }).Result;

            Assert.IsFalse(res.Success);
            Assert.AreEqual("This tag id(100) does not exist", res.Message);
        }                

        
        [TestMethod]
        public void Update_Community()
        {
            var res = communityService.UpdateCommunity(new CreateCommunity() { 
                Local="Albufeira",
                Id=1,
                UserId="1",
                Name="lalalalal"//é ignorado
            }).Result;

            var res2 = communityService.GetByIdAsync(res.Result).Result;
            Assert.AreEqual(res2.Result.local, "Albufeira");
            Assert.AreEqual(res2.Result.name, "cenas");

            var res3 = communityService.UpdateCommunity(new CreateCommunity()
            {
                Local = "Lisboa",
                Id = 1,
                UserId = "1"
            }).Result;
        }

        [TestMethod]
        public void Update_Community_User_No_Permission()
        {
            var res = communityService.UpdateCommunity(new CreateCommunity()
            {
                Local = "Albufeira",
                Id = 1,
                UserId = "2"
            }).Result;

            Assert.IsFalse(res.Success);
            Assert.AreEqual(CommunityService.Messages.USER_NOT_PERMISSION, res.Message);
        }

        [TestMethod]
        public void Update_Community_Community_Not_Exist()
        {
            var res = communityService.UpdateCommunity(new CreateCommunity()
            {
                Local = "Albufeira",
                Id = 100,
                UserId = "2"
            }).Result;

            Assert.IsFalse(res.Success);
            Assert.AreEqual(CommunityService.Messages.COMMUNITY_NOT_EXIST, res.Message);
        }

        [TestMethod]
        public void Insert_Community_Member()
        {
            var res = communityService.InsertMember(2,"3").Result;

            var result = communityService.GetByIdAsync(2).Result.Result;

            Assert.IsTrue(result.members.Select(x=> x.id).Contains("3"));

            var remove = communityService.RemoveMember(2, "3").Result;
        }

        [TestMethod]
        public void Insert_Community_Member_Already_There()
        {
            var res = communityService.InsertMember(2, "1").Result;

            Assert.IsFalse(res.Success);
            Assert.AreEqual(CommunityService.Messages.COMMUNITY_HAS_MEMBER, res.Message);
        }

        [TestMethod]
        public void Insert_Community_Admin()
        {
            var res = communityService.InsertAdmin(1, "1", "2").Result;

            var result = communityService.GetByIdAsync(1).Result.Result;

            Assert.IsTrue(result.admins.Select(x => x.id).Contains("2"));

            var remove = communityService.RemoveAdmin(1, "1", "2").Result;
        }

        [TestMethod]
        public void Insert_Community_Admin_Already_There()
        {
            var res = communityService.InsertAdmin(1, "1", "1").Result;

            Assert.IsFalse(res.Success);
            Assert.AreEqual(CommunityService.Messages.COMMUNITY_HAS_ADMIN, res.Message);
        }

        [TestMethod]
        public void Insert_Community_Admin_No_Permission()
        {
            var res = communityService.InsertAdmin(1, "2", "2").Result;

            Assert.IsFalse(res.Success);
            //Assert.AreEqual(CommunityService.Messages.USER_NOT_ADMIN, res.Message);
        }

        [TestMethod]
        public void Insert_Community_Tag()
        {
            var res = communityService.InsertTag(1, "1", new int[] {1,2,2}).Result;

            var result = communityService.GetByIdAsync(1).Result.Result;
            
            Assert.AreEqual(4, result.tag.Count);
            Assert.IsTrue(result.tag.Select(x=> x.id).Contains(1));
            Assert.IsTrue(result.tag.Select(x=> x.id).Contains(2));

            var remove = communityService.RemoveTag(1, "1", new int[] { 1, 1, 2, 2 }).Result;
        }

        [TestMethod]
        public void Insert_Community_Tag_Already_Exist()
        {
            var res = communityService.InsertTag(1, "1", new int[] { 5, 2 }).Result;

            Assert.IsFalse(res.Success);
            Assert.AreEqual(CommunityService.Messages.COMMUNITY_TAG_EXIST, res.Message);
        }

        [TestMethod]
        public void Insert_Community_Tag_No_Permission()
        {
            var res = communityService.InsertTag(1, "2", new int[] {  2 }).Result;

            Assert.IsFalse(res.Success);
           // Assert.AreEqual(CommunityService.Messages.USER_NOT_ADMIN, res.Message);
        }

        [TestMethod]
        public void Search_Success()
        {
            var res = communityService.Search(null,"tugf",null).Result;
            
             Assert.AreEqual(res.Result.Count(), 1);
            
        }

    }
}
