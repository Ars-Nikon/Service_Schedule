using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Service_Schedule.ViewComponents
{
    public class DiscriptionServices : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            string htmlContent = String.Empty;
            using (StreamReader reader = new StreamReader("wwwroot/Html/DiscriptionServices.html"))
            {
                htmlContent = await reader.ReadToEndAsync();
            }
            return new HtmlContentViewComponentResult(new HtmlString(htmlContent));
        }
    }
}
