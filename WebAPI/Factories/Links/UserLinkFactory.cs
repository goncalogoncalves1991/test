using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Controllers;

namespace WebAPI.Factories.Links
{
    public class UserLinkFactory : LinkFactory<UsersController>
    {
        public UserLinkFactory(HttpRequestMessage request)
            : base(request){}
        public new class Rels : LinkFactory.Rels
        {
            public const string Events = "events";
            public const string Communities = "communities";
        }
        public Uri events(string id)
        {
            return GetLink<UsersController>(id, Rels.Events, null);
        }
        public Uri CommunitiesWhereUserIsAdmin(string id)
        {
            return GetLink<UsersController>(id, Rels.Communities, new QueryString { name = "type", value = "admin" });
        }
        public Uri CommunitiesWhereUserIsMember(string id)
        {
            return GetLink<UsersController>(id, Rels.Communities, new QueryString { name = "type", value = "member" });
        }
    }
}
