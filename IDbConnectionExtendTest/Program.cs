using MySql.Data.MySqlClient;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using IDbConnectionExtend;
using Expression2Sql.Attributes;

namespace IDbConnectionExtendTest
{
    class Program
    {
        /*
           CREATE TABLE `messageinfo` (
          `Id` int(11) NOT NULL AUTO_INCREMENT,
          `PhoneNumber` varchar(16) DEFAULT NULL,
          `MsgDate` datetime DEFAULT NULL,
          `IsNew` tinyint(1) DEFAULT NULL,
          `MsgContent` varchar(255) DEFAULT NULL,
          PRIMARY KEY (`Id`),
          FULLTEXT KEY `PhoneNumber` (`PhoneNumber`)
          ) ENGINE=MyISAM AUTO_INCREMENT=15 DEFAULT CHARSET=latin1;
         */
        class MsgModel
        {
            public int Id { get; set; }
            public string PhoneNumber { get; set; }
            public string MsgDate { get; set; }
            public string MsgContent { get; set; }
        }

        [Table("messageinfo")]
        class MsgModel2
        {
            public int Id { get; set; }
            [Field("PhoneNumber")]//字段映射
            public string PhoneNum { get; set; }
            public string MsgDate { get; set; }
            public string MsgContent { get; set; }
        }

        class messageinfo
        {
            public int Id { get; set; }
            public string PhoneNumber { get; set; }
            public string MsgDate { get; set; }
            public string MsgContent { get; set; }
        }

        [Table("userinfo")]
        class User
        {
            public int Id { get; set; }
            [Field("UserName")]//字段映射
            public string uid { get; set; }
            [Field("Password")]//字段映射
            public string pwd { get; set; }
        }

        static void Main(string[] args)
        {
            //自动 open
            using (var con = new MySqlConnection("server=localhost;port=3306;uid=root;pwd=admin;database=test"))
            {
                var count1 = con.Count<MsgModel2>();
                var msgs2 = con.Query<MsgModel2>(m => m.Id ==10);
                var msg = msgs2.FirstOrDefault();
                msg.PhoneNum = "sdfsdfdfsdf";
                con.Add(msg);
                msg.PhoneNum = "13344444444";
                con.Update(msg);
                con.Delete<MsgModel2>(m => m.Id == 10);


                //事务 成功时返回 true， 失败时 返回 false 会调用回滚
                con.Transaction(cmd =>
                {
                    var count = cmd.Delete<MsgModel2>(m => m.Id == 10);
                    if (count == 1)
                        return true;
                    return false;
                });
            }
            Console.ReadLine();
        }
    }
}
