using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Models.Collections;

namespace WebAPI.Models.Singles
{
    public class EventSingleState
    {
        public int id { get; set; }
        public CommunitiesCollectionState community { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string local { get; set; }
        public int nrOfTickets { get; set; }
        public System.DateTime initDate { get; set; }
        public System.DateTime endDate { get; set; }
        public IEnumerable<CommentsCollectionState> comments { get; set; }
        public IEnumerable<SessionsCollectionState> session { get; set; }
        public IEnumerable<string> tag { get; set; }
        public IEnumerable<UsersCollectionState> subscribers { get; set; }
        public SurveySingleState survey { get; set; }
        public Link _links { get; set; }
        public class Link
        {
            public Uri self { get; set; }
            public Uri community { get; set; }
            public Uri subscribers { get; set; }
            public Uri subscriberCheckedIn { get; set; }
            public Uri sessions { get; set; }
            public Uri comments { get; set; }
            public Uri tags { get; set; }
            public Uri survey { get; set; }

        }
    }
}
