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
    
    public partial class USERQUAL
    {
        public int qualificationId { get; set; }
        public int refId { get; set; }
        public Nullable<int> qualLevel { get; set; }
    
        public virtual QUALIFICATION QUALIFICATION { get; set; }
        public virtual REFEREE REFEREE { get; set; }
    }
}
