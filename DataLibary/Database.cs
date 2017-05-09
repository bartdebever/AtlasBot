using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLibary
{
    public static class Database
    {
        public static SqlConnection Connection()
        {
            SqlConnection conn = new SqlConnection("Data Source=desktop-bort;Initial Catalog=Atlas;Integrated Security=True;Max Pool Size=99999;");
            conn.Open();
            return conn;
        }
    }
}
