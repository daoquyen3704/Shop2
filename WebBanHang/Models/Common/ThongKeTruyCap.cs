using Dapper;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;

namespace WebBanHang.Models.Common
{
    public class ThongKeTruyCap
    {
        private static string strConnect = ConfigurationManager.ConnectionStrings["PostgresConnection"].ToString();

        public static ThongKeViewModel ThongKe()
        {
            using (var connect = new NpgsqlConnection(strConnect))
            {
                var item = connect.QueryFirstOrDefault<ThongKeViewModel>("sp_thongke", commandType: CommandType.StoredProcedure);
                return item;
            }
        }
    }
}