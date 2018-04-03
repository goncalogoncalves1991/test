using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApp.Models
{
    public class News
    {
        public int CommunityId { get; set; }
        public string CommunityName { get; set; }
        [Required]
        public string Title { get; set; }
       
        [Required]
        [RegularExpression(@"^[\s\S]{50,}$", ErrorMessage = "The content must be at least 50 caracthers length")]
        public string Description { get; set; }
    }
}