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
    
    public partial class CommunitySocialNetwork
    {
        public int CommunityId { get; set; }
        public string provider { get; set; }
        public string Url { get; set; }
        public string Acess_Token { get; set; }
    
        public virtual community community { get; set; }
    }
}