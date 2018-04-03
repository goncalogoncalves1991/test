using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.ErrorResponses
{    
     public class Conflict: ErrorResponse
    {
        public Conflict(object uri, string message)
        {
            this.Type = "Conflict";
            this.Title = "There is a Conflit";
            this.Detail = message;
            this.Instance = uri;
        }
    
    }
}
