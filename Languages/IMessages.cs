using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;

namespace Languages
{
    public interface IMessages
    {
        string NotAllowed();
        string AccountVerified();
        string ServerVerified();
        string VerifyServer();
        string RoleHasBeenGiven(string role);
        string RolesHaveBeenGiven();
        string ServerDoesNotAllow();
        string IncorrectParamter(string parameter);
        string InviteLinkSet(string invitelink);
        string SettingsChange(string setting, string value);
        string UserHasLeft(string user, string server);
        string UserHasJoin(string user, string server);
        string UserHasRegistered(string user, string leagueaccount, string region);
    }
}
