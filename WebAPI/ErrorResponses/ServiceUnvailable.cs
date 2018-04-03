using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAPI.ErrorResponses
{
    public class ServiceUnvailable: ErrorResponse
    {
        public ServiceUnvailable(object uri, string message)
        {
            this.Type = "ServiceUnvailable";
            this.Title = "The service is unvailable";
            this.Detail = message;
            this.Instance = uri;
        }
    
    }
}
