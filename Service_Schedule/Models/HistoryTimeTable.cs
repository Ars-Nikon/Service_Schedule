using System;
using static Service_Schedule.Models.TimeTableTimeVisit;

namespace Service_Schedule.Models
{
    public class HistoryTimeTable
    {
        public DateTime Date { get; set; }

        public string Spec_Name { get; set; }

        public string Spec_Type { get; set; }

        public string Id_Spec { get; set; }
        public Guid Id_TimeTableTime { get; set; }

        public TimeSpan Visit_Start { get; set; }

        public TimeSpan Visit_End { get; set; }

        public Visit_Status Status { get; set; }
    }
}
