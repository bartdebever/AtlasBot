using RiotSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolKit.Exception;

namespace ToolKit
{
    public static class LeagueAndDatabase
    {
        public static Region GetRegionFromString(string region)
        {
            foreach (Region r in Enum.GetValues(typeof(Region)))
            {
                if (r.ToString().ToLower() == region.ToLower())
                {
                    return r;
                }
            }
            throw new RegionNotFoundException();
        }

        public static Region GetRegionFromDatabaseId(int id)
        {
            if (id == 1)
            {
                return Region.euw;
            }
            else if (id == 6)
            {
                return Region.na;
            }
            else if (id == 2)
            {
                return Region.eune;
            }
            else if (id == 7)
            {
                return Region.oce;
            }
            else if (id == 4)
            {
                return Region.lan;
            }
            else if (id == 5)
            {
                return Region.las;
            }
            else if (id == 3)
            {
                return Region.br;
            }
            else if (id == 8)
            {
                return Region.ru;
            }
            else if (id == 9)
            {
                return Region.tr;
            }
            else if (id == 10)
            {
                return Region.jp;
            }
            else if (id == 11)
            {
                return Region.oce;
            }
            else if (id == 12)
            {
                return Region.kr;
            }
            else if (id == 13)
            {
                return Region.global;
            }
            return Region.euw;

        }
    }
}
