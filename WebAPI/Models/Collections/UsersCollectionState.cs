using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAPI.Models.Collections
{
    public class UsersCollectionState
    {
        public string id { get; set; }
        public string local { get; set; }
        public string name { get; set; }
        public string lastName { get; set; }
        public string email { get; set; }
        public string picture { get; set; }
        public DateTime registerDate { get; set; }
        public LinkCollection _Links { get; set; }
        
    }
}
