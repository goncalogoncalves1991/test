using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAPI.Models.Collections
{
    public class EventsCollectionState
    {
        public int id { get; set; }
        public string title { get; set; }
        public string community { get; set; }
        public string description { get; set; }
        public string local { get; set; }
        public int nrOfTickets { get; set; }
        public System.DateTime initDate { get; set; }
        public System.DateTime endDate { get; set; }
        //public IList<object> Links { get; set; } 
        public LinkCollection _Links { get; set; }
        
    }
}
