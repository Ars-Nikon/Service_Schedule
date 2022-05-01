using Microsoft.AspNetCore.Mvc;
using Service_Schedule.Models;
using System.Collections.Generic;

namespace Service_Schedule.ViewComponents
{
    public class UsersListSegment : ViewComponent
    {
        public UsersListSegment()
        {

        }
        public IViewComponentResult Invoke(List<User> users)
        {
            return View("~/Views/Admin/Components/SegmentUsers/UsersListSegment.cshtml", users);
        }
    }
}
