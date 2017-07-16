using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLibary.Models;
using RiotSharp;
using RiotSharp.ChampionEndpoint;

namespace DataLibary.Data
{
    public interface ICoachContext
    {
        List<Coach> GetAllCoaches();
        List<Coach> GetCoachByRole(string role);
        List<Coach> GetCoachByRegion(Region region);
        Models.Coach GetCoachById(int id);
        List<Coach> GetCoachByChampion(int championid);
        void AddCoach(Coach coach);
        void RemoveCoach(ulong id);
        void AddChampionToCoach(int championid, ulong id);
        void RemoveChampionFromCoach(int championid, ulong id);
        void AddRoleToCoach(string role, ulong id);
        void RemoveRoleFromCoach(string role, ulong id);
    }
}
