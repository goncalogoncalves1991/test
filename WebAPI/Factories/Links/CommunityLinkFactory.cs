using System;
using System.Net.Http;
using WebAPI.Controllers;

namespace WebAPI.Factories.Links
{
    public class CommunityLinkFactory : LinkFactory<CommunitiesController>
    {
         public new class Rels : LinkFactory.Rels {
            public const string Events = "events";
            public const string Users = "users";
            public const string Notices = "notices";
            public const string Comments = "comments";

        }


         public CommunityLinkFactory(HttpRequestMessage request)
            : base(request)
        {
        }
   
        public Uri Events(int id)
        {
            return GetLink<CommunitiesController>(id, Rels.Events, new QueryString { name= "time", value="all"});
        }
        public Uri PastEvents(int id)
        {
            return GetLink<CommunitiesController>(id, Rels.Events, new QueryString { name = "time", value = "past" }) ;
        }
        public Uri FutureEvents(int id)
        {
            return GetLink<CommunitiesController>(id, Rels.Events, new QueryString { name = "time", value = "future" }) ;
        }

        public Uri Admins(int id)
        {
            return GetLink<CommunitiesController>(id, Rels.Users, new QueryString { name = "type", value = "admin" }) ;
        }
        public Uri Members(int id)
        {
            return GetLink<CommunitiesController>(id, Rels.Users, new QueryString { name = "type", value = "member" }) ;
        }
        public Uri Notices(int id)
        {
            return GetLink<CommunitiesController>(id, Rels.Notices, null) ;
        }
        public Uri Comments(int id)
        {
            return GetLink<CommunitiesController>(id, Rels.Comments, null) ;
        }
    }
}
