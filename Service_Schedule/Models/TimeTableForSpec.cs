using System;
using static Service_Schedule.Models.TimeTableTimeVisit;

namespace Service_Schedule.Models
{
    public class TimeTableForSpec
    {
        public DateTime Date { get; set; }

        public string Id_User { get; set; }
        public string UserName { get; set; }

        public TimeSpan Visit_Start { get; set; }

        public TimeSpan Visit_End { get; set; }

        public Visit_Status Status { get; set; }
    }
}
