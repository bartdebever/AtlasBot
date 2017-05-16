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
    public class CoachContext : ICoachContext
    {
        public List<string> GetAllCoaches()
        {
            throw new NotImplementedException();
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
            string query = "INSERT INTO [Coach] OUTPUT INSERTED.ID VALUES((SELECT [U].id FROM [User] U WHERE [U].DiscordId = @DiscordId)) ";
            SqlCommand cmd = new SqlCommand(query, Database.Connection());
            cmd.Parameters.AddWithValue("@DiscordId", Convert.ToInt64(coach.CoachId));
            int id = (int) cmd.ExecuteScalar();
            query = "INSERT INTO [Coach_Champion] VALUES (@CoachId, @ChampionId)";
            cmd = new SqlCommand(query, Database.Connection());
            cmd.Parameters.AddWithValue("@CoachId", id);
            cmd.Parameters.AddWithValue("@ChampionId", coach.Champion);
            cmd.ExecuteNonQuery();
            query = "INSERT INTO [Coach_Role] VALUES (@CoachId, @Role)";
            cmd = new SqlCommand(query, Database.Connection());
            cmd.Parameters.AddWithValue("@CoachId", id);
            cmd.Parameters.AddWithValue("@Role", coach.Role);
            cmd.ExecuteNonQuery();
        }

        public void RemoveCoach(ulong id)
        {
            throw new NotImplementedException();
        }

        public void AddChampionToCoach(int championid, ulong id)
        {
            throw new NotImplementedException();
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
    }
}
