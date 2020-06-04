using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Universe.Web.Data;
using System;
using System.ComponentModel;
#pragma warning disable CS8618 // null을 허용하지 않는 필드가 초기화되지 않았습니다. nullable로 선언하는 것이 좋습니다.

namespace DrBAE.WdmServer.Models
{
    using CT = ConfigType;
    public class ConfigModel : ModelBase<ConfigModel>
    {
        public const string pName = nameof(Name);
        public const string pDesc = nameof(Description);
        public const string pContent = nameof(Content);
        public const string pFile = nameof(File);

        const string _CT = "Config Type";
        const string _NC = "Num CHs";
        static ConfigModel()
        {
            //Id를 제외한 속성들 추가
            PropNames.AddRange(new[] { nameof(ConfigType), pName, pContent, nameof(Pol), nameof(NumCh), pDesc });
            DisplayNames.AddRange(new[] { _CT, pName, pContent, nameof(Pol), _NC, pDesc });
        }

        [EnumDataType(typeof(CT)), DisplayName(_CT)] public CT ConfigType { get; set; }
        public string Name { get; set; }//생성시 자동, 파일이름, 수정가능?
        public string Content { get; set; }//생성시 자동, 파일내용, 이후 변경 불가
        [NotMapped] public IFormFile File { get; set; }
        public bool Pol { get; set; }
        [DisplayName(_NC)]public int NumCh { get; set; }
        public string Description { get; set; }//사용자 제공, 수정가능

    }//class
}
#pragma warning restore CS8618 // null을 허용하지 않는 필드가 초기화되지 않았습니다. nullable로 선언하는 것이 좋습니다.
