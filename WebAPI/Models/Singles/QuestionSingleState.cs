using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Models.Collections;

namespace WebAPI.Models.Singles
{
    public class QuestionSingleState
    {
        public int id { get; set; }
        public string question { get; set; }
        public UsersCollectionState author { get; set; }
        public SessionsCollectionState session { get; set; }
        public Link _links { get; set; }
        public class Link
        {
            public Uri self { get; set; }
            public Uri session { get; set; }
            public Uri author { get; set; }
        }
    }
}
