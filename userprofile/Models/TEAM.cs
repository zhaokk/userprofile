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
    using System.Runtime.Serialization;


    public partial class TEAM : ISerializable
    {
        public TEAM()
        {
            this.MATCHes = new HashSet<MATCH>();
            this.MATCHes1 = new HashSet<MATCH>();
            this.PLAYERs = new HashSet<PLAYER>();
            this.TEAMINS = new HashSet<TEAMIN>();
        }
    
        public int teamId { get; set; }
        public string name { get; set; }
        public Nullable<int> ageBracket { get; set; }
        public string grade { get; set; }
        public string sport { get; set; }
        public string managerId { get; set; }
        public string shortName { get; set; }
        public int status { get; set; }
    
        public virtual AspNetUser AspNetUser { get; set; }
        public virtual ICollection<MATCH> MATCHes { get; set; }
        public virtual ICollection<MATCH> MATCHes1 { get; set; }
        public virtual ICollection<PLAYER> PLAYERs { get; set; }
        public virtual SPORT SPORT1 { get; set; }
        public virtual ICollection<TEAMIN> TEAMINS { get; set; }

        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue("teamId", teamId);
            info.AddValue("name", name);
            if (ageBracket != null)
            {
                info.AddValue("ageBracket", ageBracket);
            }

            info.AddValue("grade", grade);
            info.AddValue("sport", sport);

            info.AddValue("managerId", managerId);
            info.AddValue("shortName", shortName);
            info.AddValue("status", status);
        }

    }
}
