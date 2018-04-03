using DataAccess.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models.Create
{
    public class CreateQuestion
    {
        public int id { get; set; }
        public string message { get; set; }
        public string authorId { get; set; }
        public int sessionId { get; set; }
        public string liked_user { get; set; }

        public bool ParameterValid()
        {
            return message != null && authorId != null && sessionId != 0;
        }
    }
}
