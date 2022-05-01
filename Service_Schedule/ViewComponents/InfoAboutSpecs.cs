using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Service_Schedule.Contexts;
using System.Linq;
using System.Threading.Tasks;

namespace Service_Schedule.ViewComponents
{
    public class InfoAboutSpecs : ViewComponent
    {
        private readonly ApplicationContext _db;

        public InfoAboutSpecs(ApplicationContext context)
        {
            _db = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
          // Вообщем эти данные надо кешировать, что бы каждый раз не стучать к бд
            var specs = await _db.Specialists.Include(x => x.User).Where(x=>(x.Accepts == true)).ToListAsync();
            return View("~/Views/Home/Components/InfoAboutSpecs.cshtml", specs);
        }

    }
}
