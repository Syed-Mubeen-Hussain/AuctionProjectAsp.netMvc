//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Auction.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Review
    {
        public int r_id { get; set; }
        public string r_username { get; set; }
        public string r_email { get; set; }
        public string r_stars { get; set; }
        public string r_message { get; set; }
        public int r_status { get; set; }
    }
}