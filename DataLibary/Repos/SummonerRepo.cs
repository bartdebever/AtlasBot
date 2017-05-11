using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLibary.Data;
using DataLibary.Models;

namespace DataLibary.Repos
{
    public class SummonerRepo
    {
        private ISummonerContext context;

        public SummonerRepo(ISummonerContext context)
        {
            this.context = context;
        }

        public void AddSummoner(int userid, int riotid, int regionid, string token)
        {
            context.AddSummoner(userid, riotid, regionid, token);
        }

        public void RemoveSummoner(User user, int riotid)
        {
            context.RemoveSummoner(user, riotid);
        }

        public void VerifySummoner(User user, int riotid)
        {
            context.VerifySummoner(user, riotid);
        }

        public string GetToken(User user, int riotid)
        {
            return context.GetToken(user, riotid);
        }

        public int GetSummonerByUserId(User user)
        {
            return context.GetSummonerByUserId(user);
        }

        public List<int> GetSummonersByRegion(int regionid)
        {
            return context.GetSummonersByRegion(regionid);
        }

        public bool IsSummonerInSystem(int riotid)
        {
            return context.IsSummonerInSystem(riotid);
        }
        public int GetUnverifiedSummonerByUserId(ulong userid)
        {
            return context.GetUnverifiedSummonerByUserId(userid);
        }
    }
}
