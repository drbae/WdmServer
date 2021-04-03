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
    using MR = ModelRecordAttribute;
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
        
        [MR(1)] public string Description { get; set; }

        [MR(2), DisplayName(_C), ForeignKey(pConfigId)] public ConfigModel Config { get; set; }
        [MR(3), DisplayName(_C)] public int ConfigId { get; set; }

        [MR(4), DisplayName(_U), ForeignKey(pUserId)] public IdentityUser User { get; set; }
        [MR(5), DisplayName(_U)] public string UserId { get; set; }

        [MR(6), DisplayName(_D)] public int NumDut { get; set; }//Analysis에 대한 RawData,AnalysisData,ChipData 개수
        [MR(7), DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyMMdd-HHmmss}")] public DateTime Date { get; set; }

        [MR(8), DisplayName(_R)] public ICollection<AnalysisRawUpload> AnalysisRawUploads { get; set; }
        [NotMapped] public List<int> RawUploadIds { get; set; }

        [MR(9)] public ICollection<AnalysisDataModel> AnalysisData { get; set; }

        [MR(10), DisplayName(_T)] public long DeltaT { get; set; }
    }
}
#pragma warning restore CS8618 // null을 허용하지 않는 필드가 초기화되지 않았습니다. nullable로 선언하는 것이 좋습니다.
