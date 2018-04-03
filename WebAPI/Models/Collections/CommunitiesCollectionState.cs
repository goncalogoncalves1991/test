using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAPI.Models.Collections
{
    public class CommunitiesCollectionState
    {
        public int id { get; set; }
        public string name { get; set; }
        public string local { get; set; }
        public string description { get; set; }
        public DateTime foundationDate { get; set; }
        public string avatar { get; set; }
        //public IList<object> Links { get; set; }
        public LinkCollection _Links { get; set; }
    }
}
