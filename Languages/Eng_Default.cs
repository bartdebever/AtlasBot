using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Languages
{
    public static class Eng_Default
    {
        public static string NotAllowed()
        {
            return "This action is not allowed by the server";
        }

        public static string AccountVerified()
        {
            return "Your account has been verified!";
        }

        public static string ServerVerified()
        {
            return "Your server has been verified!";
        }

        public static string VerifyServer()
        {
            return "Thank you for adding AtlasBot to your server!\nFor safety reasons we need verification from Bort that this is allowed.\nPlease use the command *-verify <key>* to verify your server!\nBort will contact you soon with your key.";
        }

        public static string RoleHasBeenGiven(string role)
        {
            return "Given the " + role + " role to you.";
        }

        public static string RolesHaveBeenGiven()
        {
            return "Your roles have been given to you.";
        }

        public static string ServerDoesNotAllow()
        {
            return "The server does not allow this action.";
        }

        public static string IncorrectParamter(string parameter)
        {
            return "Incorrect parameter: " + parameter + ".";
        }

        public static string InviteLinkSet(string invitelink)
        {
            return "Invite link has been set to: " + invitelink + ".";
        }

        public static string SettingsChange(string setting, string value)
        {
            return setting + " has been changed to " + value;
        }

        public static string UserHasLeft(string user, string server)
        {
            return user + " has left " + server +".";
        }

        public static string UserHasJoin(string user, string server)
        {
            return user + " has joined " + server + ".";
        }

        public static string UserHasRegistered(string user, string leagueaccount, string region)
        {
            return user + " has registered as: " + leagueaccount + " on region: " + region + ".";
        }

        public static string RoleHasBeenDisabled()
        {
            return "This role has been disabled to be given as parameter, claim it by account instead.";
        }
    }
}
