using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using DataLibary.Data;
using DataLibary.Models;
using RiotSharp;
using ToolKit;
using CommandType = System.Data.CommandType;

namespace DataLibary.MSSQLContext
{
    public class UserContext : IUserContext
    {
        public User GetUserById(int id)
        {
            string query =
                "SELECT [u].id as userid, [u].discordid, [r].short, [s].riotid FROM [User] AS [u] INNER JOIN [Summoner] AS [s] ON [s].UserId = [u].id INNER JOIN [Region] AS [r] ON [r].id = [s].regionid WHERE [u].id = @Id";
            SqlCommand cmd = new SqlCommand(query, Database.Connection());
            cmd.Parameters.AddWithValue("@Id", id);
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    return CreateUserFromReader(reader);
                }
            }
            throw new Exception("User not found");
        }

        public List<User> GetUsersByRegion(Region region)
        {
            List<User> result = new List<User>();
            string query =
                "SELECT [u].id as userid, [u].discordid, [r].short, [s].riotid FROM [User] AS [u] INNER JOIN [Summoner] AS [s] ON [s].UserId = [u].id INNER JOIN [Region] AS [r] ON [r].id = [s].regionid WHERE [r].short = @Region";
            SqlCommand cmd = new SqlCommand(query, Database.Connection());
            cmd.Parameters.AddWithValue("@Region", region.ToString().ToLower());
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                     CreateUserFromReader(reader);
                }
            }
            return result;
        }

        public void AddUser(Int64 userid)
        {
            string query1 = "INSERT INTO [User] VALUES(@DiscordId)";
            SqlCommand cmd1 =  new SqlCommand(query1, Database.Connection());
            cmd1.Parameters.AddWithValue("@DiscordId", userid);
            cmd1.ExecuteNonQuery();
        }

        public void RemoveUser(User user)
        {
            string query1 = "REMOVE [Summoner] WHERE Id = @UserId";
            SqlCommand cmd1 = new SqlCommand(query1, Database.Connection());
            cmd1.Parameters.AddWithValue("@UserId", user.Id);
            cmd1.ExecuteNonQuery();
            string query2 = "REMOVE [User] WHERE Id = @Id";
            SqlCommand cmd2 = new SqlCommand(query2, Database.Connection());
            cmd2.Parameters.AddWithValue("@Id", user.Id);
            cmd2.ExecuteNonQuery();
        }

        public User GetUserByDiscord(ulong discordid)
        {
            string query =
                "SELECT [u].id as userid, [u].discordid, [r].short, [s].riotid FROM [User] AS [u] INNER JOIN [Summoner] AS [s] ON [s].UserId = [u].id INNER JOIN [Region] AS [r] ON [r].id = [s].regionid WHERE [u].discordid = @Id";
            SqlCommand cmd = new SqlCommand(query, Database.Connection());
            cmd.Parameters.AddWithValue("@Id", Convert.ToInt64(discordid));
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    return CreateUserFromReader(reader);
                }
            }
            throw new Exception("User not found");
        }

        public int GetUserIdByDiscord(ulong discordid)
        {
            string query =
                "SELECT [u].id FROM [User] AS [u]WHERE [u].discordid = @Id";
            SqlCommand cmd = new SqlCommand(query, Database.Connection());
            cmd.Parameters.AddWithValue("@Id", Convert.ToInt64(discordid));
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    return reader.GetInt32(0);
                }
            }
            throw new Exception("User not found");
        }
        public User GetUserByRiotid(int riotid)
        {
            string query =
                "SELECT [u].id as userid, [u].discordid, [r].short, [s].riotid FROM [User] AS [u] INNER JOIN [Summoner] AS [s] ON [s].UserId = [u].id INNER JOIN [Region] AS [r] ON [r].id = [s].regionid WHERE [s].riotid = @Id";
            SqlCommand cmd = new SqlCommand(query, Database.Connection());
            cmd.Parameters.AddWithValue("@Id", riotid);
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    return CreateUserFromReader(reader);
                }
            }
            throw new Exception("User not found");
        }

        public User CreateUserFromReader(SqlDataReader reader)
        {
            return new User(
                Convert.ToInt32(reader["userid"]),
                Convert.ToUInt64(reader["discordid"]),
                Convert.ToInt32(reader["riotid"]),
                LeagueAndDatabase.GetRegionFromString(reader["short"].ToString())
                );
        }

        public DateTime GetLastRefreshDate(ulong userid)
        {
            string query =
                "SELECT LastUpdate FROM [Summoner] S INNER JOIN [User] U ON [S].UserId = [U].Id WHERE [U].DiscordId = @userid";
            SqlCommand cmd = new SqlCommand(query, Database.Connection());
            cmd.Parameters.AddWithValue("@userid", Convert.ToInt64(userid));
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
    

        public void SetLastRefreshDate(ulong userid, DateTime date)
        {
        string query = "UPDATE [Summoner] SET LastUpdate = @Date WHERE UserId = (SELECT Id FROM [User] WHERE DiscordId = @userid)";
        SqlCommand cmd = new SqlCommand(query, Database.Connection());
        cmd.Parameters.AddWithValue("@Date", date);
        cmd.Parameters.AddWithValue("@userid", Convert.ToInt64(userid));
        cmd.ExecuteNonQuery();
        }

        public bool IsAtlasAdmin(ulong userid)
        {
            string query =
                "SELECT CASE WHEN (EXISTS(SELECT [AA].Id FROM [AtlasAdmin] AA INNER JOIN [User] U ON [U].id = [AA].UserId WHERE [U].DiscordId = @userid)) THEN 1 ELSE 0 END";
            SqlCommand cmd = new SqlCommand(query, Database.Connection());
            cmd.Parameters.AddWithValue("userid", Convert.ToInt64(userid));
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    return Convert.ToBoolean(reader.GetInt32(0));
                }
            }
            return false;
        }

        public List<User> GetAllAccounts(ulong userid)
        {
            List<User> result  = new List<User>();
            string query = "[dbo].[GetAccounts]";
            SqlCommand cmd = new SqlCommand(query, Database.Connection());
            cmd.Parameters.AddWithValue("@DiscordId", Convert.ToInt64(userid));
            cmd.CommandType = CommandType.StoredProcedure;
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    result.Add(new User(0, userid, Convert.ToInt32(reader["SummonerId"]), LeagueAndDatabase.GetRegionFromDatabaseId(Convert.ToInt32(reader["Region"])), Convert.ToBoolean(Convert.ToInt64(reader["Type"]))));
                }
            }
            return result;
        }

        public string GetBackupName(ulong discordid)
        {
            string query = "SELECT Tag FROM [User] WHERE DiscordId = @Id";
            SqlCommand cmd = new SqlCommand(query, Database.Connection());
            cmd.Parameters.AddWithValue("@id", Convert.ToInt64(discordid));
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    return reader.GetString(0);
                }
            }
            return null;
        }
    }
}
