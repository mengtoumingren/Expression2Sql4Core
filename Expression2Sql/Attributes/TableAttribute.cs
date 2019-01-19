using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Expression2Sql.Attributes
{
    public class TableAttribute : Attribute
    {
        public string Name { get; set; }
        public TableAttribute(string name)
        {
            this.Name = name;
        }
    }
}
