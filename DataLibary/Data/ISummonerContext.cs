using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using DataLibary.Models;
using RiotSharp.SummonerEndpoint;

namespace DataLibary.Data
{
    public interface ISummonerContext
    {
        void AddSummoner(int userid, long riotid, int regionid, string token);
        void RemoveSummoner(User user, long riotid);
        void VerifySummoner(User user,long riotid);
        string GetToken(User user, long riotid);
        long GetSummonerByUserId(User user);
        long GetUnverifiedSummonerByUserId(ulong userid);
        List<int> GetSummonersByRegion(int regionid);
        bool IsSummonerInSystem(long riotid);
    }
}
