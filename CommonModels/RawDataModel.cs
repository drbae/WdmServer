using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Universe.Web.Data;
#pragma warning disable CS8618 // null을 허용하지 않는 필드가 초기화되지 않았습니다. nullable로 선언하는 것이 좋습니다.

namespace DrBAE.WdmServer.Models
{
    using MR = ModelRecordAttribute;
    public class RawDataModel : ModelBase<RawDataModel>
    {
        public const string pSn = nameof(Sn);
        public const string pData = nameof(Data);
        const string _DS = "DUT SN";
        const string _U = "Upload";
        const string _A = "Analyses";        

        [MR(1), DisplayName(_DS)] public string Sn { get; set; }
        [MR(2)] public byte[] Data { get; set; }
        [NotMapped] public IFormFile File { get; set; }
        [MR(3)] public RawUpload RawUpload { get; set; }
        [MR(4), DisplayName(_U), ForeignKey("RawUpload")] public int RawUploadId { get; set; }

        [MR(5), DisplayName(_A)] public ICollection<AnalysisDataModel> AnalysisDataModels { get; set; }
        [NotMapped] public string Analyses
        {
            get
            {
                var sb = new StringBuilder();
                foreach (var ad in AnalysisDataModels) sb.Append($"{ad.Analysis.Description}, ");
                if(sb.Length >= 2) sb.Remove(sb.Length - 2, 2);
                return sb.ToString();
            }
        }

    }
}
#pragma warning restore CS8618 // null을 허용하지 않는 필드가 초기화되지 않았습니다. nullable로 선언하는 것이 좋습니다.
