using Microsoft.AspNetCore.Mvc;
using Service_Schedule.Models;

namespace Service_Schedule.ViewComponents
{
    public class UserInfoSegment : ViewComponent
    {
        public IViewComponentResult Invoke(User user)
        {
            AccountViewModel accountViewModel = null;
            if (user == null)
            {
                return View("~/Views/Admin/Components/SegmentUsers/UserInfoSegment.cshtml", accountViewModel);
            }
            accountViewModel = Utilits.Utilit.ConvertUserByAccountUser(user);
            return View("~/Views/Admin/Components/SegmentUsers/UserInfoSegment.cshtml", accountViewModel);
        }
    }
}
