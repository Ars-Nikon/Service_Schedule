using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Service_Schedule.Contexts;
using Service_Schedule.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Service_Schedule.Utilits;
using System.Globalization;
using static Service_Schedule.Models.TimeTableTimeVisit;

namespace Service_Schedule.Controllers
{

    [Authorize(Roles = "admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ApplicationContext _db;

        public AdminController(UserManager<User> userManager, SignInManager<User> signInManager, ApplicationContext context)
        {
            _db = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        #region TimeTable
        [HttpGet]
        public async Task<IActionResult> AdminTimeTable()
        {
            var date = DateTime.UtcNow.AddHours(3);
            var specs = await _db.Specialists.Include(x => x.User).Select(x => new { x.Id, x.Surname, x.Patronymic, x.User.Name }).ToListAsync();

            var specsFioList = specs.Select(x => new SpecsFio { FIO = $"{x.Surname} {x.Name} {x.Patronymic}", Id = x.Id }).ToList();

            ViewBag.SelectList = new SelectList(specsFioList, "Id", "FIO");
            ViewBag.Months = new SelectList(Utilit.Mounths.Skip(date.Month - 1), "Value", "Key");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> TimeTableDay(int IdDay, int IdMounth, string IdSpec, string GuidTimeTable)
        {
            if (!string.IsNullOrEmpty(GuidTimeTable))
            {
                return ViewComponent("CalendarInfoSegment", new { GuidTimeTable });
            }
            if (IdDay > 31 || IdDay < 1)
            {
                return null;
            }
            if (IdMounth > 12 || IdMounth < 1)
            {
                return null;
            }
            var spec = await _userManager.FindByIdAsync(IdSpec);
            if (spec == null)
            {
                return null;
            }
            DateTime dateTime = DateTime.UtcNow.AddHours(3);
            var countDay = DateTime.DaysInMonth(dateTime.Year, dateTime.Month);
            if (countDay < IdDay)
            {
                return null;
            }
            var chooseDate = DateTime.Parse($"{IdDay}.{IdMounth}.{DateTime.UtcNow.AddHours(3).Year}", new CultureInfo("ru-RU"));

            return ViewComponent("CalendarInfoSegment", new { chooseDate, IdSpec, GuidTimeTable });
        }

        [HttpGet]
        public IActionResult CalendarSegment(string Id, int Mounth)
        {
            return ViewComponent("CalendarSegment", new { Id, Mounth, admin = true });
        }

        [HttpPost]
        public async Task<JsonResult> CreateTimeTableDate(string date, string IdSpec)
        {
            var spec = await _userManager.FindByIdAsync(IdSpec);
            if (spec == null)
            {
                return Json("Error");
            }
            DateTime chooseDate = DateTime.UtcNow.AddHours(3);
            if (!DateTime.TryParse(date, out chooseDate))
            {
                return Json("Error");
            }
            if (_db.TimeTableDates.Any(x => x.Date == chooseDate.Date && x.Spec_Id == IdSpec))
            {
                return Json("Error");
            }
            var newTimeTable = new TimeTableDate { Date = chooseDate, Spec_Id = IdSpec };
            await _db.TimeTableDates.AddAsync(newTimeTable);
            await _db.SaveChangesAsync();
            return Json(newTimeTable.Id);
        }

        [HttpPost]
        public async Task<JsonResult> AddNewTimeTableTime(string Id_TimeTableDate, TimeSpan? Start, TimeSpan? Finish, Visit_Status? Status)
        {

            if (Start == null || Finish == null || Status == null)
            {
                return Json(new { Status = "Error", Message = "Значения не могут быть пустыми" });
            }
            if (Start >= Finish)
            {
                return Json(new { Status = "Error", Message = "Начало приема не может быть позже (равно) окончанию" });
            }
            if (_db.TimeTableTimeVisits.Where(x => x.Date_Id == Guid.Parse(Id_TimeTableDate)).Any(x => !((Start.Value < x.Visit_Start && Finish.Value <= x.Visit_Start) || (Start.Value >= x.Visit_End))))
            {
                return Json(new { Status = "Error", Message = "Время конфликтует с другим временем приема" });
            }
            await _db.TimeTableTimeVisits.AddAsync(new TimeTableTimeVisit { Date_Id = Guid.Parse(Id_TimeTableDate), Status = Status.Value, Visit_Start = Start.Value, Visit_End = Finish.Value });
            await _db.SaveChangesAsync();

            return Json(new { Status = "Ok", Message = "Ok" });
        }

        [HttpPost]
        public async Task<JsonResult> DeleteTimeTableTime(string Id_TimeTableTime)
        {
            if (Id_TimeTableTime == null)
            {
                return Json(new { Status = "Error", Message = "Id отсутвтыет" });
            }
            var timeTableTime = await _db.TimeTableTimeVisits.FirstOrDefaultAsync(x => x.Id == Guid.Parse(Id_TimeTableTime));
            if (timeTableTime == null)
            {
                return Json(new { Status = "Error", Message = "Расписания отсутствует в базе" });
            }

            _db.TimeTableTimeVisits.Remove(timeTableTime);
            await _db.SaveChangesAsync();

            return Json(new { Status = "Ok", Message = "Ok" });
        }

        [HttpPost]
        public async Task<JsonResult> UpdateTimeTableTime(string Id_TimeTableTime, Visit_Status? Status, string Discription)
        {
            if (Id_TimeTableTime == null)
            {
                return Json(new { Status = "Error", Message = "Id отсутвтыет" });
            }
            if (Status == null)
            {
                return Json(new { Status = "Error", Message = "Status отсутвтыет" });
            }
            var timeTableTime = await _db.TimeTableTimeVisits.FirstOrDefaultAsync(x => x.Id == Guid.Parse(Id_TimeTableTime));
            if (timeTableTime == null)
            {
                return Json(new { Status = "Error", Message = "Расписания отсутствует в базе" });
            }
            timeTableTime.Status = Status.Value;
            if (Status.Value == Visit_Status.Free && timeTableTime.User_Id != null)
            {
                timeTableTime.User_Id = null;
            }
            timeTableTime.Discription = Discription;
            _db.TimeTableTimeVisits.Update(timeTableTime);
            await _db.SaveChangesAsync();
            return Json(new { Status = "Ok", Message = "Ok" });
        }
        #endregion

        #region User
        [HttpGet]
        public async Task<IActionResult> Users()
        {
            var users = await _userManager.GetUsersInRoleAsync("user");
            return View(users.OrderBy(x => x.DateCreate).ToList());
        }

        [HttpGet]
        public async Task<IActionResult> UserInfoSegment(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return null;
            }
            return ViewComponent("UserInfoSegment", user);
        }

        [HttpPost]
        public async Task<JsonResult> EditUser(AccountViewModel model)
        {
            var ResultMessage = new List<string>();
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.Id);

                if (user == null)
                {
                    return null;
                }
                if (!string.IsNullOrEmpty(model.Password?.Trim()))
                {
                    var _passwordValidator =
                HttpContext.RequestServices.GetService(typeof(IPasswordValidator<User>)) as IPasswordValidator<User>;
                    var _passwordHasher =
                        HttpContext.RequestServices.GetService(typeof(IPasswordHasher<User>)) as IPasswordHasher<User>;

                    IdentityResult resulPassword =
                        await _passwordValidator.ValidateAsync(_userManager, user, model.Password?.Trim());
                    if (resulPassword.Succeeded)
                    {
                        user.PasswordHash = _passwordHasher.HashPassword(user, model.Password);
                        await _userManager.UpdateAsync(user);
                        ResultMessage.Add("Пароль успешно изменен");
                    }
                    else
                    {
                        foreach (var error in resulPassword.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
                if (model.IsChange(user))
                {
                    user.Email = model.Email?.Trim();
                    user.UserName = model.Email?.Trim();
                    user.BirthDate = model.BirthDate;
                    user.Name = model.Name?.Trim();
                    user.PhoneNumber = model.Phone?.Trim();
                    user.Gender = model.Gender;
                    var result = await _userManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        ResultMessage.Add("Данные успешно изменены");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
            }
            var Errors = ModelState.Values.Where(x => x.ValidationState == ModelValidationState.Invalid).SelectMany(x => x.Errors.Select(y => y.ErrorMessage).ToList()).ToList();
            if (Errors.Count > 0)
            {
                return Json(new { Errors, Success = false });
            }

            return Json(new { ResultMessage, Success = true });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(AccountViewModel model)
        {
            if (model == null || string.IsNullOrEmpty(model.Id))
            {
                return StatusCode(400);
            }
            User user = await _userManager.FindByIdAsync(model.Id);
            var IsUser = await _userManager.IsInRoleAsync(user, "user");
            if (!IsUser)
            {
                return StatusCode(400);
            }
            if (user == null)
            {
                return StatusCode(400);
            }
            IdentityResult result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return Ok();
            }
            return StatusCode(400);
        }


        [HttpGet]
        public async Task<IActionResult> UsersList()
        {
            var users = await _userManager.GetUsersInRoleAsync("user");
            return ViewComponent("UsersListSegment", users.OrderBy(x => x.DateCreate).ToList());
        }
        #endregion

        #region Spec
        [HttpGet]
        public IActionResult NewSpecialist()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> NewSpecialist(RegistrationSpecView model)
        {
            if (ModelState.IsValid)
            {
                User user = model.ConvertToUser();

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "spec");
                    var spec = model.ConvertToSpecialist();
                    spec.Id = user.Id;
                    spec.Accepts = false;
                    _db.Specialists.Add(spec);
                    await _db.SaveChangesAsync();
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> Specialists()
        {
            var specialists = await _db.Specialists.Include(x => x.User).OrderBy(x => x.User.DateCreate).ToListAsync();

            return View(specialists.Select(x => x.ConvertToSpecialistView()).ToList());
        }


        [HttpGet]
        public IActionResult SpecInfoSegment(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }
            var spec = _db.Specialists.Include(x => x.User).FirstOrDefault(x => x.Id == id);
            if (spec == null)
            {
                return null;
            }
            return ViewComponent("SpecInfoSegment", spec.ConvertToSpecialistView());
        }


        [HttpGet]
        public async Task<IActionResult> SpecsList()
        {
            var specialists = await _db.Specialists.Include(x => x.User).OrderBy(x => x.User.DateCreate).ToListAsync();
            return ViewComponent("SpecListSegment", specialists.Select(x => x.ConvertToSpecialistView()).ToList());
        }



        [HttpPost]
        public async Task<IActionResult> ChangePhoto(string Id, IFormFile Avatar)
        {
            if (Avatar != null && Id != null)
            {
                var spec = await _db.Specialists.Include(x => x.User).FirstOrDefaultAsync(x => x.Id == Id);

                if (spec == null)
                {
                    return null;
                }
                using (var binaryReader = new BinaryReader(Avatar.OpenReadStream()))
                {
                    spec.Avatar = binaryReader.ReadBytes((int)Avatar.Length);
                    _db.Specialists.Update(spec);
                }
            }
            await _db.SaveChangesAsync();
            return RedirectToAction("Specialists");
        }

        [HttpPost]
        public async Task<JsonResult> EditSpec(SpecialistView model)
        {
            var ResultMessage = new List<string>();
            if (ModelState.IsValid)
            {
                var spec = await _db.Specialists.Include(x => x.User).FirstOrDefaultAsync(x => x.Id == model.AccountViewModel.Id);

                if (spec == null)
                {
                    return null;
                }

                if (!string.IsNullOrEmpty(model.AccountViewModel.Password?.Trim()))
                {
                    var _passwordValidator =
                HttpContext.RequestServices.GetService(typeof(IPasswordValidator<User>)) as IPasswordValidator<User>;
                    var _passwordHasher =
                        HttpContext.RequestServices.GetService(typeof(IPasswordHasher<User>)) as IPasswordHasher<User>;

                    IdentityResult resulPassword =
                        await _passwordValidator.ValidateAsync(_userManager, spec.User, model.AccountViewModel.Password?.Trim());
                    if (resulPassword.Succeeded)
                    {
                        spec.User.PasswordHash = _passwordHasher.HashPassword(spec.User, model.AccountViewModel.Password);
                        await _userManager.UpdateAsync(spec.User);
                        ResultMessage.Add("Пароль успешно изменен");
                    }
                    else
                    {
                        foreach (var error in resulPassword.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
                if (model.AccountViewModel.IsChange(spec.User))
                {
                    spec.User.Email = model.AccountViewModel.Email?.Trim();
                    spec.User.UserName = model.AccountViewModel.Email?.Trim();
                    spec.User.BirthDate = model.AccountViewModel.BirthDate;
                    spec.User.Name = model.AccountViewModel.Name?.Trim();
                    spec.User.PhoneNumber = model.AccountViewModel.Phone?.Trim();
                    spec.User.Gender = model.AccountViewModel.Gender;
                    var result = await _userManager.UpdateAsync(spec.User);
                    if (result.Succeeded)
                    {
                        ResultMessage.Add("Данные пользователя успешно изменены<br>");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
                if (model.IsChange(spec))
                {
                    spec.Price = model.Price;
                    spec.Accepts = model.Accepts;
                    spec.Discription = model.Discription;
                    spec.Patronymic = model.Patronymic;
                    spec.Surname = model.Surname;
                    spec.TypeSpec = model.TypeSpec;
                    _db.Specialists.Update(spec);
                    ResultMessage.Add("Данные спициалиста успешно изменены<br>");
                }
            }
            var Errors = ModelState.Values.Where(x => x.ValidationState == ModelValidationState.Invalid).SelectMany(x => x.Errors.Select(y => y.ErrorMessage + "<br>").ToList()).ToList();
            if (Errors.Count > 0)
            {
                return Json(new { Errors, Success = false });
            }

            await _db.SaveChangesAsync();
            return Json(new { ResultMessage, Success = true });
        }

        #endregion
    }
}
