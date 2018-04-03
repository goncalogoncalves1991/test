using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models.Create
{
    public class CreateEvent
    {
        public int Id { get; set; }
        public int communityId { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string local { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public int nrOfTickets { get; set; }
        public System.DateTime initDate { get; set; }
        public System.DateTime endDate { get; set; }
        public string UserId { get; set; }
        public string qrcode { get; set; }
        public int[] Tags { get; set; }

        public bool VerifyProperties(){
            return communityId != 0 && title != null && description != null && local != null && nrOfTickets != 0 && initDate != null && endDate != null && Tags != null && UserId!=null;
        }

        public bool ParameterValid()
        {
            return communityId != 0 && title != null && initDate != null && endDate != null && UserId != null;
        }
    }
    
}
