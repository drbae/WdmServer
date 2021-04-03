using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Universe.Web.Data;
using System.ComponentModel;
#pragma warning disable CS8618 // null을 허용하지 않는 필드가 초기화되지 않았습니다. nullable로 선언하는 것이 좋습니다.

namespace DrBAE.WdmServer.Models
{
    using MR = ModelRecordAttribute;
    /// <summary>
    /// DUT 분석 데이터의 기본 모델
    /// </summary>
    public class DutAnalysisModelBase : ModelBase<DutAnalysisModelBase>
    {
        public const string pAnalysisData = nameof(AnalysisData);
        const string _AD = "Analysis Data";

        [MR(1), DisplayName(_AD)] public virtual AnalysisDataModel AnalysisData { get; set; }
        [MR(2), DisplayName(_AD), ForeignKey(pAnalysisData)] public int AnalysisDataId { get; set; }

        //
        public new T GetValue<T>(string propName)// where T: struct
        {            
            return base.GetValue<T>(propName);
        }
        
        public override IList<object> GetValues()
        {
            return base.GetValues();
        }

        //public new void SetValue<T>(string propName, T value)
        //{

        //}
        //public override void SetValues(in IEnumerable<object> values)
        //{
        //    base.SetValues(values);
        //}
    }
}
#pragma warning restore CS8618 // null을 허용하지 않는 필드가 초기화되지 않았습니다. nullable로 선언하는 것이 좋습니다.
