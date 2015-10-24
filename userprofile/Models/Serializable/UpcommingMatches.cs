using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Runtime.Serialization;

namespace userprofile.Models.Serializable
{
    public class UpcommingMatches : ISerializable
    {
        public List<MATCH> myUpcommingReferee;
        public List<MATCH> myUpcommingPlayer;
        public List<MATCH> myUpcomming;
        public Boolean pendingRefereeOffers;

        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            if (myUpcomming != null)
            {
                info.AddValue("upcommingAll", myUpcomming);
            }
            if (myUpcommingPlayer != null)
            {
                info.AddValue("myUpcommingPlayer", myUpcommingPlayer);
            }
            if (myUpcommingReferee != null)
            {
                info.AddValue("myUpcommingReferee", myUpcommingReferee);
            }
            if (pendingRefereeOffers != null)
            {
                info.AddValue("pendingRefereeOffers", pendingRefereeOffers);
            }

        }
    }
}