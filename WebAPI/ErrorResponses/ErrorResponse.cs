using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace WebAPI.ErrorResponses
{
    public class ErrorResponse 
    {
        public string Type { get; set; }
        public string Title { get; set; }
        public string Detail { get; set; }
        public object Instance { get; set; }

        
    }
}
