//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DataAccess.Models.DTOs
{
    using System;
    using System.Collections.Generic;
    
    public partial class surveyTextAnswer
    {
        public int eventId { get; set; }
        public int questionId { get; set; }
        public string authorId { get; set; }
        public string response { get; set; }
    
        public virtual surveyAnswer surveyAnswer { get; set; }
    }
}
