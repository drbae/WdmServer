using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.ComponentModel;
using Universe.Web.Data;
#pragma warning disable CS8618

namespace DrBAE.WdmServer.Models
{
    using MR = ModelRecordAttribute;

    public class LotModel : ModelBase<LotModel>
    {
        public const string pName = nameof(LotName);

        [MR(1), DisplayName("LOT Name")] public string LotName { get; set; }
        [MR(2), DisplayName("LOT Date"), DataType(DataType.DateTime)] public DateTime LotDate { get; set; } = DateTime.Now;

    }//class
}
#pragma warning restore CS8618
