using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Service_Schedule.Contexts;
using Service_Schedule.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Service_Schedule.ViewComponents
{
    public class CalendarSegment : ViewComponent
    {
        private readonly ApplicationContext _db;
        public CalendarSegment(ApplicationContext context)
        {
            _db = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(string Id, int Mounth)
        {
            var date = DateTime.UtcNow.AddHours(3);
            if (string.IsNullOrEmpty(Id))
            {
                Id = "";
            }
            if (Mounth < date.Month - 1)
            {
                Mounth = date.Month;
            }
            date = date.AddMonths(-date.Month + Mounth);

            var freeDate = await _db.TimeTableDates.Where(x => x.Spec_Id == Id && x.Times.Any(y => y.User_Id == null)).ToListAsync();
     
            List<DayInfoForCalendar> dayInfos = new List<DayInfoForCalendar>();

            var countDay = DateTime.DaysInMonth(date.Year, date.Month);

            date = date.AddDays(-date.Day + 1);
            var firstDayOfTheWeekInMonth = date.DayOfWeek;

            var DayOfWeeks = new Dictionary<DayOfWeek, int>()
            {
                {DayOfWeek.Monday,1},
                {DayOfWeek.Tuesday,2},
                {DayOfWeek.Wednesday,3},
                {DayOfWeek.Thursday,4},
                {DayOfWeek.Friday,5},
                {DayOfWeek.Saturday,6},
                {DayOfWeek.Sunday,7},
            };

            //Пустые Значения в начале
            for (int i = 1; i < DayOfWeeks.GetValueOrDefault(firstDayOfTheWeekInMonth); i++)
            {
                dayInfos.Add(new DayInfoForCalendar { Day = 0, Type = 0});
            }

            for (int i = 1; i <= countDay; i++)
            {
                var freeday = freeDate.FirstOrDefault(x => x.Date.Day == i);

                if (freeday != null)
                {
                    dayInfos.Add(new DayInfoForCalendar { Day = i, Type = 1, Id =  freeday.Id.ToString()});
                }
                else
                {
                    dayInfos.Add(new DayInfoForCalendar { Day = i, Type = 0 });
                }
            }

            date = date.AddDays(countDay - 1);

            var endDayOfTheWeekInMonth = date.DayOfWeek;

            //Пустые Значения в конце
            for (int i = DayOfWeeks.GetValueOrDefault(endDayOfTheWeekInMonth); i < 7; i++)
            {
                dayInfos.Add(new DayInfoForCalendar { Day = 0, Type = 0});
            }

            var ss = dayInfos.Skip(0).Take(7).ToList();

            return View("~/Views/Admin/Components/SegmentCalendar/CalendarSegment.cshtml", dayInfos);
        }
    }
}
