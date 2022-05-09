using Microsoft.AspNetCore.Mvc;
using Service_Schedule.Models;

namespace Service_Schedule.ViewComponents
{
    public class UserInfoRecord : ViewComponent
    {
        public IViewComponentResult Invoke(User user)
        {
            return View("~/Views/Account/UserInfoRecord.cshtml", user);
        }
    }


    
}
