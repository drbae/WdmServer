using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Universe.Web.Data;
#pragma warning disable CS8618 // null을 허용하지 않는 필드가 초기화되지 않았습니다. nullable로 선언하는 것이 좋습니다.

namespace DrBAE.WdmServer.Models
{
    public class AnalysisDataModel : ModelBase<AnalysisDataModel>
    {
        public const string pAnalysis = nameof(Analysis);
        public const string pRawDataId = nameof(RawDataId);
        const string _S = "DUT SN";
        const string _R = "Raw Data";

        static AnalysisDataModel()
        {
            PropNames.AddRange(new[] { nameof(Sn), nameof(Data), nameof(AnalysisId), pRawDataId });
            DisplayNames.AddRange(new[] { _S, nameof(Data), pAnalysis, _R });
        }

        [DisplayName(_S)] public string Sn { get; set; }
        public string Data { get; set; }

        [DisplayName(pAnalysis), ForeignKey(pAnalysis)] public int AnalysisId { get; set; }
        [DisplayName(pAnalysis)] public AnalysisModel Analysis { get; set; }

        [DisplayName(_R)] public int RawDataId { get; set; }
        [DisplayName(_R), ForeignKey(pRawDataId)] public RawDataModel RawData { get; set; }

    }
}
#pragma warning restore CS8618 // null을 허용하지 않는 필드가 초기화되지 않았습니다. nullable로 선언하는 것이 좋습니다.
