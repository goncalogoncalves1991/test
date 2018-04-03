using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models.Create
{
    public class CreateUser
    {
        public string id { get; set; }
        public string local { get; set; }
        public string name { get; set; }
        public string lastName { get; set; }
        public string email { get; set; }
        public System.DateTime registerDate { get; set; }
        public string picture { get; set; }
        public string facebook { get; set; }
        public string googleplus { get; set; }
        public Stream profilePicture { get; set; }

        public bool ParameterValid()
        {
            return id!=null && name != null && email != null && registerDate!=null;
        }
    }
}
