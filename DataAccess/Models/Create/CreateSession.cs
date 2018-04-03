using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models.Create
{
    public class CreateSession
    {
        public int id { get; set; }
        public string userId { get; set; }
        public int eventId { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string speakerName { get; set; }
        public string lastName { get; set; }
        public string linkOfSpeaker { get; set; }
        public DateTime initialDate { get; set; }
        public DateTime endDate { get; set; }

        public bool ParameterValid()
        {
            return eventId != 0 && userId != null && title != null && initialDate != null && endDate!=null && speakerName != null && linkOfSpeaker != null && lastName!=null;
        }
    }
}
