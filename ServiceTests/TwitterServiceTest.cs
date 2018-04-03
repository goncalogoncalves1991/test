using Microsoft.VisualStudio.TestTools.UnitTesting;
using Services.Services.Other;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceTests
{
    [TestClass]
    public class TwitterServiceTest
    {
        [TestMethod]
        public void PostTweet()
        {
            var result = TwitterService.sendTweet("message test", "moAbcZNdY3XLVbF4Wwqhc323SCg4B9pCvPGRgdjVuvXNw:2389820719-CcM1nr4fi6tRKN5NgosImml5utadrapXEIP58Ng").Result;
            Assert.AreEqual(true, result.Success);
        }
    }
}
