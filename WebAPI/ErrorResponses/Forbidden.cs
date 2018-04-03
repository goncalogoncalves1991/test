using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebAPI.ErrorResponses
{
    public class Forbidden : ErrorResponse
    {
        public Forbidden(object uri, string message)
        {
            this.Type = "Forbidden";
            this.Title = "User Forbidden";
            this.Detail = message;
            this.Instance = uri;
        }

    }
}
