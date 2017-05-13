using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLibary.Data;
using DataLibary.Models;

namespace DataLibary.Repos
{
    public class SettingsRepo
    {
        private ISettingsContext context;

        public SettingsRepo(ISettingsContext context)
        {
            this.context = context;
        }
        public bool RankByParameter(ulong serverid)
        {
            return context.RankByParameter(serverid);
        }

        public bool RankByAccount(ulong serverid)
        {
            return context.RankByAccount(serverid);
        }

        public bool RegionByParameter(ulong serverid)
        {
            return context.RegionByParameter(serverid);
        }

        public bool RegionByAccount(ulong serverid)
        {
            return context.RegionByAccount(serverid);
        }

        public bool MasteryPointsByAccount(ulong serverid)
        {
            return context.MasteryPointsByAccount(serverid);
        }

        public bool MasteryLevelByAccount(ulong serverid)
        {
            return context.MasteryLevelByAccount(serverid);
        }

        public CommandType RankCommandType(ulong serverid)
        {
            return context.RankCommandType(serverid);
        }

        public void SetRankType(CommandType type, ulong serverid)
        {
            context.SetRankType(type, serverid);
        }

        public void ToggleAccountRank(bool value, ulong serverid)
        {
            context.AllowRankAccount(value, serverid);
        }

        public void ToggleRankParameter(bool value, ulong serverid)
        {
            context.AllowRankParameter(value, serverid);
        }

        public void ToggleRegionAccount(bool value, ulong serverid)
        {
            context.ChangeRegionAccount(value, serverid);
        }

        public void ToggleRegionParameter(bool value, ulong serverid)
        {
            context.ChangeRegionParameter(value, serverid);
        }
        public ulong GetOverride(string parameter, ulong serverid)
        {
            return context.GetOverride(parameter, serverid);
        }

        public void AddOverride(string parameter, ulong rank, ulong serverid)
        {
            context.AddOverride(parameter, rank, serverid);
        }

        public List<string> GetAllOverridesInformation(ulong serverid)
        {
            return context.GetAllOverridesInformation(serverid);
        }
        public List<string> GetAllOverrides(ulong serverid)
        {
            return context.GetAllOverrides(serverid);
        }
        public void RemoveOverride(int id, ulong serverid)
        {
            context.RemoveOverride(id, serverid);
        }

        public void CreateSettings(ulong serverid)
        {
            context.CreateSettings(serverid);
        }

        public List<string> GetDisabledRoles(ulong serverid)
        {
            return context.GetDisabledRoles(serverid);
        }

        public bool IsRoleDisabled(string parameter, ulong serverid)
        {
            return context.IsRoleDisabled(parameter, serverid);
        }

        public void AddRoleDisable(string parameter, ulong serverid)
        {
            context.AddRoleDisable(parameter, serverid);
        }

        public void RemoveRoleDisable(int id, ulong serverid)
        {
            context.RemoveRoleDisable(id, serverid);
        }
        public bool RoleByParameter(ulong serverid)
        {
            return context.RoleByParameter(serverid);
        }

        public bool RoleByAccount(ulong serverid)
        {

            return context.RoleByAccount(serverid);
        }

        public void ChangeRoleAccount(bool value, ulong serverid)
        {
            context.ChangeRoleAccount(value, serverid);
        }

        public void ChangeRoleParameter(bool value, ulong serverid)
        {
            context.ChangeRoleParameter(value, serverid);
        }

        public CommandType RoleCommandType(ulong serverid)
        {
            return context.RoleCommandType(serverid);
        }
        public void SetRoleType(CommandType type, ulong serverid)
        {
            context.SetRoleType(type, serverid);
        }

        public int GetChampionId(ulong serverid)
        {
            return context.GetChampionId(serverid);
        }

        public ulong GetRoleByPoints(ulong serverid, int points)
        {
            return context.GetRoleByPoints(serverid, points);
        }

        public void SetRoleByPoints(ulong roleid, ulong serverid, int points)
        {
            context.AddRoleByPoints(roleid, serverid, points);
        }

        public void RemoveRoleByPoints(ulong serverid, int points)
        {
            context.RemoveRoleByPoints(serverid, points);
        }

        public void SetChampionId(ulong serverid, int championid)
        {
            context.SetChampionId(serverid, championid);
        }
        public List<string> GetAllMasteryRoles(ulong serverid)
        {
            return context.GetAllMasteryRoles(serverid);
        }
        public void ChangeMasteryAccount(bool value, ulong serverid)
        {
            context.ChangeMasteryAccount(value, serverid);
        }
        public bool lfgStatus(ulong serverid)
        {
            return context.lfgStatus(serverid);
        }

        public ulong GetLfgChannel(ulong serverid)
        {
            return context.GetLfgChannel(serverid);
        }

        public void SetLfgChannel(ulong channelid, ulong serverid)
        {
            context.SetLfgChannel(channelid, serverid);
        }

        public void DisableLfg(ulong serverid)
        {
            context.DisableLfg(serverid);
        }
    }
}

