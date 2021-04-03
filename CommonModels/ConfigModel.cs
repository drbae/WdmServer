using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Universe.Web.Data;
using System;
using System.ComponentModel;
#pragma warning disable CS8618

namespace DrBAE.WdmServer.Models
{
    using CT = ConfigType;
    using MR = ModelRecordAttribute;
    public class ConfigModel : ModelBase<ConfigModel>
    {
        public const string pName = nameof(Name);
        public const string pDesc = nameof(Description);
        public const string pContent = nameof(Content);
        public const string pFile = nameof(File);

        [MR(1), EnumDataType(typeof(CT)), DisplayName("Config Type")] public CT ConfigType { get; set; }
        [MR(2)] public string Name { get; set; }//파일이름, 생성시 자동, 수정가능?
        [MR(3)] public string Content { get; set; }//파일내용, 생성시 자동, 이후 변경 불가
        [NotMapped] public IFormFile File { get; set; }
        [MR(4)] public bool Pol { get; set; }
        [MR(5), DisplayName("Num CHs")]public int NumCh { get; set; }
        [MR(6)] public string Description { get; set; }//사용자 제공, 수정가능

    }//class
}
#pragma warning restore CS8618 // null을 허용하지 않는 필드가 초기화되지 않았습니다. nullable로 선언하는 것이 좋습니다.
