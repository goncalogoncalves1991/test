using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAPI.ErrorResponses
{
    public class NotFound : ErrorResponse
    {
        public NotFound(object uri, string message)
        {
            this.Type = "NotFound";
            this.Title = "Page not Found";
            this.Detail = message;
            this.Instance = uri;
        }
    
    }
}
