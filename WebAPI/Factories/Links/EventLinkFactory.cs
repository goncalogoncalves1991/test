using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Controllers;

namespace WebAPI.Factories.Links
{
    public class EventLinkFactory : LinkFactory<EventsController>
    {
        public new class Rels : LinkFactory.Rels {
            public const string Community = "community";
            public const string Sessions = "sessions";
            public const string Tags = "tags";
            public const string Comments = "comments";
            public const string Subscribers = "subscribers";
        }


        public EventLinkFactory(HttpRequestMessage request)
            : base(request)
        {
        }
   
        public Uri Community(int id)
        {
            return GetLink<CommunitiesController>(id, null, null);
        }
        public Uri Sessions(int id)
        {
            return GetLink<EventsController>(id, Rels.Sessions, null);
        }
        
        public Uri SubscribersCheckedIn(int id)
        {
            return GetLink<EventsController>(id, Rels.Subscribers, new QueryString { name = "isCheckedIn",value = "true"});
        }
        public Uri Subscribers(int id)
        {
            return GetLink<EventsController>(id, Rels.Subscribers, new QueryString { name = "isCheckedIn", value = "false" });
        }
        public Uri Comments(int id)
        {
            return GetLink<EventsController>(id, Rels.Comments, null);
        }
        

    }
}
