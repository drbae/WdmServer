using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DrBAE.WdmServer.TagHelpers
{
    public static class Extension
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public static bool HasClickHandler(this ControlAction action) => action > ControlAction.ClickEvent;


       
    }
}
