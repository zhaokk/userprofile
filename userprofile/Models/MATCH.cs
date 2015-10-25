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
    using System.ComponentModel.DataAnnotations;
    using System.Runtime.Serialization;

    [Serializable]
    public partial class MATCH : ISerializable
    {
        public MATCH()
        {
            this.INFRACTIONS = new HashSet<INFRACTION>();
            this.OFFERs = new HashSet<OFFER>();
        }

        [Display(Name = "Match Id")]
        public int matchId { get; set; }
        [Display(Name = "Match Date")]
        [Required(ErrorMessage = "Match Date required")]
        public System.DateTime matchDate { get; set; }
        [Display(Name = "Location Id")]
        [Required(ErrorMessage = "Location is required")]
        public Nullable<int> locationId { get; set; }
        [Display(Name = "Home Team")]
        [Required(ErrorMessage = "Home Team Name is required")]
        public int teamAId { get; set; }
        [Display(Name = "Away Team")]
        [Required(ErrorMessage = "Away Team Name is required")]
        public int teamBId { get; set; }
        [Display(Name = "Home Team Score")]
        public Nullable<int> teamAScore { get; set; }
        [Display(Name = "Away Team Score")]
        public Nullable<int> teamBScore { get; set; }
        [Display(Name = "Status")]
        public Nullable<int> status { get; set; }
        [Display(Name = "Tournament Id")]
        public int tournamentId { get; set; }
        [Display(Name = "Match Duration")]
        [Required(ErrorMessage = "Match Duration is required")]
        public int matchLength { get; set; }
        [Display(Name = "Half Time Duration")]
        [Required(ErrorMessage = "Half Time Duration is required")]
        public int halfTimeDuration { get; set; }
        [Display(Name = "Match counts towards point system")]
        public bool countsToDraw { get; set; }
        public String tournamentName { get; set; }
        public String locationName { get; set; }
        public String teamA { get; set; }
        public String teamB { get; set; }

        public String lat { get; set; }
        public String lon { get; set; }

        public virtual ICollection<INFRACTION> INFRACTIONS { get; set; }
        public virtual LOCATION LOCATION { get; set; }
        public virtual TEAM TEAM { get; set; }
        public virtual TEAM TEAM1 { get; set; }
        public virtual TOURNAMENT TOURNAMENT { get; set; }
        public virtual ICollection<OFFER> OFFERs { get; set; }

        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue("MatchId", matchId);
            info.AddValue("matchDate", matchDate);
            if (locationId != null)
            {
                info.AddValue("locationId", locationName);
            }

            info.AddValue("teamA", teamA);
            info.AddValue("teamB", teamB);

            info.AddValue("teamAId", teamAId);
            info.AddValue("teamBId", teamBId);

            info.AddValue("teamAScore", teamAScore);
            info.AddValue("teamBScore", teamBScore);
            info.AddValue("status", status);
            info.AddValue("tournamentId", tournamentId);
            info.AddValue("matchLength", matchLength);
            info.AddValue("halfTimeDuration", halfTimeDuration);
            info.AddValue("countsToDraw", countsToDraw);
            info.AddValue("tournamentName", tournamentName);
            info.AddValue("lat", lat);
            info.AddValue("lon", lon);
        }

    }
}
