using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Service_Schedule.CustomTagHelpers
{
    public class WelcomeUserTagHelper :  TagHelper
    {
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
        }
    }
}
