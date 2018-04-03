using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAPI.ErrorResponses
{
    public class BadRequest: ErrorResponse
    {
        public BadRequest(object uri, string message)
        {
            this.Type = "BadRequest";
            this.Title = "Bad Request";
            this.Detail = message;
            this.Instance = uri;
        }
    
    }
}
