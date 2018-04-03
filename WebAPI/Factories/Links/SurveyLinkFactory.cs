using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace WebAPI.Factories.Links
{
    public class SurveyLinkFactory
    {
        private HttpRequestMessage request;

        public SurveyLinkFactory(HttpRequestMessage request)
        {
            this.request = request;
        }
    }
}