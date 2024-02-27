using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WinFormsApp1
{
    
    public class SqlSugarConfig
    {
        private static readonly string connectionString = "Data Source=localhost;Database=iimp_jv;User Id=root;Password=root;charset=utf8;port=3306";

        public static SqlSugarClient GetInstance()
        {
            var db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = "Data Source=ZEROTPC003\\MSSQLSERVER2022;Initial Catalog=MyFristDB;Persist Security Info=True;User ID=sa;Password=sa123456",
                DbType = DbType.SqlServer,
                IsAutoCloseConnection = true,
                InitKeyType = InitKeyType.Attribute
            });

            return db;
        }
    }

}
