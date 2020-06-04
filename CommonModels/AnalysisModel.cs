using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Universe.Web.Data;
#pragma warning disable CS8618 // null을 허용하지 않는 필드가 초기화되지 않았습니다. nullable로 선언하는 것이 좋습니다.

namespace DrBAE.WdmServer.Models
{
    public class AnalysisModel : ModelBase<AnalysisModel>
    {
        public const string pConfigId = nameof(ConfigId);
        public const string pConfig = nameof(Config);
        public const string pDesc = nameof(Description);
        public const string pUserId = nameof(UserId);
        public const string pUser = nameof(User);
        public const string pNumDut = nameof(NumDut);
        public const string pDate = nameof(Date);
        public const string pAnalysisRawUploads = nameof(AnalysisRawUploads);
        public const string pRawUploadIds = nameof(RawUploadIds);
        public const string pAnalysisData = nameof(AnalysisData);
        public const string pDeltaT = nameof(DeltaT);

        const string _C = "Config";
        const string _R = "Raw Uploads";
        const string _U = "User";
        const string _D = "Num DUT";
        const string _T = "Δt";

        static AnalysisModel()
        {
            PropNames.AddRange(new[] { pDesc, pConfigId, pUserId, pNumDut, pDate, pDeltaT, pAnalysisRawUploads });
            DisplayNames.AddRange(new[] { pDesc, _C, _U, _D, pDate, _T, _R });
        }

        public string Description { get; set; }

        [DisplayName(_C), ForeignKey(pConfigId)] public ConfigModel Config { get; set; }
        [DisplayName(_C)] public int ConfigId { get; set; }

        [DisplayName(_U), ForeignKey(pUserId)] public IdentityUser User { get; set; }
        [DisplayName(_U)] public string UserId { get; set; }

        [DisplayName(_D)] public int NumDut { get; set; }//Analysis에 대한 RawData,AnalysisData,ChipData 개수
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyMMdd-HHmmss}")] public DateTime Date { get; set; }

        [DisplayName(_R)] public ICollection<AnalysisRawUpload> AnalysisRawUploads { get; set; }
        [NotMapped] public List<int> RawUploadIds { get; set; }

        public ICollection<AnalysisDataModel> AnalysisData { get; set; }

        [DisplayName(_T)] public long DeltaT { get; set; }
    }
}
#pragma warning restore CS8618 // null을 허용하지 않는 필드가 초기화되지 않았습니다. nullable로 선언하는 것이 좋습니다.
