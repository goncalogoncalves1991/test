using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models.Create
{
    public class CreateNotice
    {
        public int id { get; set; }
        public int communityId { get; set; }
        public string userId { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public System.DateTime initialDate { get; set; }

        public bool ParameterValid()
        {
            return communityId != 0 && title != null && initialDate != null && userId!=null;
        }
    }
}
