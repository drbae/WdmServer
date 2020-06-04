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
    public class AnalysisRawUpload : ModelBase<AnalysisRawUpload>
    {
        public const string pAnalysisId = nameof(AnalysisId);
        public const string pRawUploadId = nameof(RawUploadId);

        static AnalysisRawUpload()
        {
            PropNames.AddRange(new[] { pAnalysisId, pRawUploadId });
            DisplayNames.AddRange(new[] { pAnalysisId, pRawUploadId });
        }

        [ForeignKey(pAnalysisId)] public AnalysisModel Analysis { get; set; }
        public int AnalysisId { get; set; }

        [ForeignKey(pRawUploadId)] public RawUpload RawUpload { get; set; }
        public int RawUploadId { get; set; }        

    }
}
#pragma warning restore CS8618 // null을 허용하지 않는 필드가 초기화되지 않았습니다. nullable로 선언하는 것이 좋습니다.
