using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Expression2Sql.Attributes
{
    /// <summary>
    /// 备注
    /// </summary>
    public class RemarkAttribute :Attribute
    {
        public string remark { get; set; }

        public RemarkAttribute(string remark)
        {
            this.remark = remark;
        }
    }
}
