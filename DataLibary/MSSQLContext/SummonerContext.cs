﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLibary.Data;
using DataLibary.Models;
using RiotSharp.SummonerEndpoint;

namespace DataLibary.MSSQLContext
{
    public class SummonerContext : ISummonerContext
    {
        public void AddSummoner(int userid, long riotid, int regionid, string token)
        {
            string query = "INSERT INTO [Summoner] VALUES (@userid, @riotid, @regionid, @token, 0, null)";
            SqlCommand cmd = new SqlCommand(query, Database.Connection());
            cmd.Parameters.AddWithValue("@userid", userid);
            cmd.Parameters.AddWithValue("@riotid", riotid);
            cmd.Parameters.AddWithValue("@regionid", regionid);
            cmd.Parameters.AddWithValue("@token", token);
            cmd.ExecuteNonQuery();
        }

        public void RemoveSummoner(User user, long riotid)
        {
            string query = "DELETE [Summoner] WHERE UserId = @userid AND RiotId = @riotid";
            SqlCommand cmd = new SqlCommand(query, Database.Connection());
            cmd.Parameters.AddWithValue("@userid", user.Id);
            cmd.Parameters.AddWithValue("@riotid", riotid);
            cmd.ExecuteNonQuery();
        }

        public void VerifySummoner(User user, long riotid)
        {
            string query = "UPDATE [Summoner] SET Verified = 1 WHERE UserId = @userid AND RiotId = @riotid";
            SqlCommand cmd = new SqlCommand(query, Database.Connection());
            cmd.Parameters.AddWithValue("@userid", user.Id);
            cmd.Parameters.AddWithValue("@riotid", riotid);
            cmd.ExecuteNonQuery();
        }

        public long GetSummonerByUserId(User user)
        {
            string query = "SELECT Riotid FROM [Summoner] WHERE UserId = @userid AND Verified = 1";
            SqlCommand cmd = new SqlCommand(query, Database.Connection());
            cmd.Parameters.AddWithValue("@userid", user.Id);
            using ( SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    return reader.GetInt64(0);
                }
            }
            throw new Exception("Summoner not found");
        }

        public List<int> GetSummonersByRegion(int regionid)
        {
            List<int> result = new List<int>();
            string query = "SELECT Riotid FROM [Summoner] WHERE RegionId = @regionid AND Verified = true";
            SqlCommand cmd = new SqlCommand(query, Database.Connection());
            cmd.Parameters.AddWithValue("@regionid", regionid);
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    result.Add(reader.GetInt32(0));
                }
            }
            return result;
        }

        public string GetToken(User user, long riotid)
        {
            string query = "SELECT Token FROM [Summoner] WHERE UserId = @userid AND RiotId = @riotid";
            SqlCommand cmd = new SqlCommand(query, Database.Connection());
            cmd.Parameters.AddWithValue("@userid", user.Id);
            cmd.Parameters.AddWithValue("@riotid", riotid);
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    return reader.GetString(0);
                }
            }
            throw new Exception("Summoner not found");
        }

        public bool IsSummonerInSystem(long riotid)
        {
            string query = "SELECT COUNT(Id) FROM [Summoner] WHERE RiotId = @riotid";
            SqlCommand cmd = new SqlCommand(query, Database.Connection());
            cmd.Parameters.AddWithValue("@riotid", riotid);
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    if (reader.GetInt32(0) == 1)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return false;
        }

        public long GetUnverifiedSummonerByUserId(ulong userid)
        {
            string query = "SELECT Riotid FROM [Summoner] INNER JOIN [User] U ON [U].id = [Summoner].userid WHERE DiscordId = @userid AND verified = 0";
            SqlCommand cmd = new SqlCommand(query, Database.Connection());
            cmd.Parameters.AddWithValue("@userid", Convert.ToInt64(userid));
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    return reader.GetInt64(0);
                }
            }
            throw new Exception("Summoner not found");
        }
    }
    
}
