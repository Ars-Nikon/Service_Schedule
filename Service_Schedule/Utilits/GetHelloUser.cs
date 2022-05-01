using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Service_Schedule.Models;
using System.Threading.Tasks;

namespace Service_Schedule.Utilits
{
    [ViewComponent]
    public class GetHelloUser : ViewComponent
    {
        private readonly UserManager<User> _userManager;
        public GetHelloUser(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var result = new HtmlContentViewComponentResult(
                new HtmlString($"<h1 class=\"display-4\">Привет!!!</h1>"));

            if (HttpContext.Request.Cookies.ContainsKey("name"))
            {
                string name = HttpContext.Request.Cookies["name"];
                return new HtmlContentViewComponentResult(
               new HtmlString($"<h1 class=\"display-4\">Привет, {name}!!!</h1>")
           );
            }


            if (!User.Identity.IsAuthenticated)
            {
                return result;
            }
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            if (user == null)
            {
                return result;
            }
            HttpContext.Response.Cookies.Append("name", user.Name);

            return new HtmlContentViewComponentResult(
                new HtmlString($"<h1 class=\"display - 4\">Привет, {user.Name}!!!</h1>")
            );
        }
    }
}
