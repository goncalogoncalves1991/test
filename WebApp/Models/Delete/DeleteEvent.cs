using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApp.Models.Delete
{
    public class DeleteEvent
    {
        [Required]
        public string Name { get; set; }

        public int Id { get; set; }
    }
}