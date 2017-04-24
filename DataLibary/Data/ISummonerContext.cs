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
        void AddSummoner(int userid, int riotid, int regionid, string token);
        void RemoveSummoner(User user, int riotid);
        void VerifySummoner(User user, int riotid);
        string GetToken(User user, int riotid);
        int GetSummonerByUserId(User user);
        List<int> GetSummonersByRegion(int regionid);
        bool IsSummonerInSystem(int riotid);
    }
}
