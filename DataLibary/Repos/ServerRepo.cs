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
    }
}
