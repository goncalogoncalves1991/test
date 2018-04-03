using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAPI.Models.Collections
{
    public class CommentsCollectionState
    {
        public int id { get; set; }
        public string message { get; set; }
        public DateTime date { get; set; }
        // public UsersCollectionState user { get; set; }

        public string userName { get; set; }

        public string userId { get; set; }

        public LinkCollection _links { get; set; }
    }
}
