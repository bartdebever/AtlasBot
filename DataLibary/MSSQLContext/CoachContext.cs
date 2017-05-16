using System;
using System.Collections.Generic;
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

        public List<string> GetCoachByRole(Role role)
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
            throw new NotImplementedException();
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

        public void AddRoleToCoach(Role role, ulong id)
        {
            throw new NotImplementedException();
        }

        public void RemoveRoleFromCoach(Role role, ulong id)
        {
            throw new NotImplementedException();
        }
    }
}
