using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DrBAE.WdmServer.WebUtility
{
    public static class CookieExtension
    {
        public static void WriteCookie(this Controller controller, string key, string value, int numHours = 8)
        {
            var options = new CookieOptions();
            options.Expires = DateTime.Now.AddHours(numHours);
            controller.Response.Cookies.Append(key, value, options);
        }
        public static string ReadCookie(this Controller controller, string key)
        {
            if (controller.Request.Cookies.ContainsKey(key)) return controller.Request.Cookies[key];
            else return "";
        }


    }//class

}
