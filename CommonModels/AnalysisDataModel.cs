using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Universe.Web.Data;
#pragma warning disable CS8618 // null을 허용하지 않는 필드가 초기화되지 않았습니다.

namespace DrBAE.WdmServer.Models
{
    using MR = ModelRecordAttribute;
    public class AnalysisDataModel : ModelBase<AnalysisDataModel>
    {
        public const string pAnalysis = nameof(Analysis);
        public const string pRawDataId = nameof(RawDataId);

        [MR(1), DisplayName("DUT SN")] public string Sn { get; set; }
        [MR(2)] public string Data { get; set; }

        [MR(3), DisplayName(pAnalysis), ForeignKey(pAnalysis)] public int AnalysisId { get; set; }
        [MR(4), DisplayName(pAnalysis)] public AnalysisModel Analysis { get; set; }

        [MR(5), DisplayName("Raw ID")] public int RawDataId { get; set; }
        [MR(6), DisplayName("Raw Data"), ForeignKey(pRawDataId)] public RawDataModel RawData { get; set; }
    }
}
#pragma warning restore CS8618 // null을 허용하지 않는 필드가 초기화되지 않았습니다.
