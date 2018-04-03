using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Services.Services
{
    public class OperationResult<TModel>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public TModel Result { get; set; }
        public Exception Exception { get; set; }
    }
}
