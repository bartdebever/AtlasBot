using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLibary.Data;
using RiotSharp;

namespace DataLibary.Models
{
    public class CoachRepo : ICoachContext
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

        public List<Coach> GetCoachByRole(string role)
        {
            return context.GetCoachByRole(role);
        }

        public List<Coach> GetCoachByRegion(Region region)
        {
            return context.GetCoachByRegion(region);
        }

        public Coach GetCoachById(int id)
        {
            return context.GetCoachById(id);
        }

        public List<Coach> GetCoachByChampion(int championid)
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
