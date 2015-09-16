using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace userprofile.Models
{
    public class WEEKLYAVAILABILITYViewModel
    {
        public WEEKLYAVAILABILITYViewModel()
        {
            //   this.moment = new Boolean[7][] { new Boolean[] { false, false, false }, new Boolean[] { false, false, false }, new Boolean[] { false, false, false }, new Boolean[] { false, false, false }, new Boolean[] { false, false, false }, new Boolean[] { false, false, false }, new Boolean[] { false, false, false } };
        }
        public WEEKLYAVAILABILITYViewModel(WEEKLYAVAILABILITY availa)
        {

            this.moment = new Boolean[7][] { new Boolean[] { false, false, false, false }, new Boolean[] { false, false, false, false }, new Boolean[] { false, false, false, false }, new Boolean[] { false, false, false, false }, new Boolean[] { false, false, false, false }, new Boolean[] { false, false, false, false }, new Boolean[] { false, false, false, false } };
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
                //switch (dayDb)
                //{
                //    case 0:
                //        moment[day][0] = false;
                //        moment[day][1] = false;
                //        moment[day][2] = false;
                //        break;
                //    case 1:
                //        moment[day][0] = true;
                //        moment[day][1] = false;
                //        moment[day][2] = false;
                //        break;
                //    case 2:
                //        moment[day][0] = false;
                //        moment[day][1] = true;
                //        moment[day][2] = false;
                //        break;
                //    case 3:
                //        moment[day][0] = true;
                //        moment[day][1] = true;
                //        moment[day][2] = false;
                //        break;
                //    case 4:
                //        moment[day][0] = false;
                //        moment[day][1] = false;
                //        moment[day][2] = true;
                //        break;
                //    case 5:
                //        moment[day][0] = true;
                //        moment[day][1] = false;
                //        moment[day][2] = true;
                //        break;
                //    case 6:
                //        moment[day][0] = false;
                //        moment[day][1] = true;
                //        moment[day][2] = true;
                //        break;
                //    case 7:
                //        moment[day][0] = true;
                //        moment[day][1] = true;
                //        moment[day][2] = true;
                //        break;


                //    default:
                //        break;

                //}


                moment = resolve(day,dayDb, moment);
            }


        }
        public Boolean[][] resolve(int day,int dayDb, Boolean[][] moment)
        {
            if (dayDb >= 8)
            {
                moment[day][0] = true;
                dayDb -= 8;
                return digit2(day,dayDb, moment);

            }
            else
            {
                moment[day][0] = false;
                return digit2(day,dayDb, moment);
            }

        }
        public Boolean[][] digit2(int day,int dayDb, Boolean[][] moment)
        {
            if (dayDb >= 4)
            {
                moment[day][1] = true;
                dayDb -= 4;
                return digit1(day,dayDb, moment);
            }
            else
            {
                moment[day][1] = false;
                return digit1(day,dayDb, moment);
            }

        }
        public Boolean[][] digit1(int day,int dayDb, Boolean[][] moment)
        {
            if (dayDb >= 2)
            {
                moment[day][2] = true;
                dayDb -= 2;
                return digit0(day,dayDb, moment);
            }
            else
            {
                moment[day][2] = false;
                return digit0(day,dayDb, moment);
            }

        }
        public Boolean[][] digit0(int day,int dayDb, Boolean[][] moment)
        {
            if (dayDb >= 1)
            {
                moment[day][3] = true;
                dayDb -= 1;
                return moment;
            }
            else
            {
                moment[day][3] = false;
                return moment;
            }


        }
        public Boolean[][] moment { get; set; }
        public WEEKLYAVAILABILITY getWeekADb()
        {
            var waDB = new WEEKLYAVAILABILITY();
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
        public int setDb(Boolean[][] moment, int i)
        {
            var result = 0;
            if (moment[i][3] == true)
            {
                result += 1;

            }
            if (moment[i][2] == true)
            {
                result += 2;
            }
            if (moment[i][1] == true)
            {
                result += 4;
            }
            if (moment[i][0] == true)
            {
                result += 8;
            }
            return result;
        }


    }
}