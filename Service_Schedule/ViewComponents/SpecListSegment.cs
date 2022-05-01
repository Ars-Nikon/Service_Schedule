using Microsoft.AspNetCore.Mvc;
using Service_Schedule.Models;
using System.Collections.Generic;

namespace Service_Schedule.ViewComponents
{
    public class SpecListSegment : ViewComponent
    {
        public IViewComponentResult Invoke(List<SpecialistView> specialists)
        {
            return View("~/Views/Admin/Components/SegmentSpecs/SpecListSegment.cshtml", specialists);
        }
    }
}
