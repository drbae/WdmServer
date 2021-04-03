using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Universe.Web.Data;
using System.ComponentModel;
using System.Text;

#pragma warning disable CS8618 // null을 허용하지 않는 필드가 초기화되지 않았습니다. nullable로 선언하는 것이 좋습니다.
namespace DrBAE.WdmServer.Models
{
    using MR = ModelRecordAttribute;
    public class RawUpload : ModelBase<RawUpload>
    {
        public const string pDesc = nameof(Description);
        public const string pConfigId = nameof(ConfigId);
        public const string pConfig = nameof(Config);
        public const string pUserId = nameof(UserId);
        public const string pUser = nameof(User);
        public const string pRawData = nameof(RawData);
        public const string pAnalysisRawUploads = nameof(AnalysisRawUploads);
        const string _C = "Config";
        const string _U = "Uploader";
        const string _ND = "NumDut";
        const string _A = "Analyses";
        
        [MR(1)] public string Description { get; set; }
        [MR(2)] [ForeignKey(pConfigId)] public ConfigModel Config { get; set; }
        [MR(3)] [DisplayName(_C)] public int ConfigId { get; set; }

        [MR(4)] [ForeignKey(pUserId)] public IdentityUser User { get; set; }
        [MR(5)] [DisplayName(_U)] public string UserId { get; set; }
        [MR(6)] [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyMMdd-HHmmss}")] public DateTime Date { get; set; }

        [MR(7)] [DisplayName(_ND)] public int NumDut { get; set; }//Analysis에 대한 RawData,AnalysisData,ChipData 개수        
        [MR(8)] public ICollection<RawDataModel> RawData { get; set; }

        [NotMapped] public List<IFormFile>? Files { get; set; }
        [NotMapped] public byte[]? ZipFile { get; set; }//

        [MR(9)]
        [DisplayName(_A)]
        public ICollection<AnalysisRawUpload> AnalysisRawUploads { get; set; }
            = new List<AnalysisRawUpload>();
        [NotMapped] public string Analyses
        {
            get
            {
                var sb = new StringBuilder();
                foreach (var au in AnalysisRawUploads) sb.Append($"{au.Analysis.Description}, ");
                if (sb.Length >= 2) sb.Remove(sb.Length - 2, 2);
                return sb.ToString();
            }
        }

        [MR(10)] public string RawLogicVersion { get; set; } = "";
        [MR(11)] public long DeltaT { get; set; } = 0;
    }
}
#pragma warning restore CS8618 // null을 허용하지 않는 필드가 초기화되지 않았습니다. nullable로 선언하는 것이 좋습니다.
