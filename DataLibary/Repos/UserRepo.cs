using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLibary.Data;
using RiotSharp;

namespace DataLibary.Models
{
    public class UserRepo : IUserContext
    {
        private IUserContext context;

        public UserRepo(IUserContext context)
        {
            this.context = context;
        }
        public User GetUserById(int id)
        {
            return context.GetUserById(id);
        }

        public List<User> GetUsersByRegion(Region region)
        {
            return context.GetUsersByRegion(region);
        }

        public void AddUser(Int64 discordid)
        {
            context.AddUser(discordid);
        }

        public void RemoveUser(User user)
        {
            context.RemoveUser(user);
        }

        public User GetUserByDiscord(ulong discordid)
        {
            return context.GetUserByDiscord(discordid);
        }

        public User GetUserByRiotid(int riotid)
        {
            return GetUserByRiotid(riotid);
        }

        public int GetUserIdByDiscord(ulong discordid)
        {
            return context.GetUserIdByDiscord(discordid);
        }

        public DateTime GetLastRefreshDate(ulong userid)
        {
            return context.GetLastRefreshDate(userid);
        }

        public void SetLastRefreshDate(ulong userid, DateTime date)
        {
            context.SetLastRefreshDate(userid, date);
        }

        public bool IsAtlasAdmin(ulong userid)
        {
            return context.IsAtlasAdmin(userid);
        }

        public List<User> GetAllAccounts(ulong userid)
        {
            return context.GetAllAccounts(userid);
        }

        public string GetBackupName(ulong discordid)
        {
            return context.GetBackupName(discordid);
        }
    }
}
