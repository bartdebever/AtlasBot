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
    }
}
