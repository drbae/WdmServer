using Microsoft.AspNetCore.Razor.TagHelpers;

namespace DrBAE.WdmServer.TagHelpers
{
    [HtmlTargetElement(AttributeName)]
    [HtmlTargetElement(Attributes = AttributeName)]
    public class NavManuTagHelper : TagHelper
    {
        const string AttributeName = "neon-nav";

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "a";

            //아이콘추가
            var navIcon = @"<i class=""material-icons n-icon"">assignment</i>";
            output.PreContent.SetHtmlContent(navIcon);
            output.Attributes.Add("class", "nav-item");
        }
    }
}
