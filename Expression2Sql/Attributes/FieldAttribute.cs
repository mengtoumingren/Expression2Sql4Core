using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Expression2Sql.Attributes
{
    public class FieldAttribute : Attribute
    {
        public string Name { get; set; }
        public FieldAttribute(string name)
        {
            this.Name = name;
        }
    }
}
