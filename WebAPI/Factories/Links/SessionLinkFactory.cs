using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Controllers;

namespace WebAPI.Factories.Links
{
    public class SessionLinkFactory : LinkFactory<SessionsController>
    {
        public SessionLinkFactory(HttpRequestMessage request)
            : base(request){}

        public new class Rels : LinkFactory.Rels
        {
            public const string Questions = "questions";
        }
        public Uri Event(int id)
        {
            return GetLink<EventsController>(id, null, null);
        }
        public Uri Questions(int id)
        {
            return GetLink<SessionsController>(id, Rels.Questions, null);
        }
    }
}
