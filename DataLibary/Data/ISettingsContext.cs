using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLibary.Models;

namespace DataLibary.Data
{
    public interface ISettingsContext
    {
        bool RankByParameter(ulong serverid);
        bool RankByAccount(ulong serverid);
        bool RegionByParameter(ulong serverid);
        bool RegionByAccount(ulong serverid);
        bool MasteryPointsByAccount(ulong serverid);
        bool MasteryLevelByAccount(ulong serverid);
        CommandType RankCommandType(ulong serverid);
        void SetRankType(CommandType type, ulong serverid);
        void AllowRankAccount(bool value, ulong serverid);
        ulong GetOverride(string parameter, ulong serverid);
    }
}
