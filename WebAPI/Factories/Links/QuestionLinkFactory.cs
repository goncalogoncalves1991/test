using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Controllers;

namespace WebAPI.Factories.Links
{
    public class QuestionLinkFactory : LinkFactory<QuestionsController>
    {
        public QuestionLinkFactory(HttpRequestMessage request)
            : base(request){}


        public Uri User(string id)
        {
            return GetLink<UsersController>(id, null, null);
        }
        public Uri Session(int id)
        {
            return GetLink<SessionsController>(id, null, null);
        }
    }
}
