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
    
    public partial class Product_View
    {
        public int av_id { get; set; }
        public Nullable<int> av_fk_product_id { get; set; }
        public Nullable<decimal> av_veiws_count { get; set; }
    
        public virtual Product Product { get; set; }
    }
}