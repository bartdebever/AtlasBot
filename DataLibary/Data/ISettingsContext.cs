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
        void AllowRankParameter(bool value, ulong serverid);
        void ChangeRegionAccount(bool value, ulong serverid);
        void ChangeRegionParameter(bool value, ulong serverid);
        ulong GetOverride(string parameter, ulong serverid);
        void AddOverride(string parameter, ulong rank, ulong serverid);
        List<string> GetAllOverridesInformation(ulong serverid);
        List<string> GetAllOverrides(ulong serverid);
        void RemoveOverride(int id, ulong serverid);
        void CreateSettings(ulong serverid);
        List<string> GetDisabledRoles(ulong serverid);
        bool IsRoleDisabled(string parameter, ulong serverid);
        void AddRoleDisable(string parameter, ulong serverid);
        void RemoveRoleDisable(int id, ulong serverid);
    }
}
