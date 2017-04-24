using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using DataLibary.Models;

namespace DataLibary.Data
{
    interface IRankContext
    {
        List<Rank> GetRanksByServer(Server server);
        void AddRank(Server server, Rank rank);
        void RemoveRank(Server server, Rank rank);
        void UpdateRanks(Server server, Rank rank, Rank newrank);
        Rank GetRankById(Rank rank);
    }
}
