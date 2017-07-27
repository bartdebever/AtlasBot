using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DataLibary.Data;
using DataLibary.Models;
using RiotSharp;
using ToolKit;

namespace DataLibary.MSSQLContext
{
    public class CoachContext : ICoachContext
    {
        public List<Coach> GetAllCoaches()
        {
            List<Coach> result = new List<Coach>();

            string query =
                "SElECT [U].DiscordId, [S].RiotId, [S].RegionId, [C].*, CASE WHEN EXISTS(SELECT Verified FROM [Coach_LoM] CL WHERE [CL].CoachId = [C].Id AND [CL].Verified = 1) THEN 1 ELSE 0 END AS [Verification] " +
                "FROM [Coach] C " +
                "INNER JOIN [User] U " +
                "ON [c].UserId = [U].Id " +
                "LEFT OUTER JOIN [Summoner] S " +
                "ON [S].UserId = [U].Id ";
            SqlCommand cmd = new SqlCommand(query, Database.Connection());
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    result.Add(CreateCoachFromReader(reader));
                }
            }
            cmd.Dispose();
            //foreach (var coach in result)
            //{
                
            //}
            //foreach (var coach in result)
            //{
            //    query =
            //        "SELECT [CR].[Role] FROM [Coach] C LEFT OUTER JOIN [Coach_Role] CR ON [CR].CoachId = [C].Id WHERE [C].Id = @Id;";
            //    cmd = new SqlCommand(query, Database.Connection());
            //    cmd.Parameters.AddWithValue("@Id", coach.Id);
            //    using (SqlDataReader reader = cmd.ExecuteReader())
            //    {
            //        while (reader.Read())
            //        {
            //            coach.Roles.Add(reader.GetString(0));
            //        }
            //    }
            //    cmd.Dispose();
            //}
            //foreach (var coach in result)
            //{
            //    query =
            //        "SELECT [CL].Language FROM [Coach_Language] CL INNER JOIN [Coach] C ON [C].id = [CL].coachid WHERE [c].id = @id";
            //    cmd = new SqlCommand(query, Database.Connection());
            //    cmd.Parameters.AddWithValue("@Id", coach.Id);
            //    using (SqlDataReader reader = cmd.ExecuteReader())
            //    {
            //        while (reader.Read())
            //        {
            //            coach.Languages.Add(reader.GetString(0));
            //        }
            //    }
            //    cmd.Dispose();
            //}
            //foreach (var coach in result)
            //{
            //    query =
            //        "SELECT [CL].Link FROM [Coach_Link] CL INNER JOIN [Coach] C ON [C].id = [CL].coachid WHERE [c].id = @id";
            //    cmd = new SqlCommand(query, Database.Connection());
            //    cmd.Parameters.AddWithValue("@Id", coach.Id);
            //    using (SqlDataReader reader = cmd.ExecuteReader())
            //    {
            //        while (reader.Read())
            //        {
            //            coach.Links.Add(reader.GetString(0));
            //        }
            //    }
            //    cmd.Dispose();
            //}
            //foreach (var coach in result)
            //{
            //    query =
            //        "SELECT [CP].preference FROM [Coach_Preference] CP INNER JOIN [Coach] C ON [C].id = [CP].coachid WHERE [c].id = @id";
            //    cmd = new SqlCommand(query, Database.Connection());
            //    cmd.Parameters.AddWithValue("@Id", coach.Id);
            //    using (SqlDataReader reader = cmd.ExecuteReader())
            //    {
            //        while (reader.Read())
            //        {
            //            coach.Prerferences.Add(reader.GetString(0));
            //        }
            //    }
            //    cmd.Dispose();
            //}
            return result;
        }

        public List<Coach> GetCoachByRole(string role)
        {
            List<Coach> result = new List<Coach>();
            string query =
                "SElECT DISTINCT [U].DiscordId, [C].Bio, [S].RiotId, [S].RegionId, [C].*, CASE WHEN EXISTS(SELECT Verified FROM [Coach_LoM] CL WHERE [CL].CoachId = [C].Id AND [CL].Verified = 1) THEN 1 ELSE 0 END AS [Verification] \r\nFROM [Coach] C \r\nINNER JOIN [User] U \r\nON [c].UserId = [U].Id \r\nLEFT OUTER JOIN [Summoner] S ON [S].UserId = [U].Id \r\nLEFT OUTER JOIN [Coach_Role] CR\r\nON [CR].CoachId = [C].Id\r\nLEFT OUTER JOIN [Coach_Language] CLL\r\nON [CLL].CoachId = [C].Id\r\nWHERE [CR].Role LIKE \'%\' + @Filter + \'%\' OR [CLL].Language LIKE \'%\' + @Filter + \'%\';";
            SqlCommand cmd = new SqlCommand(query, Database.Connection());
            cmd.Parameters.AddWithValue("@Filter", role);
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    result.Add(CreateCoachFromReader(reader));
                }
            }
            cmd.Dispose();
            return result;
        }

        public List<Coach> GetCoachByRegion(Region region)
        {
            throw new NotImplementedException();
        }

        public Coach GetCoachById(int id)
        {
            throw new NotImplementedException();
        }

        public List<Coach> GetCoachByChampion(int championid)
        {
            List<Coach> result = new List<Coach>();

            string query =
                "SElECT [U].DiscordId, [S].RiotId, [C].Bio, [S].RegionId, [C].*, CASE WHEN EXISTS(SELECT Verified FROM [Coach_LoM] CL WHERE [CL].CoachId = [C].Id AND [CL].Verified = 1) THEN 1 ELSE 0 END AS [Verification] FROM [Coach] C INNER JOIN [User] U ON [c].UserId = [U].Id LEFT OUTER JOIN [Summoner] S ON [S].UserId = [U].Id INNER JOIN [Coach_Champion] CC ON [CC].CoachId = [C].Id WHERE [CC].ChampionId  = @ChampionId;";
            SqlCommand cmd = new SqlCommand(query, Database.Connection());
            cmd.Parameters.AddWithValue("@ChampionId", championid);
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    result.Add(CreateCoachFromReader(reader));
                }
            }
            cmd.Dispose();
            return result;
        }

        public void AddCoach(Coach coach)
        {
            //string query = "INSERT INTO [Coach] OUTPUT INSERTED.ID VALUES((SELECT [U].id FROM [User] U WHERE [U].DiscordId = @DiscordId)) ";
            //SqlCommand cmd = new SqlCommand(query, Database.Connection());
            //cmd.Parameters.AddWithValue("@DiscordId", Convert.ToInt64(coach.CoachId));
            //int id = (int) cmd.ExecuteScalar();
            //query = "INSERT INTO [Coach_Champion] VALUES (@CoachId, @ChampionId)";
            //cmd = new SqlCommand(query, Database.Connection());
            //cmd.Parameters.AddWithValue("@CoachId", id);
            //cmd.Parameters.AddWithValue("@ChampionId", coach.Champion);
            //cmd.ExecuteNonQuery();
            //query = "INSERT INTO [Coach_Role] VALUES (@CoachId, @Role)";
            //cmd = new SqlCommand(query, Database.Connection());
            //cmd.Parameters.AddWithValue("@CoachId", id);
            //cmd.Parameters.AddWithValue("@Role", coach.Role);
            //cmd.ExecuteNonQuery();
        }

        public void RemoveCoach(ulong id)
        {
            throw new NotImplementedException();
        }

        public void AddChampionToCoach(int championid, ulong id)
        {
            string query =
                "INSERT INTO [Coach_Champion] VALUES (SELECT [C].Id FROM [Coach] INNER JOIN [User] U ON [U].Id = [C].UserId WHERE [U].DiscordId = @DiscordId), @ChampionId)";
            SqlCommand cmd = new SqlCommand(query, Database.Connection());
            cmd.Parameters.AddWithValue("@DiscordId", Convert.ToInt64(id));
            cmd.Parameters.AddWithValue("@ChampionId", championid);
            cmd.ExecuteNonQuery();
        }

        public void RemoveChampionFromCoach(int championid, ulong id)
        {
            throw new NotImplementedException();
        }

        public void AddRoleToCoach(string role, ulong id)
        {
            throw new NotImplementedException();
        }

        public void RemoveRoleFromCoach(string role, ulong id)
        {
            throw new NotImplementedException();
        }

        private Coach CreateCoachFromReader(SqlDataReader reader)
        {
            int riotid = 0;
            Region region = Region.euw;
            try
            {
                riotid = Convert.ToInt32(reader["RiotId"]);
                region = LeagueAndDatabase.GetRegionFromDatabaseId(Convert.ToInt32(reader["RegionId"]));
            }
            catch { }
            Coach coach =  new Coach(Convert.ToInt32(reader["id"]), new List<string>(),new List<int>(), reader["Timezone"].ToString(),reader["availability"].ToString(),Convert.ToBoolean(reader["Verification"]),riotid, region, Convert.ToUInt64(Convert.ToInt64(reader["DiscordId"])),reader["Bio"].ToString());
            string query = "";
            SqlCommand cmd;
            try
            {
                query =
                    "SELECT [CH].ChampionId FROM [Coach] C LEFT OUTER JOIN [Coach_Champion] CH ON [CH].CoachId = [C].Id WHERE [C].Id = @Id";
                cmd = new SqlCommand(query, Database.Connection());
                cmd.Parameters.AddWithValue("@Id", coach.Id);
                using (SqlDataReader reader2 = cmd.ExecuteReader())
                {
                    while (reader2.Read())
                    {
                        coach.ChampionIds.Add(reader2.GetInt32(0));
                    }
                }
                cmd.Dispose();
            }
            catch
            {
                
            }
            try
            {
                query =
                    "SELECT [CL].Language FROM [Coach_Language] CL INNER JOIN [Coach] C ON [C].id = [CL].coachid WHERE [c].id = @id";
                cmd = new SqlCommand(query, Database.Connection());
                cmd.Parameters.AddWithValue("@Id", coach.Id);
                using (SqlDataReader reader2 = cmd.ExecuteReader())
                {
                    while (reader2.Read())
                    {
                        coach.Languages.Add(reader2.GetString(0));
                    }
                }
                cmd.Dispose();
            }
            catch { }
            try
            {
                query =
                    "SELECT [CP].preference FROM [Coach_Preference] CP INNER JOIN [Coach] C ON [C].id = [CP].coachid WHERE [c].id = @id";
                cmd = new SqlCommand(query, Database.Connection());
                cmd.Parameters.AddWithValue("@Id", coach.Id);
                using (SqlDataReader reader2 = cmd.ExecuteReader())
                {
                    while (reader2.Read())
                    {
                        coach.Prerferences.Add(reader2.GetString(0));
                    }
                }
                cmd.Dispose();
            }
            catch { }
            try
            {
                query =
                    "SELECT [CR].[Role] FROM [Coach] C LEFT OUTER JOIN [Coach_Role] CR ON [CR].CoachId = [C].Id WHERE [C].Id = @Id;";
                cmd = new SqlCommand(query, Database.Connection());
                cmd.Parameters.AddWithValue("@Id", coach.Id);
                using (SqlDataReader reader2 = cmd.ExecuteReader())
                {
                    while (reader2.Read())
                    {
                        coach.Roles.Add(reader2.GetString(0));
                    }
                }
                cmd.Dispose();
            }
            catch { }
            try
            {
                query =
                    "SELECT [CL].Link FROM [Coach_Link] CL INNER JOIN [Coach] C ON [C].id = [CL].coachid WHERE [c].id = @id";
                cmd = new SqlCommand(query, Database.Connection());
                cmd.Parameters.AddWithValue("@Id", coach.Id);
                using (SqlDataReader reader2 = cmd.ExecuteReader())
                {
                    while (reader2.Read())
                    {
                        coach.Links.Add(reader2.GetString(0));
                    }
                }
                cmd.Dispose();
            }
            catch
            {
                
            }
            coach.CreateChampions();
            return coach;

        }
    }
}
