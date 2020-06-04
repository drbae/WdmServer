using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Universe.Web.Data;
#pragma warning disable CS8618 // null을 허용하지 않는 필드가 초기화되지 않았습니다. nullable로 선언하는 것이 좋습니다.

namespace DrBAE.WdmServer.Models
{
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
                
        static PigtailReport()
        {
            PropNames.AddRange(new[] { pFormatId, pLotId, pDesc, pDataFile, pReportName, pReport });
            DisplayNames.AddRange(new[] { _F, _L, pDesc, _D, _R, pReport });
        }

        [DisplayName(_F), ForeignKey(nameof(Format))] public int FormatId { get; set; }
        [ForeignKey(pFormatId)] public virtual PigtailReportFormat Format { get; set; }

        [DisplayName(_L)] public int LotId { get; set; }
        [ForeignKey(pLotId)] public virtual LotModel Lot { get; set; }

        public string Description { get; set; }
        
        [DisplayName(_D)] public byte[] DataFile { get; set; }
        [NotMapped] public IFormFile IDataFile { get; set; }// = EmptyFile.Default;

        public byte[] Report { get; set; }
        [DisplayName(_R)] public string ReportName { get; set; }
        [NotMapped] public int ReportSize => Report?.Length ?? 0;

    }
}
#pragma warning restore CS8618 // null을 허용하지 않는 필드가 초기화되지 않았습니다. nullable로 선언하는 것이 좋습니다.
