using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DrBAE.WdmServer.TagHelpers
{
    /// <summary>
    /// 임시 클래스
    /// </summary>
    public class Utility
    {
        protected static Dictionary<string, ControlAction> _nameToAction = new Dictionary<string, ControlAction>()
        {
            { "Index", ControlAction.Index },
            { "Details", ControlAction.Details },
            { "Edit", ControlAction.Edit },
            { "Delete", ControlAction.Delete },
            { "Create", ControlAction.Create },
            { "Refresh", ControlAction.Refresh },
            { "Excel", ControlAction.Excel },
            { "ExcelSave", ControlAction.ExcelSave },
            { "ExcelDown", ControlAction.ExcelDown },
        };
        protected static Dictionary<ControlAction, string> _actionToText = new Dictionary<ControlAction, string>()
        {
            { ControlAction.Index, "목록" },
            { ControlAction.Details, "상세" },
            { ControlAction.Edit, "수정" },
            { ControlAction.Delete, "삭제" },
            { ControlAction.Create, "추가" },
            { ControlAction.Refresh, "갱신" },
            { ControlAction.Excel, "엑셀 올리기" },
            { ControlAction.ExcelSave, "저장" },
            { ControlAction.ExcelDown, "엑셀 내려받기" },
        };

        /// <summary>
        /// ControlAction enum --> UI Text
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public static string ToText(ControlAction action) => _actionToText.ContainsKey(action) ? _actionToText[action] : action.ToString();

        /// <summary>
        /// action (method) name --> UI Text
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string ToText(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return "--UNKNOWN--";
            return _nameToAction.ContainsKey(name) ? ToText(_nameToAction[name]) : name;
        }

        /// <summary>
        /// action (method) name --> ControlAction enum
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static ControlAction ToAction(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return ControlAction.Index;
            return _nameToAction.ContainsKey(name) ? _nameToAction[name] : ControlAction.Index;
        }

    }//class

}
