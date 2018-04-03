using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Controllers;

namespace WebAPI.Factories.Links
{
    public class CommentLinkFactory : LinkFactory<CommentsController>
    {
        public new class Rels : LinkFactory.Rels
        {
            public const string Community = "community";
            public const string events = "event";
        }
        public CommentLinkFactory(HttpRequestMessage request)
            : base(request){}

        public Uri Community(int id)
        {
            return GetLink<CommunitiesController>(id, null, null);
        }
        public Uri Event(int id)
        {
            return GetLink<EventsController>(id, null, null);
        }
    }
}
