﻿using System;
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
        bool RoleByParameter(ulong serverid);
        bool RoleByAccount(ulong serverid);
        bool MasteryPointsByAccount(ulong serverid);
        bool MasteryLevelByAccount(ulong serverid);
        CommandType RankCommandType(ulong serverid);
        CommandType RoleCommandType(ulong serverid);
        void SetRankType(CommandType type, ulong serverid);
        void SetRoleType(CommandType type, ulong serverid);
        void AllowRankAccount(bool value, ulong serverid);
        void AllowRankParameter(bool value, ulong serverid);
        void ChangeRegionAccount(bool value, ulong serverid);
        void ChangeRegionParameter(bool value, ulong serverid);
        void ChangeRoleAccount(bool value, ulong serverid);
        void ChangeRoleParameter(bool value, ulong serverid);
        void ChangeMasteryAccount(bool value, ulong serverid);
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
        int GetChampionId(ulong serverid);
        ulong GetRoleByPoints(ulong serverid, int points);
        void AddRoleByPoints(ulong roleid, ulong serverid, int points);
        void RemoveRoleByPoints(ulong serverid, int points);
        void SetChampionId(ulong serverid, int championid);
        List<string> GetAllMasteryRoles(ulong serverid);
        bool lfgStatus(ulong serverid);
        ulong GetLfgChannel(ulong serverid);
        void SetLfgChannel(ulong channelid, ulong serverid);
        void DisableLfg(ulong serverid);
    }
}
