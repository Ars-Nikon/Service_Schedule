using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Service_Schedule.Contexts;
using Service_Schedule.Models;
using Service_Schedule.Utilits;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Service_Schedule.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationContext _db;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;

        public HomeController(ILogger<HomeController> logger, UserManager<User> userManager, IConfiguration configuration, ApplicationContext context, SignInManager<User> signInManager)
        {
            _db = context;
            _configuration = configuration;
            _userManager = userManager;
            _logger = logger;
            _signInManager = signInManager;
        }

        public IActionResult Index()
        {
            ViewBag.Text = _configuration.GetSection("TextInIndexPage").Value;
            return View();
        }


        [HttpGet]
        [Authorize(Roles = "admin,spec")]
        public async Task<ActionResult> TimeTableRecord(string Id)
        {
            var spec = await _userManager.FindByNameAsync(User.Identity.Name);
            if (spec == null)
            {
                await _signInManager.SignOutAsync();
                return RedirectToAction("Index", "Home");
            }
            var dateNow = DateTime.UtcNow.AddHours(+3);
            var history = await _db.TimeTableDates.Include(x => x.Times).Where(x => x.Spec_Id == spec.Id && x.Date >= dateNow.Date && x.Date <= dateNow.Date.AddDays(7)).ToListAsync();
            var result = new List<TimeTableForSpec>();
            foreach (var day in history)
            {
                foreach (var time in day.Times)
                {
                    var timetable = new TimeTableForSpec();
                    timetable.Visit_Start = time.Visit_Start;
                    timetable.Visit_End = time.Visit_End;
                    timetable.Status = time.Status;
                    timetable.Date = day.Date.Date;
                    if (time.User_Id != null)
                    {
                        var user = await _db.Users.Where(x => x.Id == time.User_Id).Select(x => x.Name).FirstOrDefaultAsync();
                        if (user != null)
                        {
                            timetable.Id_User = time.User_Id;
                            timetable.UserName = user;
                        }
                    }
                    result.Add(timetable);
                }
            }

            return View(result);
        }


        [HttpGet]
        public async Task<ActionResult> SpecInfo(string Id)
        {
            if (string.IsNullOrEmpty(Id))
            {
                return NotFound();
            }
            var spec = await _db.Specialists.Include(x => x.User).FirstOrDefaultAsync(x => x.Id == Id);
            if (spec == null)
            {
                return NotFound();
            }
            return View(spec);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CancelRecord(string Id)
        {

            if (string.IsNullOrEmpty(Id))
            {
                return Json(new { Status = "Error", Message = "Id отсутвтыет" });
            }
            var timeTable = await _db.TimeTableTimeVisits.FirstOrDefaultAsync(x => x.Id == Guid.Parse(Id));
            if (timeTable == null)
            {
                return Json(new { Status = "Error", Message = "Данные отсутвтуют" });
            }
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user == null)
            {
                await _signInManager.SignOutAsync();
                return Json(new { Status = "Error", Message = "Данные отсутвтуют" });
            }
            if (string.IsNullOrEmpty(timeTable.User_Id) || timeTable.Status != TimeTableTimeVisit.Visit_Status.Taken)
            {
                return Json(new { Status = "Error", Message = "Данные отсутвтуют" });
            }
            if (user.Id != timeTable.User_Id)
            {
                return Json(new { Status = "Error", Message = "Данные отсутвтуют" });
            }
            timeTable.Status = TimeTableTimeVisit.Visit_Status.Free;
            timeTable.User_Id = null;
            await _db.SaveChangesAsync();
            return Json(new { Status = "Ok", Message = "Ok" });
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> MyRecord()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user == null)
            {
                await _signInManager.SignOutAsync();
                return RedirectToAction("Index", "Home");
            }

            var history = await _db.TimeTableDates.Include(x => x.Times.Where(y => y.User_Id == user.Id)).ToListAsync();
            var result = new List<HistoryTimeTable>();
            foreach (var day in history)
            {
                foreach (var time in day.Times)
                {
                    var spec = await _db.Specialists.Include(x => x.User).FirstOrDefaultAsync(x => x.Id == day.Spec_Id);
                    var historyTimeTable = new HistoryTimeTable();
                    historyTimeTable.Visit_Start = time.Visit_Start;
                    historyTimeTable.Visit_End = time.Visit_End;
                    historyTimeTable.Status = time.Status;
                    historyTimeTable.Date = day.Date.Date;
                    historyTimeTable.Id_TimeTableTime = time.Id;
                    historyTimeTable.Spec_Name = spec?.User?.Name;
                    historyTimeTable.Id_Spec = spec?.Id;
                    historyTimeTable.Spec_Type = spec?.TypeSpec;
                    result.Add(historyTimeTable);
                }
            }

            return View(result);
        }


        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Record(string Id)
        {
            if (string.IsNullOrEmpty(Id))
            {
                return NotFound();
            }
            var date = DateTime.UtcNow.AddHours(3);

            var spec = await _db.Specialists.Include(x => x.User)
                .Select(x => new { x.Id, x.Surname, x.Patronymic, x.User.Name, x.Price, x.TypeSpec })
                .FirstOrDefaultAsync(x => x.Id == Id);

            if (spec == null)
            {
                return NotFound();
            }

            var specFio = new SpecsFio { FIO = $"{spec.Surname} {spec.Name} {spec.Patronymic}", Id = spec.Id, Price = spec.Price, Type = spec.TypeSpec };

            ViewBag.Months = new SelectList(Utilit.Mounths.Skip(date.Month - 1), "Value", "Key");

            return View(specFio);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Recording(string Id)
        {
            if (string.IsNullOrEmpty(Id))
            {
                return Json(new { Status = "Error", Message = "Id отсутвтыет" });
            }
            var dateNow = DateTime.UtcNow.AddHours(+3);
            var timetabletime = await _db.TimeTableTimeVisits.Include(x => x.TimeTableDate).FirstOrDefaultAsync(x => x.Id == Guid.Parse(Id));

            if (timetabletime == null)
            {
                return Json(new { Status = "Error", Message = "Нет данных" });
            }

            if (timetabletime.TimeTableDate.Date < dateNow.Date)
            {
                return Json(new { Status = "Error", Message = "Дата приема уже устарела" });
            }

            if (timetabletime.Visit_Start < dateNow.AddMinutes(+30).TimeOfDay && timetabletime.TimeTableDate.Date == dateNow.Date)
            {
                return Json(new { Status = "Error", Message = "Дата приема уже устарела" });
            }

            if (timetabletime.User_Id != null || timetabletime.Status != TimeTableTimeVisit.Visit_Status.Free)
            {
                return Json(new { Status = "Error", Message = "Дата приема уже занята" });
            }
            var user = await _userManager.FindByNameAsync(User?.Identity?.Name);
            if (user == null)
            {
                await _signInManager.SignOutAsync();
                return Json(new { Status = "Error", Message = "User Error" });
            }

            var recordCount = _db.TimeTableTimeVisits.Count(x => x.User_Id == user.Id && x.TimeTableDate.Spec_Id == timetabletime.TimeTableDate.Spec_Id && x.Status == TimeTableTimeVisit.Visit_Status.Taken);

            if (recordCount > 0)
            {
                return Json(new { Status = "Error", Message = "На одного специалиста можно записаться один раз" });
            }

            timetabletime.Status = TimeTableTimeVisit.Visit_Status.Taken;
            timetabletime.User_Id = user.Id;
            _db.Update(timetabletime);
            await _db.SaveChangesAsync();
            return Json(new { Status = "Ok" });
        }

        [Authorize]
        [HttpPost]
        public IActionResult TimeTableDay(string GuidTimeTable)
        {
            if (!string.IsNullOrEmpty(GuidTimeTable))
            {
                return ViewComponent("RecordInfoTimeTable", new { GuidTimeTable });
            }
            return NotFound();
        }

        [Authorize]
        [HttpGet]
        public IActionResult CalendarSegment(string Id, int Mounth)
        {
            return ViewComponent("CalendarSegment", new { Id, Mounth });
        }
    }
}
