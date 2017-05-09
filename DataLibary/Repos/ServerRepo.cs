using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLibary.Data;

namespace DataLibary.Models
{
    public class ServerRepo
    {
        private IServerContext context;

        public ServerRepo(IServerContext context)
        {
            this.context = context;
        }

        public void AddServer(ulong serverid, ulong ownerid, string servername, string key)
        {
            context.AddServer(serverid, ownerid, servername, key);
        }

        public void VerifyServerSQL(ulong userid, string key)
        {
            context.VerifyServerSQL(userid, key);
        }

        public void AddInviteLink(ulong userid, ulong serverid, string key)
        {
            context.AddInviteLink(userid, serverid, key);
        }

        public bool IsAdmin(ulong user, ulong server)
        {
            return context.IsAdmin(user, server);
        }

        public List<ulong> GetAllServerIds()
        {
            return context.GetAllServerIds();
        }

        public bool IsServerVerified(ulong serverid)
        {
            return context.IsServerVerified(serverid);
        }

        public string GetServerDescription(ulong serverid)
        {
            return context.GetServerDescription(serverid);
        }

        public void SetServerGescriptiong(ulong serverid, string description)
        {
            context.SetServerDecription(serverid, description);
        }

        public string InviteLink(ulong serverid)
        {
            return context.InviteLink(serverid);
        }

        public string ServerName(ulong serverid)
        {
            return context.ServerName(serverid);
        }

        public void SetServerName(ulong serverid, string name)
        {
            context.SetServerName(serverid, name);
        }
        public void AddAdmin(ulong userid, ulong serverid)
        {
            context.AddAdmin(userid, serverid);
        }
        public List<string> ListAdmins(ulong serverid)
        {
            return context.ListAdmins(serverid);
        }

        public void RemoveAdmin(ulong userid, ulong serverid)
        {
            context.RemoveAdmin(userid, serverid);
        }
        public DateTime GetLastupdateDateServer(ulong serverid)
        {
            return context.GetLastRefreshDate(serverid);
        }

        public void SetUpdateDateServer(ulong serverid, DateTime date)
        {
            context.SetLastRefreshDate(serverid, date);
        }
    }
}
