using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using Universe.Web.Data;
#pragma warning disable CS8618 // null을 허용하지 않는 필드가 초기화되지 않았습니다. nullable로 선언하는 것이 좋습니다.

namespace DrBAE.WdmServer.Models
{
    public class PigtailReportFormat : ModelBase<PigtailReportFormat>
    {
        public const string pName = nameof(Name);
        public const string pConfigId = nameof(ConfigId);
        public const string pFormFile = nameof(FormFile);
        public const string pIFormFile = nameof(IFormFile);
        public const string pConfig = nameof(Config);

        const string _FN = "Format Name";
        const string _FF = "Format File";
        const string _CI = "Config";

        static PigtailReportFormat()
        {
            PropNames.AddRange(new[] { nameof(Name), nameof(FormFile), nameof(ConfigId) });
            DisplayNames.AddRange(new[] { _FN, _FF, _CI });
        }

        [DisplayName(_FN)] public string Name { get; set; }
        [DisplayName(_FF)] public byte[] FormFile { get; set; }
        [NotMapped] public IFormFile IFormFile { get; set; }
        public virtual ConfigModel Config { get; set; }
        [DisplayName(_CI), ForeignKey("Config")] public int ConfigId { get; set; }

    }
}
#pragma warning restore CS8618 // null을 허용하지 않는 필드가 초기화되지 않았습니다. nullable로 선언하는 것이 좋습니다.
