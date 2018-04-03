using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApp.Models
{
    public class Community
    {

        public int Id { get; set; }
        
        [DataType(DataType.Upload, ErrorMessage = "This uri must be a image url")]
        public HttpPostedFileBase Avatar { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [RegularExpression(@"^[\s\S]{50,}$", ErrorMessage = "Description must be at least 50 caracthers length")]
        public string Description { get; set; }

        [Required]
        public string Local { get; set; }

        public string Latitude { get; set; }
        public string Longitude { get; set; }

        [DataType(DataType.Url)]
        public string Facebook { get; set; }

        [DataType(DataType.EmailAddress)]
        [Display(Name ="Email")]
        public string Mail { get; set; }
        [DataType(DataType.Url)]
        public string GitHub { get; set;}
        [DataType(DataType.Url)]
        public string Twitter { get; set; }

        [DataType(DataType.Url)]
        [Display(Name ="WebSite")]
        public string Site { get; set; }

        [Required]
        [Display(Name="Foundation Date")]
        [DataType(DataType.Date)]
        public DateTime date{ get; set; }

    }
}