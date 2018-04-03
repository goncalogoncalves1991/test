using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApp.Models
{
    public class Event : IValidatableObject
    {
        public int communityId { get; set; }
        public string communityName { get; set; }
        public int Id { get; set; }
        
        [Required]
        public string Title { get; set; }
        
        [Required]
        [RegularExpression(@"^[\s\S]{50,}$", ErrorMessage = "Description must be at least 50 caracthers length")]
        public string Description { get; set; }
        
        [Required]
        public string Local { get; set; }
        
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        
        [Required]
        [Display(Name="Number of Tickets")]
        [Range(1, int.MaxValue, ErrorMessage = "Please enter a value bigger than {1}")]
        public int NrOfTickets { get; set; }
       
        [Required]
        [Display(Name = "Starts at")]
        public string InitDate { get; set; }
        
        [Required]
        [Display(Name = "Ends at")]
        public string EndDate { get; set; }
        
        public int[] Tags { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var end = DateTime.Parse(EndDate);
            var start = DateTime.Parse(InitDate);
            if (end < start)
            {
                yield return new ValidationResult("End date must be greater or equal than start day");
            }
        }

    }
}