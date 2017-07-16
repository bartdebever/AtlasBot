﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RiotSharp;
using RiotSharp.ChampionEndpoint;

namespace DataLibary.Models
{
    //public class Coach
    //{
    //    public ulong CoachId { get; private set; }
    //    public string Role { get; private set; }
    //    public int Champion { get; private set; }

    //    public Coach(ulong coachid, string role, int champion)
    //    {
    //        this.CoachId = coachid;
    //        this.Role = role;
    //        this.Champion = champion;
    //    }
    //}
    public class Coach
    {
        public int Id { get; }
        public List<string> Roles { get; }
        public List<int> ChampionIds { get; set; }
        public List<string> Languages { get; set; }
        public List<string> Prerferences { get; set; }
        public List<string> Links { get; set; }
        public string Cost { get; set; }
        public string Timezone { get; }
        public string Availability { get; }
        public string Bio { get; }
        public bool LoMVerified { get; }
        public int SummonerId { get; }
        public Region region { get; }
        public ulong DiscordId { get; }

        public Coach(int id, List<string> roles, List<int> championIds, string Timezone, string Availability, bool loMVerified, int summonerId, Region region, ulong discordId, string bio)
        {
            this.Id = id;
            this.Roles = roles;
            this.ChampionIds = championIds;
            this.Timezone = Timezone;
            this.Availability = Availability;
            this.LoMVerified = loMVerified;
            this.SummonerId = summonerId;
            this.region = region;
            this.DiscordId = discordId;
            this.Languages = new List<string>();
            this.Links = new List<string>();
            this.Prerferences = new List<string>();
            this.Bio = bio;
        }
        public Coach(int id, List<string> roles, List<int> championIds, string Timezone, string Availability, bool loMVerified, int summonerId, Region region, ulong discordId, List<string> language, List<string> preferences, List<string> links, string cost, string bio)
        {
            this.Id = id;
            this.Roles = roles;
            this.ChampionIds = championIds;
            this.Timezone = Timezone;
            this.Availability = Availability;
            this.LoMVerified = loMVerified;
            this.SummonerId = summonerId;
            this.region = region;
            this.DiscordId = discordId;
            this.Cost = cost;
            this.Languages = language;
            this.Prerferences = preferences;
            this.Links = links;
            this.Bio = bio;
        }
        public Coach(int id, string role, int champion)
        {
            this.Id = id;
            this.Roles = new List<string>();
            this.Roles.Add(role);
            this.ChampionIds = new List<int>();
            this.ChampionIds.Add(champion);
        }

        public Coach()
        {
            this.ChampionIds = new List<int>();
            this.Roles = new List<string>();
            this.Roles = new List<string>();
        }
    }
}
