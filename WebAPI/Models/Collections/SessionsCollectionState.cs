using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAPI.Models.Collections
{
    public class SessionsCollectionState 
    {
        public int id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public System.DateTime initialDate { get; set; }
        public System.DateTime endDate { get; set; }
        public string speaker { get; set; }
        public string profileSpeaker { get; set; }
        public LinkCollection _links { get; set; }
    }
}
