﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class Raoconnection : DbContext
    {
        public Raoconnection()
            : base("name=Raoconnection")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<AspNetRole> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUser> AspNetUsers { get; set; }
        public virtual DbSet<Event> Events { get; set; }
        public virtual DbSet<INFRACTION> INFRACTIONS { get; set; }
        public virtual DbSet<LOCATION> LOCATIONs { get; set; }
        public virtual DbSet<MATCH> MATCHes { get; set; }
        public virtual DbSet<OFFER> OFFERs { get; set; }
        public virtual DbSet<OFFERQUAL> OFFERQUALs { get; set; }
        public virtual DbSet<PLAYER> PLAYERs { get; set; }
        public virtual DbSet<QUALIFICATION> QUALIFICATIONS { get; set; }
        public virtual DbSet<REFEREE> REFEREEs { get; set; }
        public virtual DbSet<score> scores { get; set; }
        public virtual DbSet<SPORT> SPORTs { get; set; }
        public virtual DbSet<TEAM> TEAMs { get; set; }
        public virtual DbSet<TIMEOFF> TIMEOFFs { get; set; }
        public virtual DbSet<TOURNAMENT> TOURNAMENTs { get; set; }
        public virtual DbSet<TYPE> TYPEs { get; set; }
        public virtual DbSet<USERQUAL> USERQUALs { get; set; }
        public virtual DbSet<WEEKLYAVAILABILITY> WEEKLYAVAILABILITies { get; set; }
    }
}
