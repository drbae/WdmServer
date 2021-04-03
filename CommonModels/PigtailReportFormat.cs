using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using Universe.Web.Data;
#pragma warning disable CS8618 // null�� ������� �ʴ� �ʵ尡 �ʱ�ȭ���� �ʾҽ��ϴ�. nullable�� �����ϴ� ���� �����ϴ�.

namespace DrBAE.WdmServer.Models
{
    using MR = ModelRecordAttribute;
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

        [MR(1), DisplayName(_FN)] public string Name { get; set; }
        [MR(2), DisplayName(_FF)] public byte[] FormFile { get; set; }
        [NotMapped] public IFormFile IFormFile { get; set; }
        [MR(3)] public virtual ConfigModel Config { get; set; }
        [MR(4), DisplayName(_CI), ForeignKey("Config")] public int ConfigId { get; set; }

    }
}
#pragma warning restore CS8618 // null�� ������� �ʴ� �ʵ尡 �ʱ�ȭ���� �ʾҽ��ϴ�. nullable�� �����ϴ� ���� �����ϴ�.
