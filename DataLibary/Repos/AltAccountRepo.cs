using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLibary.Data;

namespace DataLibary.Repos
{
    public class AltAccountRepo : IAltAccountContext
    {
        private IAltAccountContext context;

        public AltAccountRepo(IAltAccountContext context)
        {
            this.context = context;
        }
        public string GetToken(ulong userid)
        {
            return context.GetToken(userid);
        }

        public void AddAccount(ulong discordid, long riotid, int regionid, string token)
        {
            context.AddAccount(discordid, riotid, regionid, token);
        }

        public void VerifyAccount(ulong userid)
        {
            context.VerifyAccount(userid);
        }

        public bool UniverifiedAccount(ulong userid)
        {
            return context.UniverifiedAccount(userid);
        }
    }
}
