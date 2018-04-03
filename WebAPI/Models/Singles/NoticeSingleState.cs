using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Models.Collections;

namespace WebAPI.Models.Singles
{
    public class NoticeSingleState
    {
        public int id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public DateTime date { get; set; }
        public CommunitiesCollectionState community { get; set; }
        public Link _links { get; set; }
        public class Link
        {
            public Uri self { get; set; }
            public Uri community { get; set; }
        }
    }
}
