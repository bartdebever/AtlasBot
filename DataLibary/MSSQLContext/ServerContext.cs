using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLibary.Data;

namespace DataLibary.MSSQLContext
{
    public class ServerContext : IServerContext
    {
        public void AddServer(ulong serverid, ulong ownerid, string servername, string key)
        {
            string query = "INSERT INTO [Server] VALUES(@serverid, null, @ownerid, @servername, null, @key, 0)";
            SqlCommand cmd = new SqlCommand(query, Database.Connection());
            cmd.Parameters.AddWithValue("@serverid", Convert.ToInt64(serverid));
            cmd.Parameters.AddWithValue("@ownerid", Convert.ToInt64(ownerid));
            cmd.Parameters.AddWithValue("@servername", servername);
            cmd.Parameters.AddWithValue("@key", key);
            cmd.ExecuteNonQuery();
        }
        public void VerifyServerSQL(ulong userid, string key)
        {
            string query = "UPDATE [Server] SET Verified = 1 WHERE VerificationKey = @key AND OwnerId = @userid";
            SqlCommand cmd = new SqlCommand(query, Database.Connection());
            cmd.Parameters.AddWithValue("@key", key);
            cmd.Parameters.AddWithValue("@userid", Convert.ToInt64(userid));
            cmd.ExecuteNonQuery();
        }

        public void AddInviteLink(ulong userid, ulong serverid, string key)
        {
            if (IsAdmin(userid, serverid) == true)
            {
                string query = "UPDATE [Server] SET InviteLink = @link WHERE DiscordServerId = @Id";
                SqlCommand cmd = new SqlCommand(query, Database.Connection());
                cmd.Parameters.AddWithValue("@Id", Convert.ToInt64(serverid));
                cmd.Parameters.AddWithValue("@link", key);
                cmd.ExecuteNonQuery();
            }
            
        }
        public bool IsAdmin(ulong userid, ulong server)
        {
            string query =
                "SELECT CASE WHEN (EXISTS(SELECT [SA].DiscordId FROM [ServerAdmin] AS SA INNER JOIN [Server] S ON [S].id = [SA].serverid WHERE [S].DiscordServerId = @serverid AND [SA].discordid = @userid) OR EXISTS(SELECT [S].OwnerID FROM [Server] AS [S] WHERE @serverid = [S].DiscordServerId AND [S].OwnerId = @userid))THEN 1 ELSE 0 END;";
            SqlCommand cmd = new SqlCommand(query, Database.Connection());
            cmd.Parameters.AddWithValue("@userid", Convert.ToInt64(userid));
            cmd.Parameters.AddWithValue("@serverid", Convert.ToInt64(server));
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    return Convert.ToBoolean(reader.GetInt32(0));
                }
            }
            return false;
        }

        public List<ulong> GetAllServerIds()
        {
            List<ulong> result = new List<ulong>();
            string query = "SELECT DiscordServerId FROM [Server]";
            SqlCommand cmd = new SqlCommand(query, Database.Connection());
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    result.Add(Convert.ToUInt64(reader.GetInt64(0)));
                }
            }
            return result;
        }

        public bool IsServerVerified(ulong serverid)
        {
            string query = "SELECT [S].verified FROM [Server] S WHERE [S].DiscordServerId = @Id;";
            SqlCommand cmd = new SqlCommand(query, Database.Connection());
            cmd.Parameters.AddWithValue("@Id", Convert.ToInt64(serverid));
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    return reader.GetBoolean(0);
                }
            }
            return false;
        }

        public string GetServerDescription(ulong serverid)
        {
            string query = "SELECT [S].Description FROM [Server] S WHERE [S].DiscordServerId = @Id";
            SqlCommand cmd = new SqlCommand(query, Database.Connection());
            cmd.Parameters.AddWithValue("@Id", Convert.ToInt64(serverid));
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    return reader.GetString(0);
                }
            }
            return null;
        }

        public void SetServerDecription(ulong serverid, string description)
        {
            string query =
                "UPDATE [Server] SET [Server].Description = @description WHERE [Server].DiscordServerId = @Id";
            SqlCommand cmd = new SqlCommand(query, Database.Connection());
            cmd.Parameters.AddWithValue("@Id", Convert.ToInt64(serverid));
            cmd.Parameters.AddWithValue("@description", description);
            cmd.ExecuteNonQuery();
        }

        public string InviteLink(ulong serverid)
        {
            string query = "SELECT [S].InviteLink FROM [Server] S WHERE [S].DiscordServerId = @Id";
            SqlCommand cmd = new SqlCommand(query, Database.Connection());
            cmd.Parameters.AddWithValue("@Id", Convert.ToInt64(serverid));
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    return reader.GetString(0);
                }
            }
            return null;
        }

        public string ServerName(ulong serverid)
        {
            string query = "SELECT [S].ServerName FROM [Server] S WHERE [S].DiscordServerId = @Id";
            SqlCommand cmd = new SqlCommand(query, Database.Connection());
            cmd.Parameters.AddWithValue("@Id", Convert.ToInt64(serverid));
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    return reader.GetString(0);
                }
            }
            return null;
        }

        public void SetServerName(ulong serverid, string name)
        {
            string query = "UPDATE [Server] SET ServerName = @Name WHERE DiscordServerId = @Id";
            SqlCommand cmd = new SqlCommand(query, Database.Connection());
            cmd.Parameters.AddWithValue("@Id", Convert.ToInt64(serverid));
            cmd.Parameters.AddWithValue("@Name", name);
            cmd.ExecuteNonQuery();
        }

        public void AddAdmin(ulong userid, ulong serverid)
        {
            string query = "INSERT INTO [ServerAdmin] VALUES (@User, (SELECT [S].Id FROM [Server] S WHERE [S].DiscordServerId = @Server))";
            SqlCommand cmd = new SqlCommand(query, Database.Connection());
            cmd.Parameters.AddWithValue("@Server", Convert.ToInt64(serverid));
            cmd.Parameters.AddWithValue("@User", Convert.ToInt64(userid));
            cmd.ExecuteNonQuery();
        }

        public List<string> ListAdmins(ulong serverid)
        {
            List<string> result = new List<string>();
            string query =
                "SELECT [SA].DiscordId FROM [ServerAdmin] SA WHERE [SA].ServerId = (SELECT [S].id FROM [server] s WHERE [S].DiscordServerId = @serverid)";
            SqlCommand cmd = new SqlCommand(query, Database.Connection());
            cmd.Parameters.AddWithValue("@serverid", Convert.ToInt64(serverid));
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    result.Add(reader.GetInt64(0).ToString());
                }
            }
            return result;
        }

        public void RemoveAdmin(ulong userid, ulong serverid)
        {
            string query =
                "DELETE [ServerAdmin] WHERE DiscordId = @userid AND (SELECT [S].Id FROM [Server] S WHERE [S].DiscordServerId = @serverid)";
            SqlCommand cmd = new SqlCommand(query, Database.Connection());
            cmd.Parameters.AddWithValue("@userid", Convert.ToInt64(userid));
            cmd.Parameters.AddWithValue("@serverid", Convert.ToInt64(serverid));
            cmd.ExecuteNonQuery();
        }

        public DateTime GetLastRefreshDate(ulong serverid)
        {
            string query = "SELECT [S].LastUpdate FROM [Server] S WHERE [S].DiscordServerId = @serverid";
            SqlCommand cmd = new SqlCommand(query, Database.Connection());
            cmd.Parameters.AddWithValue("@serverid", Convert.ToInt64(serverid));
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    try
                    {
                        return Convert.ToDateTime(reader.GetDateTime(0));
                    }
                    catch
                    {
                        
                    }
                    
                }
            }
            return new DateTime(1998, 2, 13);
        }

        public void SetLastRefreshDate(ulong serverid, DateTime date)
        {
            string query = "UPDATE [Server] SET LastUpdate = @Date WHERE DiscordServerId = @serverid";
            SqlCommand cmd = new SqlCommand(query, Database.Connection());
            cmd.Parameters.AddWithValue("@Date", date);
            cmd.Parameters.AddWithValue("@serverid", Convert.ToInt64(serverid));
            cmd.ExecuteNonQuery();
        }
    }
}
