//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace userprofile.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class OFFERQUAL
    {
        public int qID { get; set; }
        public int offerID { get; set; }
        public Nullable<int> qualLevel { get; set; }
    
        public virtual OFFER OFFER { get; set; }
        public virtual QUALIFICATION QUALIFICATION { get; set; }
    }
}
