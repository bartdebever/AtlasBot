﻿using System;
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
            return "This action is not allowed.";
        }

        public static string RegisterAccount()
        {
            return "Please register your account by using -ClaimAccount *Region SummonerName*";
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

        public static string CommandTypeChange(string command, string type)
        {
            return "Type of " + command + " changed to " + type;
        }

        public static string CommandPermsChanged(string command, string value)
        {
            return command + " changed to " + value;
        }

        public static string PermsCommandNotFound()
        {
            return "Command not found, use -command help to get a list of commands.";
        }

        public static string InvalidBoolValue()
        {
            return "Please put use a right value: True, False, 1 or 0.";
        }
        public static string RolesHaveBeenGiven()
        {
            return "Your roles have been given to you.";
        }

        public static string RoleHasBeenRemoved(string role)
        {
            return "The role " + role + " has been removed from you.";
        }

        public static string RoleNotFound(string role)
        {
            return "Can not find role called " + role + ".";
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

        public static string RenameMasteryPageLong(string key)
        {
            return "Please rename one of your masterypages to: " + key +
                   "\nIt may take a little for the RiotAPI to update, please stay patient!";
        }

        public static string RenameMasteryPage(string key)
        {
            return "Please rename one of your masterypages to: " + key + ". Use this command again when you have done this.";
        }

        public static string OverrideAdded()
        {
            return "Override added successfully";
        }

        public static string OverrideFailedToAdd()
        {
            return "Failed to add override";
        }

        public static string OverrideRemoved()
        {
            return "Override removed successfully";
        }

        public static string OverrideFailedToRemoved(string id)
        {
            return "Failed to remove override with id:" + id;
        }

        public static string NoOverrides()
        {
            return "There are no overrides for this server";
        }

        public static string ServerIsNotVerified()
        {
            return "This server is not verified, no commands possible till verification";
        }

        public static string DescriptionTooLong()
        {
            return "Description is too long, please supply one with 500 characters or less.";
        }

        public static string DescriptionSet()
        {
            return "Description has been set";
        }
    }
}
