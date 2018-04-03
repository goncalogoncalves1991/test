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
    public class UserTests
    {
        UserRepository userRepo;
        [TestInitialize]
        public void initialize()
        {
            userRepo = new UserRepository();
        }
        [TestMethod]
        public void GetUser()
        {
            var res = userRepo.GetByIdAsync("1").Result;

            Assert.AreEqual("João", res.name);
        }
        [TestMethod]
        public void GetUser_Id_does_not_exist()
        {
            var res = userRepo.GetByIdAsync("100").Result;
            Assert.AreEqual(null, res);
        }
        [TestMethod]
        public void GetUsers()
        {
            IEnumerable<userInfo> res = userRepo.GetAllAsync().Result;
            Assert.AreEqual(3, res.Count());
        }
      

        [TestMethod]
        public void CreateEvent()
        {
            string newUserID=null;

            try
            {
                newUserID = userRepo.PostAsync(new CreateUser() { id="5", name = "Zé", lastName="Carlos", local = "tras-dos-montes", email="zecarlos@gmail.com", registerDate=DateTime.Now }).Result;
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex.InnerException, typeof(ArgumentException));
            }
            var res = userRepo.GetByIdAsync(newUserID).Result;
            Assert.AreEqual(res.name, "Zé");
            var id = userRepo.DeleteAsync(res).Result;

        }


        [TestMethod]
        public void UpdateEvent()
        {
            var user = userRepo.GetByIdAsync("2").Result;
            user.name = "Obama";
            var id = userRepo.PutAsync(user).Result;
            var user2 = userRepo.GetByIdAsync("2").Result;
            Assert.AreEqual(user2.name, "Obama");
            Assert.AreEqual(user2.lastName, "Manuel");

            user.name = "Pedro";
            var final = userRepo.PutAsync(user).Result;

        }
        
    }
}
