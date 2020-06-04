using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Threading.Tasks;

namespace DrBAE.WdmServer.TagHelpers
{
    //[HtmlTargetElement(Attributes = AttributeName)]
    [HtmlTargetElement("n-mail")]
    public class EmailLinkTagHelper : TagHelper
    {
        const string AttributeName = "neon-mail";
        const string domain = "neonphotonics.com";
        
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "a";

            string originText = (await output.GetChildContentAsync()).GetContent();

            string emailString = $"{originText}@{domain}";

            output.Attributes.Add("href", $"mailto:{emailString}");

            output.Content.SetContent(emailString);
        }
    }
}
