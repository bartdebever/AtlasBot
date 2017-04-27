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
            throw new NotImplementedException();
        }

        public bool RegionByAccount(ulong serverid)
        {
            throw new NotImplementedException();
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
    }
}
