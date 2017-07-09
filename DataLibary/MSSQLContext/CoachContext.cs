using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
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

            string query = "SElECT [U].DiscordId, [S].RiotId, [S].RegionId, [C].*, CASE WHEN EXISTS(SELECT Verified FROM [Coach_LoM] CL WHERE [CL].CoachId = 2 AND Verified = 1) THEN 1 ELSE 0 END AS [Verified LoM] " +
                           "FROM [Coach] C " +
                           "INNER JOIN [User] U " +
                           "ON [c].UserId = [U].Id " +
                           "INNER JOIN [Summoner] S " +
                           "ON [S].UserId = [U].Id " +
                           "WHERE [S].Verified = 1;";
            SqlCommand cmd = new SqlCommand(query, Database.Connection());
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    result.Add(CreateCoachFromReader(reader));
                }
            }
            cmd.Dispose();
            foreach (var coach in result)
            {
                query =
                    "SELECT [CH].ChampionId FROM [Coach] C LEFT OUTER JOIN [Coach_Champion] CH ON [CH].CoachId = [C].Id WHERE [C].Id = @Id";
                cmd = new SqlCommand(query, Database.Connection());
                cmd.Parameters.AddWithValue("@Id", coach.Id);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        coach.ChampionIds.Add(reader.GetInt32(0));
                    }
                }
                cmd.Dispose();
            }
            foreach (var coach in result)
            {
                query =
                    "SELECT [CR].[Role] FROM [Coach] C LEFT OUTER JOIN [Coach_Role] CR ON [CR].CoachId = [C].Id WHERE [C].Id = @Id;";
                cmd = new SqlCommand(query, Database.Connection());
                cmd.Parameters.AddWithValue("@Id", coach.Id);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        coach.Roles.Add(reader.GetString(0));
                    }
                }
                cmd.Dispose();
            }
            foreach (var coach in result)
            {
                query =
                    "SELECT [CL].Language FROM [Coach_Language] CL INNER JOIN [Coach] C ON [C].id = [CL].coachid WHERE [c].id = @id";
                cmd = new SqlCommand(query, Database.Connection());
                cmd.Parameters.AddWithValue("@Id", coach.Id);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        coach.Languages.Add(reader.GetString(0));
                    }
                }
                cmd.Dispose();
            }
            foreach (var coach in result)
            {
                query =
                    "SELECT [CL].Link FROM [Coach_Link] CL INNER JOIN [Coach] C ON [C].id = [CL].coachid WHERE [c].id = @id";
                cmd = new SqlCommand(query, Database.Connection());
                cmd.Parameters.AddWithValue("@Id", coach.Id);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        coach.Links.Add(reader.GetString(0));
                    }
                }
                cmd.Dispose();
            }
            foreach (var coach in result)
            {
                query =
                    "SELECT [CP].preference FROM [Coach_Preference] CP INNER JOIN [Coach] C ON [C].id = [CP].coachid WHERE [c].id = @id";
                cmd = new SqlCommand(query, Database.Connection());
                cmd.Parameters.AddWithValue("@Id", coach.Id);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        coach.Prerferences.Add(reader.GetString(0));
                    }
                }
                cmd.Dispose();
            }
            return result;
        }

        public List<string> GetCoachByRole(string role)
        {
            throw new NotImplementedException();
        }

        public List<string> GetCoachByRegion(Region region)
        {
            throw new NotImplementedException();
        }

        public List<string> GetCoachByChampion(int championid)
        {
            throw new NotImplementedException();
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
            return new Coach(Convert.ToInt32(reader["id"]), new List<string>(),new List<int>(), reader["Timezone"].ToString(),reader["availability"].ToString(),Convert.ToBoolean(reader["Verified LoM"]),Convert.ToInt32(reader["RiotId"]),
                LeagueAndDatabase.GetRegionFromDatabaseId(Convert.ToInt32(reader["RegionId"])), Convert.ToUInt64(Convert.ToInt64(reader["DiscordId"])));
        }
    }
}
