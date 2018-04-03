using System;
using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
    public class Session
    {
        public string UserId { get; set; }
        public int EventId { get; set; }

        [Required]
        public string Title { get; set; }
        
        [Required]
        [RegularExpression(@"^[\s\S]{50,}$", ErrorMessage = "Description must be at least 50 caracthers length")]
        public string Description { get; set; }

        [Required]
        [Display(Name ="Starts at")]
        public string StartDate { get; set; }

        [Required]
        [Display(Name = "Ends at")]
        public string EndDate { get; set; }


        [Required]
        [Display(Name = "First Name")]
        public string SpeakerName { get; set; }
        
        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Profile")]
        public string LinkOfSpeaker { get; set; }
    }
}