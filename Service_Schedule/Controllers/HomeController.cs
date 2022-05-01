using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;

        public HomeController(ILogger<HomeController> logger, UserManager<User> userManager, IConfiguration configuration)
        {
            _configuration = configuration;
            _userManager = userManager;
            _logger = logger;
        }

        public IActionResult Index()
        {

            ViewBag.Text = _configuration.GetSection("TextInIndexPage").Value;
            return View();
        }

        public async Task<IActionResult> Record(string Id)
        {


            return View();
        }

    }
}
