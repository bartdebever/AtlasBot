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
    }
}
