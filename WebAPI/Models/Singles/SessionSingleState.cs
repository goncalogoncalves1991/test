using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Models.Collections;

namespace WebAPI.Models.Singles
{
    public class SessionSingleState
    {
        public int id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public System.DateTime initialDate { get; set; }
        public System.DateTime endDate { get; set; }
        public string speaker { get; set; }
        public string profileSpeaker { get; set; }
        public EventsCollectionState _event { get; set; }
        public IEnumerable<QuestionsCollectionState> questions { get; set; }
        public Link _links { get; set; }

        public class Link
        {
            public Uri self{get;set;}
            public Uri _event{get;set;}
            public Uri questions { get; set; }

        }
    }
}
