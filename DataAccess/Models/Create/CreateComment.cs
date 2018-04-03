using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models.Create
{
    public class CreateComment
    {
        public int id { get; set; }
        public int communityId { get; set; }
        public int eventId { get; set; }
        public string message { get; set; }
        public System.DateTime initialDate { get; set; }
        public string authorId { get; set; }

        public bool ParameterValid()
        {
            return ((eventId != 0 && communityId == 0) || (communityId != 0 && eventId == 0)) && message != null && authorId != null;
        }
    }
}
