using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLibary.Data;
using RiotSharp;

namespace DataLibary.Models
{
    public class CoachRepo
    {
        private ICoachContext context;

        public CoachRepo(ICoachContext context)
        {
            this.context = context;
        }
        public List<Coach> GetAllCoaches()
        {
            return context.GetAllCoaches();
        }

        public List<string> GetCoachByRole(string role)
        {
            return context.GetCoachByRole(role);
        }

        public List<string> GetCoachByRegion(Region region)
        {
            return context.GetCoachByRegion(region);
        }

        public List<string> GetCoachByChampion(int championid)
        {
            return context.GetCoachByChampion(championid);
        }

        public void AddCoach(Coach coach)
        {
            context.AddCoach(coach);
        }

        public void RemoveCoach(ulong id)
        {
            context.RemoveCoach(id);
        }

        public void AddChampionToCoach(int championid, ulong id)
        {
            context.AddChampionToCoach(championid, id);
        }

        public void RemoveChampionFromCoach(int championid, ulong id)
        {
            context.RemoveChampionFromCoach(championid, id);
        }

        public void AddRoleToCoach(string role, ulong id)
        {
            context.AddRoleToCoach(role, id);
        }

        public void RemoveRoleFromCoach(string role, ulong id)
        {
            context.RemoveRoleFromCoach(role, id);
        }
    }
}
