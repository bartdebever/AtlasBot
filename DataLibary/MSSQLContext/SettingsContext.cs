using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DataLibary.Data;
using CommandType = DataLibary.Models.CommandType;

namespace DataLibary.MSSQLContext
{
    public class SettingsContext :ISettingsContext
    {
        public bool RankByParameter(ulong serverid)
        {
            string query =
               "SELECT [SS].RankCommand FROM [ServerSettings] SS INNER JOIN [Server] S ON [SS].serverid = [S].id WHERE [S].DiscordServerId = @Id";
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
        public bool RankByAccount(ulong serverid)
        {
            string query =
                "SELECT [SS].RankAccountCommand FROM [ServerSettings] SS INNER JOIN [Server] S ON [SS].serverid = [S].id WHERE [S].DiscordServerId = @Id";
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

        public bool RegionByParameter(ulong serverid)
        {
            string query =
               "SELECT [SS].RegionCommand FROM [ServerSettings] SS INNER JOIN [Server] S ON [SS].serverid = [S].id WHERE [S].DiscordServerId = @Id";
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

        public bool RegionByAccount(ulong serverid)
        {
            string query =
               "SELECT [SS].RegionAccountCommand FROM [ServerSettings] SS INNER JOIN [Server] S ON [SS].serverid = [S].id WHERE [S].DiscordServerId = @Id";
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

        public bool MasteryPointsByAccount(ulong serverid)
        {
            throw new NotImplementedException();
        }

        public bool MasteryLevelByAccount(ulong serverid)
        {
            throw new NotImplementedException();
        }

        public CommandType RankCommandType(ulong serverid)
        {
            string query =
                "SELECT RankCommandType FROM [ServerSettings] SS INNER JOIN [Server] S ON [S].id = [SS].serverid WHERE [S].DiscordServerId = @Id";
            SqlCommand cmd = new SqlCommand(query, Database.Connection());
            cmd.Parameters.AddWithValue("@Id", Convert.ToInt64(serverid));
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    return (CommandType) reader.GetInt32(0);
                }
            }
            throw new Exception("Server not found");
        }

        public void SetRankType(CommandType type, ulong serverid)
        {
            string query =
                "UPDATE [ServerSettings] SET [ServerSettings].RankCommandType = @Type FROM [ServerSettings] INNER JOIN [Server] S ON [S].id = [ServerSettings].serverid WHERE [S].DiscordServerId = @Id";
            SqlCommand cmd = new SqlCommand(query, Database.Connection());
            cmd.Parameters.AddWithValue("@Id", Convert.ToInt64(serverid));
            cmd.Parameters.AddWithValue("@Type", (int) type);
            cmd.ExecuteNonQuery();
        }

        public void AllowRankAccount(bool value, ulong serverid)
        {
            string query =
                "UPDATE [ServerSettings] SET [ServerSettings].RankAccountCommand = @Value FROM [ServerSettings] INNER JOIN [Server] S ON [S].id = [ServerSettings].serverid WHERE [S].DiscordServerId = @Id";
            SqlCommand cmd = new SqlCommand(query, Database.Connection());
            cmd.Parameters.AddWithValue("@Value", value);
            cmd.Parameters.AddWithValue("@Id", Convert.ToInt64(serverid));
            cmd.ExecuteNonQuery();
        }

        public ulong GetOverride(string parameter, ulong serverid)
        {
            string query =
                "SELECT [RO].DiscordRoleId FROM [RoleOverride] AS RO INNER JOIN [Server] S ON [RO].ServerId = [S].Id WHERE [S].DiscordServerId = @Id AND [RO].parameter = @Parameter";
            SqlCommand cmd = new SqlCommand(query, Database.Connection());
            cmd.Parameters.AddWithValue("@Id", Convert.ToInt64(serverid));
            cmd.Parameters.AddWithValue("@Parameter", parameter);
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    return Convert.ToUInt64(reader.GetInt64(0));
                }
            }
            return 0;
        }

        public void AddOverride(string parameter, ulong rank, ulong serverid)
        {
            string query =
                "INSERT INTO [RoleOverride] VALUES((SELECT [S].id FROM [server] S WHERE [S].DiscordServerId = @ServerId), @Parameter, @rank)";
            SqlCommand cmd = new SqlCommand(query, Database.Connection());
            cmd.Parameters.AddWithValue("@ServerId", Convert.ToInt64(serverid));
            cmd.Parameters.AddWithValue("@Parameter", parameter);
            cmd.Parameters.AddWithValue("@rank", Convert.ToInt64(rank));
            cmd.ExecuteNonQuery();
        }

        public List<string> GetAllOverridesInformation(ulong serverid)
        {
            List<string> returnstring = new List<string>();
            string query =
                "SELECT [RO].id, [RO].DiscordRoleId, [RO].Parameter FROM [RoleOverride] RO INNER JOIN [Server] S ON [S].id = [RO].ServerId WHERE [S].DiscordServerId = @Id";
            SqlCommand cmd = new SqlCommand(query, Database.Connection());
            cmd.Parameters.AddWithValue("@Id", Convert.ToInt64(serverid));
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    returnstring.Add("Id: " + reader.GetInt32(0) + " Replaces: " + reader.GetString(2) + " For role:" +
                                    reader.GetInt64(1));
                }
            }
            return returnstring;
        }

        public void RemoveOverride(int id, ulong serverid)
        {
            string query = "DELETE [RoleOverride] FROM [RoleOverride] RO INNER JOIN [Server] S ON [RO].ServerId = [S].Id WHERE [S].DiscordServerId = @Serverid AND [RO].Id = @Overrideid";
            SqlCommand cmd = new SqlCommand(query, Database.Connection());
            cmd.Parameters.AddWithValue("@Serverid", Convert.ToInt64(serverid));
            cmd.Parameters.AddWithValue("@Overrideid", id);
            cmd.ExecuteNonQuery();
        }

        public List<string> GetAllOverrides(ulong serverid)
        {
            List<string> returnstring = new List<string>();
            string query =
                "SELECT [RO].id, [RO].DiscordRoleId, [RO].Parameter FROM [RoleOverride] RO INNER JOIN [Server] S ON [S].id = [RO].ServerId WHERE [S].DiscordServerId = @Id";
            SqlCommand cmd = new SqlCommand(query, Database.Connection());
            cmd.Parameters.AddWithValue("@Id", Convert.ToInt64(serverid));
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    returnstring.Add(reader.GetString(2)+":"+reader.GetInt64(1).ToString());
                }
            }
            return returnstring;
        }

        public void CreateSettings(ulong serverid)
        {
            string query =
                "INSERT INTO [ServerSettings] VALUES ((SELECT [S].Id as serverid FROM  [Server] S  WHERE [S].DiscordServerId = @Id), 0, NULL, 0, 1, 0, 0, NULL, 0, 0, 1, 0,0,0, NULL)";
            SqlCommand cmd = new SqlCommand(query, Database.Connection());
            cmd.Parameters.AddWithValue("@Id", Convert.ToInt64(serverid));
            cmd.ExecuteNonQuery();
        }

        public void AllowRankParameter(bool value, ulong serverid)
        {
            string query =
                "UPDATE [ServerSettings] SET [ServerSettings].RankCommand = @Value FROM [ServerSettings] INNER JOIN [Server] S ON [S].id = [ServerSettings].serverid WHERE [S].DiscordServerId = @Id";
            SqlCommand cmd = new SqlCommand(query, Database.Connection());
            cmd.Parameters.AddWithValue("@Value", value);
            cmd.Parameters.AddWithValue("@Id", Convert.ToInt64(serverid));
            cmd.ExecuteNonQuery();
        }

        public void ChangeRegionAccount(bool value, ulong serverid)
        {
            string query =
                "UPDATE [ServerSettings] SET [ServerSettings].RegionAccountCommand = @Value FROM [ServerSettings] INNER JOIN [Server] S ON [S].id = [ServerSettings].serverid WHERE [S].DiscordServerId = @Id";
            SqlCommand cmd = new SqlCommand(query, Database.Connection());
            cmd.Parameters.AddWithValue("@Value", value);
            cmd.Parameters.AddWithValue("@Id", Convert.ToInt64(serverid));
            cmd.ExecuteNonQuery();
        }

        public void ChangeRegionParameter(bool value, ulong serverid)
        {
            string query =
                "UPDATE [ServerSettings] SET [ServerSettings].RegionCommand = @Value FROM [ServerSettings] INNER JOIN [Server] S ON [S].id = [ServerSettings].serverid WHERE [S].DiscordServerId = @Id";
            SqlCommand cmd = new SqlCommand(query, Database.Connection());
            cmd.Parameters.AddWithValue("@Value", value);
            cmd.Parameters.AddWithValue("@Id", Convert.ToInt64(serverid));
            cmd.ExecuteNonQuery();
        }

        public List<string> GetDisabledRoles(ulong serverid)
        {
            List<string> result = new List<string>();
            string query =
                "SELECT [DR].Id, [DR].Parameter FROM [RoleDisable] DR INNER JOIN [Server] S ON [S].id = [DR].Serverid WHERE [S].DiscordServerId = @Id";
            SqlCommand cmd = new SqlCommand(query, Database.Connection());
            cmd.Parameters.AddWithValue("@Id", Convert.ToInt64(serverid));
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    result.Add("Id: " + reader.GetInt32(0)+ " disables " + reader.GetString(1));
                }
            }
            return result;
        }

        public bool IsRoleDisabled(string parameter, ulong serverid)
        {
            string query =
                "SELECT CASE WHEN EXISTS (SELECT [RD].Id FROM [RoleDisable] RD INNER JOIN [Server] S ON [RD].Serverid = [S].Id WHERE [RD].parameter = @parameter AND [S].DiscordServerId = @id) THEN 1 ELSE 0 END;";
            SqlCommand cmd = new SqlCommand(query, Database.Connection());
            cmd.Parameters.AddWithValue("@parameter", parameter);
            cmd.Parameters.AddWithValue("@Id", Convert.ToInt64(serverid));
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    return Convert.ToBoolean(reader.GetInt32(0));
                }
            }
            return false;
        }

        public void AddRoleDisable(string parameter, ulong serverid)
        {
            string query =
                "INSERT INTO [RoleDisable] VALUES((SELECT [S].id FROM [Server] S WHERE [S].DiscordServerId = @Id), @parameter)";
            SqlCommand cmd = new SqlCommand(query, Database.Connection());
            cmd.Parameters.AddWithValue("@parameter", parameter);
            cmd.Parameters.AddWithValue("@Id", Convert.ToInt64(serverid));
            cmd.ExecuteNonQuery();
        }

        public void RemoveRoleDisable(int id, ulong serverid)
        {
            string query =
                "DELETE [RoleDisable] WHERE [RoleDisable].Id = @Id AND [RoleDisable].ServerId = (SELECT [S].id FROM [Server] S WHERE [S].DiscordServerId = @ServerId);";
            SqlCommand cmd = new SqlCommand(query, Database.Connection());
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Parameters.AddWithValue("@ServerId", Convert.ToInt64(serverid));
            cmd.ExecuteNonQuery();
        }

        public bool RoleByParameter(ulong serverid)
        {
            string query =
              "SELECT [SS].RoleCommand FROM [ServerSettings] SS INNER JOIN [Server] S ON [SS].serverid = [S].id WHERE [S].DiscordServerId = @Id";
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

        public bool RoleByAccount(ulong serverid)
        {
            string query =
              "SELECT [SS].RoleAccountCommand FROM [ServerSettings] SS INNER JOIN [Server] S ON [SS].serverid = [S].id WHERE [S].DiscordServerId = @Id";
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

        public void ChangeRoleAccount(bool value, ulong serverid)
        {
            string query =
                "UPDATE [ServerSettings] SET [ServerSettings].RoleAccountCommand = @Value FROM [ServerSettings] INNER JOIN [Server] S ON [S].id = [ServerSettings].serverid WHERE [S].DiscordServerId = @Id";
            SqlCommand cmd = new SqlCommand(query, Database.Connection());
            cmd.Parameters.AddWithValue("@Value", value);
            cmd.Parameters.AddWithValue("@Id", Convert.ToInt64(serverid));
            cmd.ExecuteNonQuery();
        }

        public void ChangeRoleParameter(bool value, ulong serverid)
        {
            string query =
                "UPDATE [ServerSettings] SET [ServerSettings].RoleParameterCommand = @Value FROM [ServerSettings] INNER JOIN [Server] S ON [S].id = [ServerSettings].serverid WHERE [S].DiscordServerId = @Id";
            SqlCommand cmd = new SqlCommand(query, Database.Connection());
            cmd.Parameters.AddWithValue("@Value", value);
            cmd.Parameters.AddWithValue("@Id", Convert.ToInt64(serverid));
            cmd.ExecuteNonQuery();
        }

        public CommandType RoleCommandType(ulong serverid)
        {
            string query =
                "SELECT RoleCommandType FROM [ServerSettings] SS INNER JOIN [Server] S ON [S].id = [SS].serverid WHERE [S].DiscordServerId = @Id";
            SqlCommand cmd = new SqlCommand(query, Database.Connection());
            cmd.Parameters.AddWithValue("@Id", Convert.ToInt64(serverid));
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    return (CommandType)reader.GetInt32(0);
                }
            }
            throw new Exception("Server not found");
        }

        public void SetRoleType(CommandType type, ulong serverid)
        {
            string query =
                "UPDATE [ServerSettings] SET [ServerSettings].RoleCommandType = @Type FROM [ServerSettings] INNER JOIN [Server] S ON [S].id = [ServerSettings].serverid WHERE [S].DiscordServerId = @Id";
            SqlCommand cmd = new SqlCommand(query, Database.Connection());
            cmd.Parameters.AddWithValue("@Id", Convert.ToInt64(serverid));
            cmd.Parameters.AddWithValue("@Type", (int)type);
            cmd.ExecuteNonQuery();
        }
    }
}
