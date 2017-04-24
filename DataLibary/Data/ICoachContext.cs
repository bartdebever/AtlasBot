using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLibary.Models;
using RiotSharp;
using RiotSharp.ChampionEndpoint;
using Role = DataLibary.Models.Role;

namespace DataLibary.Data
{
    public interface ICoachContext
    {
        List<ICoachContext> GetAllCoaches();
        List<ICoachContext> GetCoachByRole(Role role);
        List<ICoachContext> GetCoachByRegion(Region region);
        void AddCoach(Coach coach);
        void RemoveCoach(Coach coach);
        void UpdateCoach(Coach coach, Coach oldcoach);
        void AddChampionToCoach(Champion champion, Coach coach);
        void RemoveChampionFromCoach(Champion champion, Coach coach);
        void AddRoleToCoach(Role role, Coach coach);
        void RemoveRoleFromCoach(Role role, Coach coach);
    }
}
