﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Service_Schedule.Models;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using System;

namespace Service_Schedule.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result =
                    await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {
                    var user = await _userManager.FindByNameAsync(model.Email);
                    HttpContext.Response.Cookies.Append("name", user?.Name ?? "NoName");
                    // проверяем, принадлежит ли URL приложению
                    if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Неправильный логин и (или) пароль");
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register(string returnUrl = null)
        {
            return View(new RegisterViewModel { ReturnUrl = returnUrl });
        }



        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = new User
                {
                    Email = model.Email?.Trim(),
                    UserName = model.Email?.Trim(),
                    BirthDate = model.BirthDate,
                    Name = model.Name?.Trim(),
                    PhoneNumber = model.Phone?.Trim(),
                    Gender = model.Gender,
                    DateCreate = DateTime.UtcNow.AddHours(3)
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "user");
                    await _signInManager.SignInAsync(user, false);
                    HttpContext.Response.Cookies.Append("name", user.Name);
                    if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
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

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Account()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user == null)
            {
                await _signInManager.SignOutAsync();
                return RedirectToAction("Index", "Home");
            }
            var usermodel = Utilits.Utilit.ConvertUserByAccountUser(user);
            return View(usermodel);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Account(AccountViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(User.Identity.Name);

                if (user == null)
                {
                    return RedirectToAction("Index", "Home");
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
                        ViewBag.SuccessChangePassword = true;
                        user.PasswordHash = _passwordHasher.HashPassword(user, model.Password);
                        await _userManager.UpdateAsync(user);
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
                    HttpContext.Response.Cookies.Append("name", user.Name);
                    if (result.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, false);
                        ViewBag.SuccessChangeData = true;
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
            return View(model);
        }
        [HttpGet]
        [Authorize(Roles = "admin,spec")]
        public async Task<IActionResult> UserInfoRecord(string Id)
        {
            if (string.IsNullOrEmpty(Id))
            {
                return NotFound();
            }
            var user = await _userManager.FindByIdAsync(Id);
            if (user == null)
            {
                return NotFound();
            }
            return ViewComponent("UserInfoRecord", user);
        }
    }
}
