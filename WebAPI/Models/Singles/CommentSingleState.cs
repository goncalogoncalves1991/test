using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Models.Collections;

namespace WebAPI.Models.Singles
{
    public class CommentSingleState 
    {
        public int id { get; set; }
        
        public string message { get; set; }
        public DateTime date { get; set; }

        public virtual UsersCollectionState user { get; set; }
        public virtual CommunitiesCollectionState community { get; set; }
        public virtual EventsCollectionState @event { get; set; }
        public Link _Links { get; set; }
        public class Link
        {
            public Uri Self { get; set; }
            public Uri Event { get; set; }
            public Uri Community { get; set; }
        }


        
    }
}
