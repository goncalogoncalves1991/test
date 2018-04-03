using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Models.Collections;

namespace WebAPI.Models.Singles
{
    public class UserSingleState
    {
        public string id { get; set; }
        public string local { get; set; }
        public string name { get; set; }
        public string lastName { get; set; }
        public string email { get; set; }
        public DateTime registerDate { get; set; }
        public string picture { get; set; }
        public string facebook { get; set; }
        public string twitter { get; set; }
        public string gitHub { get; set; }
        public string biography { get; set; }
        public string linkedin { get; set; }
        public IEnumerable<EventsCollectionState> subscribedEvents { get; set; }
        public IEnumerable<CommunitiesCollectionState> admin { get; set; }
        public IEnumerable<CommunitiesCollectionState> member { get; set; }
        public Link _links { get; set; }
        public class Link
        {
            public Uri self { get; set; }
            public Uri subscribedEvents { get; set; }
            public Uri admin { get; set; }
            public Uri member { get; set; }
        }
        
        /*public virtual ICollection<commentCommunity> commentCommunity { get; set; }
        public virtual ICollection<commentEvent> commentEvent { get; set; }
        public virtual ICollection<inboxMessage> inboxMessage { get; set; }
        public virtual ICollection<inboxMessage> inboxMessage1 { get; set; }
        public virtual ICollection<question> question { get; set; }*/
    }
}
