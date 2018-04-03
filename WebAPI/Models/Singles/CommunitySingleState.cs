using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Models.Collections;

namespace WebAPI.Models.Singles
{
    public class CommunitySingleState : ISerializable
    {
        public int id { get; set; }
        public string name { get; set; }
        public string local { get; set; }
        public string description { get; set; }
        public DateTime foundationDate { get; set; }
        public string avatar { get; set; }
        public IEnumerable<CommentsCollectionState> comments { get; set; }
        public IEnumerable<EventsCollectionState> events { get; set; }
        public IEnumerable<NoticesCollectionState> notices { get; set; }
        public IEnumerable<UsersCollectionState> admins { get; set; }
        public IEnumerable<UsersCollectionState> members { get; set; }
        
        public IEnumerable<string> tags { get; set; }
        
        public Link _Links { get; set; }
        public class Link
        {
            public Uri Self { get; set; }
            public Uri Events { get; set; }
            public Uri PastEvents { get; set; }
            public Uri FutureEvents { get; set; }
            public Uri Members { get; set; }
            public Uri Admins { get; set; }
            public Uri Notices { get; set; }
            public Uri Comments { get; set; }
            
        }


        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException("info");

            info.AddValue("id", id);
            info.AddValue("name", name);
            info.AddValue("local", local);
            info.AddValue("description", description);
            info.AddValue("foundationDate", foundationDate);
            info.AddValue("avatar", avatar);
            info.AddValue("comments", comments);
            info.AddValue("events", events);
            info.AddValue("notices", notices);
            info.AddValue("admins", admins);
            info.AddValue("members", members);
            info.AddValue("tags", tags);
            info.AddValue("_links", _Links);
        }
    }
}
