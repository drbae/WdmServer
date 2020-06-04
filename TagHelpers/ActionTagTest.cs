using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace DrBAE.WdmServer.TagHelpers
{
    [HtmlTargetElement(Attributes = AttributeName)]
    public class ActionTagTest : TagHelper
    {
        const string AttributeName = "tag-test";

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var att = output.Attributes[AttributeName];
            output.Attributes.Remove(att);
            //output.AddClass("action-icon", HtmlEncoder.Default);

            output.Attributes.Remove(output.Attributes["href"]);
            output.Attributes.Add("href", "javascript:void();");

            output.TagMode = TagMode.StartTagAndEndTag;
        }
    }
}
