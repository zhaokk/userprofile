using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace userprofile.Models
{
    public class WEEKLYAVAILABILITYViewModel
    {
        public WEEKLYAVAILABILITYViewModel() {
            this.moment = new Boolean[7][] { new Boolean[] { false, false, false }, new Boolean[] { false, false, false }, new Boolean[] { false, false, false }, new Boolean[] { false, false, false }, new Boolean[] { false, false, false }, new Boolean[] { false, false, false }, new Boolean[] { false, false, false } };
        }
        public WEEKLYAVAILABILITYViewModel(WEEKLYAVAILABILITY availa)
        {

            this.moment = new Boolean[7][] { new Boolean[] { false, false, false }, new Boolean[] { false, false, false }, new Boolean[] { false, false, false }, new Boolean[] { false, false, false }, new Boolean[] { false, false, false }, new Boolean[] { false, false, false }, new Boolean[] { false, false, false } };
            int day = 0;
            int dayDb = 0;
            for (day = 0; day < 7; day++)
            {
                switch (day)
                {
                    case 0:
                        dayDb = availa.monday;
                        break;
                    case 1:
                        dayDb = availa.tuesday;
                        break;
                    case 2:
                        dayDb = availa.wednesday;
                        break;
                    case 3:
                        dayDb = availa.thursday;
                        break;
                    case 4:
                        dayDb = availa.friday;
                        break;
                    case 5:
                        dayDb = availa.saturday;
                        break;
                    case 6:
                        dayDb = availa.sunday;
                        break;
                    default:
                        break;

                }
                switch (dayDb)
                {
                    case 0:
                        moment[day][0] = false;
                        moment[day][1] = false;
                        moment[day][2] = false;
                        break;
                    case 1:
                        moment[day][0] = true;
                        moment[day][1] = false;
                        moment[day][2] = false;
                        break;
                    case 2:
                        moment[day][0] = false;
                        moment[day][1] = true;
                        moment[day][2] = false;
                        break;
                    case 3:
                        moment[day][0] = true;
                        moment[day][1] = true;
                        moment[day][2] = false;
                        break;
                    case 4:
                        moment[day][0] = false;
                        moment[day][1] = false;
                        moment[day][2] = true;
                        break;
                    case 5:
                        moment[day][0] = true;
                        moment[day][1] = false;
                        moment[day][2] = true;
                        break;
                    case 6:
                        moment[day][0] = false;
                        moment[day][1] = true;
                        moment[day][2] = true;
                        break;
                    case 7:
                        moment[day][0] = true;
                        moment[day][1] = true;
                        moment[day][2] = true;
                        break;


                    default:
                        break;

                }

            }


        }
        public Boolean[][] moment { get; set; }
        public WEEKLYAVAILABILITY getWeekADb(){
            var waDB=new WEEKLYAVAILABILITY();
            for (int i = 0; i < 7; i++)
            {
                switch (i)
                {
                    case 0:
                        waDB.monday = setDb(moment, i);
                        break;
                    case 1:
                        waDB.tuesday = setDb(moment, i);
                        break;
                    case 2:
                        waDB.wednesday = setDb(moment, i);
                        break;
                    case 3:
                        waDB.thursday = setDb(moment, i);
                        break;
                    case 4:
                        waDB.friday = setDb(moment, i);
                        break;
                    case 5:
                        waDB.saturday = setDb(moment, i);
                        break;
                    case 6:
                        waDB.sunday = setDb(moment, i);
                        break;
                    default:
                        break;

                }
        

            }
            return waDB;
        
        }
        public int setDb(Boolean[][] moment,int i){
            if (moment[i][0] == false)
            {
                if (moment[i][1] == false)
                {
                    if (moment[i][2] == false)
                    {
                        return 0;
                    }
                    else
                    {
                        return 4;
                    }

                }
                else
                {
                    if (moment[i][2] == false)
                    {
                        return 2;
                    }
                    else
                    {
                        return 6;
                    }

                }
            }
            else
            {
                if (moment[i][1] == false)
                {
                    if (moment[i][2] == false)
                    {
                        return 1;
                    }
                    else
                    {
                        return 5;
                    }

                }
                else
                {
                    if (moment[i][2] == false)
                    {
                        return 3;
                    }
                    else
                    {
                        return 7;
                    }

                }


            }
        
        }

    }
}