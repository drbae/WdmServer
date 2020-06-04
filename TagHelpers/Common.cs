using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DrBAE.WdmServer.TagHelpers
{
    public enum ControlAction
    {
        Index, Details, Edit, Create, Delete, Refresh,

        ClickEvent,//href is same as action name

        Excel, ExcelSave, ExcelDown,

        End
    }

}
