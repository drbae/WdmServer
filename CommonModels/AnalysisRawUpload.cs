using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Universe.Web.Data;
#pragma warning disable CS8618 // null�� ������� �ʴ� �ʵ尡 �ʱ�ȭ���� �ʾҽ��ϴ�. nullable�� �����ϴ� ���� �����ϴ�.

namespace DrBAE.WdmServer.Models
{
    using MR = ModelRecordAttribute;
    public class AnalysisRawUpload : ModelBase<AnalysisRawUpload>
    {
        public const string pAnalysisId = nameof(AnalysisId);
        public const string pRawUploadId = nameof(RawUploadId);

        [MR(1), ForeignKey(pAnalysisId)] public AnalysisModel Analysis { get; set; }
        [MR(2)]public int AnalysisId { get; set; }

        [MR(3), ForeignKey(pRawUploadId)] public RawUpload RawUpload { get; set; }
        [MR(4)] public int RawUploadId { get; set; }        

    }
}
#pragma warning restore CS8618 // null�� ������� �ʴ� �ʵ尡 �ʱ�ȭ���� �ʾҽ��ϴ�. nullable�� �����ϴ� ���� �����ϴ�.
