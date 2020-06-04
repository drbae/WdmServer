using DrBAE.WdmServer.TagHelpers;
using DrBAE.WdmServer.WebApp.Controllers;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace DrBAE.WdmServer.WebApp.Helpers
{
    [HtmlTargetElement(Attributes = AttributeName)]
    public class ActionTag : TagHelper
    {
        const string AttributeName = "action-icon";
        public const string OpenFormId = "form-file-open";
        public const string OpenFileId = "file-open";
        public const string SaveFormId = "form-excel-save";
        public const string DownFormId = "form-file-export";
        public const string DownFileId = "file-export";

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var att = output.Attributes[AttributeName];
            output.Attributes.Remove(att);
            output.AddClass("action-icon", HtmlEncoder.Default);

            var content = string.IsNullOrWhiteSpace(att.Value.ToString()) ? new string[] { "", "" } : att.Value.ToString().Split(';');
            var actionName = (string)context.AllAttributes["asp-action"].Value;
            var imgName = string.IsNullOrWhiteSpace(content[0]) ? actionName : content[0];

            var actionText = content.Length > 1 && !string.IsNullOrWhiteSpace(content[1]) ? content[1] : ContentControllerBase.ToText(actionName);
            var imgHtml = $@"<img src='/images/icon/{imgName}.png'/>" + actionText;
            output.Content.SetHtmlContent(imgHtml);
            output.TagMode = TagMode.StartTagAndEndTag;

            var action = ContentControllerBase.ToAction(actionName);

            //set click event handler
            if (action.HasClickHandler())
            {
                output.Attributes.Remove(output.Attributes["href"]);
                output.Attributes.Add("href", "javascript:void();");
            }
            if (action == ControlAction.Refresh)
            {
                output.Attributes.Add("onclick", "window.document.location.reload();");
            }
            if (action == ControlAction.Excel) output.Attributes.Add("onclick", $"document.getElementById('{OpenFileId}').click();;");
            if (action == ControlAction.ExcelSave) output.Attributes.Add("onclick", $"document.getElementById('{SaveFormId}').submit();");
            if (action == ControlAction.ExcelDown) output.Attributes.Add("onclick", $"document.getElementById('{DownFormId}').submit();");

        }
    }
}
