using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLibary.Data;

namespace DataLibary.MSSQLContext
{
    public class AltAccountContext : IAltAccountContext
    {
        public string GetToken(ulong userid)
        {
            string query = "SELECT token FROM [AltSummoner] INNER JOIN [User] ON [AltSummoner].UserId = [User].Id WHERE [User].DiscordId = @Userid";
            SqlCommand cmd = new SqlCommand(query, Database.Connection());
            cmd.Parameters.AddWithValue("@Userid", Convert.ToInt64(userid));
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    return reader.GetString(0);
                }
            }
            throw new Exception("Token not found");
        }

        public void AddAccount(ulong discordid, long riotid, int regionid, string token)
        {
            string query = "INSERT INTO AltSummoner VALUES ((SELECT ID FROM [User] WHERE DiscordId = @DiscordId), @RiotId, @RegionId, @Token, 0)";
            SqlCommand cmd = new SqlCommand(query, Database.Connection());
            cmd.Parameters.AddWithValue("@DiscordId", Convert.ToInt64(discordid));
            cmd.Parameters.AddWithValue("@RiotId", riotid);
            cmd.Parameters.AddWithValue("@RegionId", regionid);
            cmd.Parameters.AddWithValue("@Token", token);
            cmd.ExecuteNonQuery();
        }

        public void VerifyAccount(ulong userid)
        {
            string query = "UPDATE AltSummoner SET Verified = 1 WHERE UserId = (SELECT ID FROM [User] WHERE DiscordId = @DiscordId)";
            SqlCommand cmd = new SqlCommand(query, Database.Connection());
            cmd.Parameters.AddWithValue("@DiscordId", Convert.ToInt64(userid));
            cmd.ExecuteNonQuery();
        }

        public bool UniverifiedAccount(ulong userid)
        {
            string query =
                "SELECT CASE WHEN EXISTS (SELECT Token FROM AltSummoner INNER JOIN [User] ON [User].Id = [AltSummoner].UserId WHERE [User].DiscordId = @DiscordId AND Verified = 0 ) THEN 1 ELSE 0 END;";
            SqlCommand cmd = new SqlCommand(query, Database.Connection());
            cmd.Parameters.AddWithValue("@DiscordId", Convert.ToInt64(userid));
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    return Convert.ToBoolean(reader.GetInt32(0));
                }
            }
            return false;
        }
    }
}
