using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models.Create
{
    public class CreateCommunitySocial
    {
        public string adminId { get; set; }
        public int communityId { get; set; }
        public string social { get; set; }
        public string url { get; set; }
        public string token { get; set; }
        
        public bool VerifyProperties()
        {
            return adminId != null && (social == "Facebook" || social =="Twitter") && url != null && token != null && communityId!=0;
        }

    }
}
