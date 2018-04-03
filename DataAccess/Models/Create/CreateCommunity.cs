using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models.Create
{
    public class CreateCommunity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Local { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Description { get; set; }
        public DateTime FoundationDate { get; set; }
        public int[] Tags { get; set; }
        public Stream Avatar { get; set; }
        public string UserId { get; set; }//tanto pode ser user que está a criar ou que está a fazer update
        public string GitHub { get; set; }
        public string Site { get; set; }
        public string Mail { get; set; }
        public string AvatarLink { get; set; }
        public int[] Sponsors { get; set; }

        public bool VerifyProperties()
        {
            return Sponsors != null && Name != null && Local != null && Latitude!=null && Longitude!=null && Description != null && FoundationDate != null && Avatar != null && Tags != null && UserId != null;
        }

    }
}
