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
    
    public partial class TEAM
    {
        public TEAM()
        {
            this.MATCHes = new HashSet<MATCH>();
            this.MATCHes1 = new HashSet<MATCH>();
            this.PLAYERs = new HashSet<PLAYER>();
        }
    
        public int teamID { get; set; }
        public string name { get; set; }
        public int ageBracket { get; set; }
        public int grade { get; set; }
        public string managerID { get; set; }
        public Nullable<int> tournament { get; set; }
    
        public virtual ICollection<MATCH> MATCHes { get; set; }
        public virtual ICollection<MATCH> MATCHes1 { get; set; }
        public virtual ICollection<PLAYER> PLAYERs { get; set; }
        public virtual TOURNAMENT TOURNAMENT1 { get; set; }
    }
}
