using Microsoft.VisualStudio.TestTools.UnitTesting;
using Services.Services.Other;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceTests
{
    [TestClass]
    public class ImageServiceTests
    {
        [TestMethod]
        public void SendToAws_Success()
        {
            using (FileStream fs = File.OpenRead("D:\\PS\\Projecto\\EventCommit\\ServiceTests\\balloon.png"))
            {
                var res = ImageService.SendToProvider(2, ImageService.ImageIdentity.Communities,fs,ImageService.ImageType.Avatar).Result;
                Assert.IsTrue(res.Success);
                Assert.IsNotNull(res.Result);
            }
        }

        [TestMethod]
        public void SendToAws_Fail()
        {
            using (FileStream fs = File.OpenRead("D:\\PS\\Projecto\\EventCommit\\ServiceTests\\bla.txt"))
            {
                var res = ImageService.SendToProvider(2, ImageService.ImageIdentity.Communities, fs, ImageService.ImageType.Avatar).Result;
                Assert.IsFalse(res.Success);
            }
        }
        /*
        [TestMethod]
        public void RemoveToAws()
        {
            var res = ImageService.DeleteFromProvider("Communities/2/56ff9a04-6787-4c5e-9d5b-6af9b5792511.png").Result;
            Assert.IsTrue(res.Success);
            
        }*/
    }
}
