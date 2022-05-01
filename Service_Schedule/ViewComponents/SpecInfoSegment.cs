using Microsoft.AspNetCore.Mvc;
using Service_Schedule.Models;

namespace Service_Schedule.ViewComponents
{
    public class SpecInfoSegment : ViewComponent
    {
        public IViewComponentResult Invoke(SpecialistView specs)
        {
            return View("~/Views/Admin/Components/SegmentSpecs/SpecInfoSegment.cshtml", specs);
        }
    }
}
