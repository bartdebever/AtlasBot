using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLibary.Models;
using RiotSharp;

namespace DataLibary.Data
{
    public interface IUserContext
    {
        User GetUserById(int id);
        List<User> GetUsersByRegion(Region region);
        void AddUser(Int64 discordid);
        void RemoveUser(User user);
        User GetUserByDiscord(ulong discordid);
        int GetUserIdByDiscord(ulong discordid);
        User GetUserByRiotid(int riotid);
        DateTime GetLastRefreshDate(ulong userid);
        void SetLastRefreshDate(ulong userid, DateTime date);
        bool IsAtlasAdmin(ulong userid);
        List<User> GetAllAccounts(ulong userid);
        string GetBackupName(ulong discordid);
        bool Login(string username, string hash);
        bool Validate(string username);
        void Register(string username, string password);
    }
}
