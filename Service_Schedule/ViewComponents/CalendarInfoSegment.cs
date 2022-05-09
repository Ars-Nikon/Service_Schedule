using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Service_Schedule.Contexts;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Service_Schedule.Models;
using Microsoft.AspNetCore.Identity;

namespace Service_Schedule.ViewComponents
{
    public class CalendarInfoSegment : ViewComponent
    {
        private readonly ApplicationContext _db;
        private readonly UserManager<User> _userManager;
        public CalendarInfoSegment(ApplicationContext context, UserManager<User> userManager)
        {
            _userManager = userManager;
            _db = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(DateTime? chooseDate, string IdSpec, string GuidTimeTable)
        {
            TimeTableDate timetable = null;
            if (!string.IsNullOrEmpty(GuidTimeTable))
            {
                timetable = await _db.TimeTableDates.Include(x => x.Times).FirstOrDefaultAsync(x=>x.Id == Guid.Parse(GuidTimeTable));

            }
            else
            {
                timetable = await _db.TimeTableDates.Include(x => x.Times).Where(x => x.Spec_Id == IdSpec && x.Date == chooseDate).FirstOrDefaultAsync();         
            }

            if (timetable != null && timetable.Times != null)
            {
                foreach (var item in timetable.Times)
                {
                    if (item.User_Id != null)
                    {
                        item.User = await _userManager.FindByIdAsync(item.User_Id);
                    }
                }
            }

            ViewBag.Date = timetable?.Date ?? chooseDate ;
            ViewBag.GuidTimeTable = timetable?.Id;
            return View("~/Views/Admin/Components/SegmentCalendar/CalendarInfoSegment.cshtml", timetable);
        }
    }
}
