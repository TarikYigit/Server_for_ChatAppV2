using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server_for_ChatApp
{
    public class Separate_Message_Dates
    {

        public string[] Separate_Date_Info(string message)
        {
            string[] final_time = [];

            string[] message_array = message.Split("-");
            string[] time_of_day = message_array[3].Split(" ");
            string[] final_day_split = time_of_day[0].Split(":");
            final_time[0] = message_array[0];   //year
            final_time[1] = message_array[1];   //month
            final_time[2] = message_array[2];   //day
            final_time[3] = final_day_split[0]; //hour
            final_time[4] = final_day_split[1]; //minute
            final_time[5] = final_day_split[2]; //second
            return final_time;
        }
    }
}
