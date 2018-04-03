using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Models
{
    public class LocationCoordinates
    {
        public double latitude;
        public double longitude;
        public double radius;

        public bool isEmpty()
        {
            return latitude == 0 && longitude == 0 && radius == 0;
        }
    }
}
