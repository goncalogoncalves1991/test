using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models.Create
{
    public class CoordinatesRange
    {
        public double MaxLongitude { get; set; } // In Degrees
        public double MaxLatitude { get; set; } // In Degrees 
        public double MinLongitude { get; set; } // In Degrees
        public double MinLatitude { get; set; } // In Degrees
    }
}
