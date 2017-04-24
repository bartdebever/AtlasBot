using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLibary.Data;
using DataLibary.Models;

namespace DataLibary.MSSQLContext
{
    public class RoleContext : IRoleContext
    {
        public Role GetRoleById(int id)
        {
            string query =
                "SELECT [R].full FROM [Role] AS [R] WHERE [R].id = @Id";
            SqlCommand cmd = new SqlCommand(query, Database.Connection());
            cmd.Parameters.AddWithValue("@Id", id);
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    return (CreateRoleFromReader(reader));
                }
            }
            throw new Exception("Role not found");
        }

        public List<Role> GetAllRoles()
        {
            List<Role> result = new List<Role>();
            string query =
                "SELECT[R].Short AS[Role], [R].Long AS[Full] FROM[Role] AS R UNION SELECT[R].Long AS[Role], [R].Long AS[Full] FROM[Role] AS R UNION SELECT[R].Alt AS[Role], [R].Long AS[Full] FROM[Role] AS R";
            SqlCommand cmd = new SqlCommand(query, Database.Connection());
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    result.Add(CreateRoleFromReader(reader));
                }
            }
            return result;
        }

        public List<Role> GetRolesPerServer(Server server)
        {
            throw new NotImplementedException();
        }

        public Role CreateRoleFromReader(SqlDataReader reader)
        {
            foreach (Role value in Enum.GetValues(typeof(Role)))
            {
                if (value.ToString().ToLower() == reader["[Full]"].ToString())
                {
                    return value;
                }
            }
            throw new Exception("Role not found");
        }
    }
}
