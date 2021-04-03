using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Universe.Web.Data;
#pragma warning disable CS8618 // null�� ������� �ʴ� �ʵ尡 �ʱ�ȭ���� �ʾҽ��ϴ�. nullable�� �����ϴ� ���� �����ϴ�.

namespace DrBAE.WdmServer.Models
{
    using MR = ModelRecordAttribute;
    public class PigtailReport : ModelBase<PigtailReport>
    {
        public const string pFormatId = nameof(FormatId);
        public const string pLotId = nameof(LotId);
        public const string pDesc = nameof(Description);

        public const string pLot = nameof(Lot);
        public const string pFormat = nameof(Format);
        public const string pDataFile = nameof(DataFile);
        public const string pReportName = nameof(ReportName);
        public const string pReport = nameof(Report);

        const string _F = "Format";
        const string _L = "LOT";
        const string _D = "Data File";
        const string _R = "Report Name";         

        [MR(1), DisplayName(_F), ForeignKey(nameof(Format))] public int FormatId { get; set; }
        [MR(2), ForeignKey(pFormatId)] public virtual PigtailReportFormat Format { get; set; }

        [MR(3), DisplayName(_L)] public int LotId { get; set; }
        [MR(4), ForeignKey(pLotId)] public virtual LotModel Lot { get; set; }

        [MR(5)] public string Description { get; set; }
        
        [MR(6), DisplayName(_D)] public byte[] DataFile { get; set; }
        [NotMapped] public IFormFile IDataFile { get; set; }// = EmptyFile.Default;

        [MR(7)] public byte[] Report { get; set; }
        [MR(8), DisplayName(_R)] public string ReportName { get; set; }
        [NotMapped] public int ReportSize => Report?.Length ?? 0;

    }
}
#pragma warning restore CS8618 // null�� ������� �ʴ� �ʵ尡 �ʱ�ȭ���� �ʾҽ��ϴ�. nullable�� �����ϴ� ���� �����ϴ�.
