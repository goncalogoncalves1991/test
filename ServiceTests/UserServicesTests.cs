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
    public class UserServicesTests
    {
        private UserService userService;

        [TestInitialize]
        public void initialize()
        {
            userService = UserService.GetInstance();
        }
        [TestMethod]
        public void GetUserById()
        {
            var result = userService.GetByIdAsync("1").Result;
            Assert.AreEqual(true, result.Success);
        }
        [TestMethod]
        public void GetUsers()
        {
            var res = userService.GetAllAsync().Result;
            Assert.IsTrue(res.Success);
            Assert.AreEqual(3, res.Result.Count());
        }
        [TestMethod]
        public void GetMembersOfCommunity()
        {
            var res = userService.GetUserFromCommunityByRole(1,Role.Member).Result;
            Assert.IsTrue(res.Success);
            Assert.AreEqual(2, res.Result.Count());
        }
        [TestMethod]
        public void GetAdminsOfCommunity()
        {
            var res = userService.GetUserFromCommunityByRole(3, Role.Admin).Result;
            Assert.IsTrue(res.Success);
            Assert.AreEqual(1, res.Result.Count());
        }

        [TestMethod]
        public void GetAdminsOfCommunity_NotSuccefull()
        {
            var res = userService.GetUserFromCommunityByRole(100, Role.Admin).Result;
            Assert.IsFalse(res.Success);
        }
        [TestMethod]
        public void GetSubscribedUsersFromEvente_CheckedIn()
        {
            var res = userService.GetUsersSubscribedOnEvent(1, UserService.Check_in.True).Result;
            Assert.IsTrue(res.Success);
            Assert.AreEqual(2, res.Result.Count());
        }
        [TestMethod]
        public void GetSubscribedUsersFromEvent_Not_CheckedIn()
        {
            var res = userService.GetUsersSubscribedOnEvent(5, UserService.Check_in.False).Result;
            Assert.IsTrue(res.Success);
            Assert.AreEqual(1, res.Result.Count());
        }
        [TestMethod]
        public void GetSubscribedUsersFromEvent_NotSuccefull()
        {
            var res = userService.GetUsersSubscribedOnEvent(100, UserService.Check_in.False).Result;
            Assert.IsFalse(res.Success);
        }

        [TestMethod]
        public void Create_User_Success()
        {
            var res = userService.PostUserAsync(new CreateUser()
            {
                id="5",
                name="andre",
                lastName="machado",
                email="andremachado@gmail.com",
                local="Lisboa",
                registerDate=DateTime.Now
            }).Result;

            var res2 = userService.GetByIdAsync(res.Result).Result;
            Assert.AreEqual(res2.Result.name, "andre");
            Assert.AreEqual(res2.Result.email, "andremachado@gmail.com");
            var id = userService.DeleteUser(new CreateUser()
            {
                id = res2.Result.id
            }).Result;
        }

        [TestMethod]
        public void Create_User_Email_Exists()
        {
            var res = userService.PostUserAsync(new CreateUser()
            {
                id="5",
                name = "andre",
                lastName = "machado",
                email = "joaopedro@gmail.com",
                local = "Lisboa",
                registerDate = DateTime.Now
            }).Result;

            Assert.IsFalse(res.Success);
            Assert.AreEqual(UserService.Messages.EMAIL_EXISTS, res.Message);
        }

        [TestMethod]
        public void Update_User()
        {
            var res = userService.UpdateUser(new CreateUser()
            {
                name = "cenas",
                id = "1"
            }).Result;

            var res2 = userService.GetByIdAsync(res.Result).Result;
            Assert.AreEqual(res2.Result.name, "cenas");

            var res3 = userService.UpdateUser(new CreateUser()
            {
                name = "João",
                id = "1"
            }).Result;
        }

        [TestMethod]
        public void Update_User_Email_Exists()
        {
            var res = userService.UpdateUser(new CreateUser()
            {
                email = "pedromanuel@gmail.com",
                id = "1"
            }).Result;

            Assert.IsFalse(res.Success);
            Assert.AreEqual(UserService.Messages.EMAIL_EXISTS, res.Message);
        }

    }
}
