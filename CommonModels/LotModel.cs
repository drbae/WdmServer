using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.ComponentModel;
using Universe.Web.Data;

namespace DrBAE.WdmServer.Models
{
    public class LotModel : ModelBase<LotModel>
    {
        public const string pName = nameof(LotName);
        const string _LN = "LOT Name";
        const string _LD = "LOT Date";
        static LotModel()
        {
            PropNames.AddRange(new[] { nameof(LotName), nameof(LotDate) });
            DisplayNames.AddRange(new[] { _LN, _LD });
        }

#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        [DisplayName(_LN)] public string LotName { get; set; }
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        [DisplayName(_LD), DataType(DataType.DateTime)] public DateTime LotDate { get; set; } = DateTime.Now;

    }//class
}
