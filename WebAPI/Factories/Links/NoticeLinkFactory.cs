using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Controllers;

namespace WebAPI.Factories.Links
{
    public class NoticeLinkFactory : LinkFactory<NoticesController>
    {
        public NoticeLinkFactory(HttpRequestMessage request)
            : base(request){}
        
        public new class Rels : LinkFactory.Rels
        {
            public const string Community = "community";
        }
        public Uri Community(int id)
        {
            return GetLink<CommunitiesController>(id, null, null);
        }
    }
}
