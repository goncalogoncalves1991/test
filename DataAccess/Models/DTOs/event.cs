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
    
    public partial class @event
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public @event()
        {
            this.eventSubscribers = new HashSet<eventSubscribers>();
            this.session = new HashSet<session>();
            this.tag = new HashSet<tag>();
            this.comment = new HashSet<comment>();
            this.survey = new HashSet<surveyQuestion>();
        }
    
        public int id { get; set; }
        public Nullable<int> communityId { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string local { get; set; }
        public Nullable<int> nrOfTickets { get; set; }
        public System.DateTime initDate { get; set; }
        public System.DateTime endDate { get; set; }
        public string longitude { get; set; }
        public string latitude { get; set; }
        public string qrcode { get; set; }
        public Nullable<System.DateTime> creationDate { get; set; }
    
        public virtual community community { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<eventSubscribers> eventSubscribers { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<session> session { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tag> tag { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<comment> comment { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<surveyQuestion> survey { get; set; }
    }
}
