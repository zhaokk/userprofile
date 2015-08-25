
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
    using System.ComponentModel;
    
public partial class AspNetUser
{

    public AspNetUser()
    {

        this.AspNetUserClaims = new HashSet<AspNetUserClaim>();

        this.AspNetUserLogins = new HashSet<AspNetUserLogin>();

        this.INFRACTIONS = new HashSet<INFRACTION>();

        this.PLAYERs = new HashSet<PLAYER>();

        this.REFEREEs = new HashSet<REFEREE>();

        this.SCORES = new HashSet<SCORE>();

        this.TEAMs = new HashSet<TEAM>();

        this.AspNetRoles = new HashSet<AspNetRole>();

        this.Events = new HashSet<Event>();

    }


    public string Id { get; set; }
    [DisplayName("User Name")]
    public string UserName { get; set; }
    [DisplayName("Password Hash")]
    public string PasswordHash { get; set; }
    [DisplayName("Security Stamp")]
    public string SecurityStamp { get; set; }
    [DisplayName("PhotoDir")]
    public string photoDir { get; set; }
    [DisplayName("Discriminator")]
    public string Discriminator { get; set; }
    [DisplayName("First Name")]
    public string firstName { get; set; }
    [DisplayName("Last Namee")]
    public string lastName { get; set; }

    public Nullable<int> phoneNum { get; set; }

    public string email { get; set; }

    public string country { get; set; }

    public int postcode { get; set; }

    public string street { get; set; }

    public int streetNumber { get; set; }

    public string state { get; set; }

    public string dob { get; set; }



    public virtual ICollection<AspNetUserClaim> AspNetUserClaims { get; set; }

    public virtual ICollection<AspNetUserLogin> AspNetUserLogins { get; set; }

    public virtual ICollection<INFRACTION> INFRACTIONS { get; set; }

    public virtual ICollection<PLAYER> PLAYERs { get; set; }

    public virtual ICollection<REFEREE> REFEREEs { get; set; }

    public virtual ICollection<SCORE> SCORES { get; set; }

    public virtual ICollection<TEAM> TEAMs { get; set; }

    public virtual ICollection<AspNetRole> AspNetRoles { get; set; }

    public virtual ICollection<Event> Events { get; set; }

}

}
