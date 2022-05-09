using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Service_Schedule.Contexts;
using Service_Schedule.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Service_Schedule.ViewComponents
{
    public class RecordInfoTimeTable : ViewComponent
    {
        private readonly ApplicationContext _db;
        public RecordInfoTimeTable(ApplicationContext context)
        {
            _db = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(string GuidTimeTable)
        {
            TimeTableDate timetable = null;
            var datenow = DateTime.UtcNow.AddHours(3);
            timetable = await _db.TimeTableDates.Include(x => x.Times.Where(x => x.Status == TimeTableTimeVisit.Visit_Status.Free)).FirstOrDefaultAsync(x => x.Id == Guid.Parse(GuidTimeTable));
            ViewBag.GuidTimeTable = timetable?.Id;
            return View("~/Views/Home/Components/RecordInfoTimeTable.cshtml", timetable);
        }
    }
}
