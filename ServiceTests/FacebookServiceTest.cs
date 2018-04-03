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
    public class FacebookServiceTest
    {
        [TestMethod]
        public void PostFeed()
        {
            var result = FacebookService.sendFeed("eventcommittest","from facebook service test", "EAAYswjZALtjQBAMZBg5VSChCN6Y3c0ZBlb9xIcAEKQ8jj2ijIP37fkO88cZB6UiP1wQW83GwSGz6lzLTXbqldNHfOalFqWhofQvbHO9ZBq8ZC4u84k7n2XvqOMv3a7YeHcUpLu3xfZCAg2suViZCIyRbFZCTa7ObyXc4ZD").Result;
            Assert.AreEqual(true, result.Success);
        }
    }
}
