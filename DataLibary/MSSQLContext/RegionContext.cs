using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLibary.Data;
using DataLibary.Models;
using RiotSharp;

namespace DataLibary.MSSQLContext
{
    public class RegionContext : IRegionContext
    {
        public int GetRegionId(Region region)
        {
            string query = "SELECT [r].id as rid FROM [Region] AS [r] WHERE [r].short = @Short";
            SqlCommand cmd = new SqlCommand(query, Database.Connection());
            cmd.Parameters.AddWithValue("@Short", region.ToString().ToLower());
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    return Convert.ToInt32(reader["rid"]);
                }
            }
            throw new Exception("Region not found");
        }

        public int GetRegionId(User user)
        {
            string query = "SELECT [S].RegionId FROM [User] INNER JOIN [Summoner] S ON [S].userid = [User].id WHERE [User].id = @Id";
            SqlCommand cmd = new SqlCommand(query, Database.Connection());
            cmd.Parameters.AddWithValue("@Id", user.Id);
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    return reader.GetInt32(0);
                }
            }
            throw new Exception("User or region not found");
        }
    }
}
