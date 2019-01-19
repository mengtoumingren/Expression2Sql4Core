using Expression2Sql.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Expression2SqlTest
{
    [Table("tableuserinfo")]
    class UserInfo
    {
        [Field("sysid")]
        public int Id { get; set; }
        public int Sex { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }
}
