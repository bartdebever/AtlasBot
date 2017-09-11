using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RiotNet.Models;

namespace RiotLibraryV2.Tools
{
    public class PlatformConverter
    {
        public static string GetPlatform(string name)
        {
            if (name.ToLower() == "euw")
            {
                return PlatformId.EUW1;
            }
            else if (name.ToLower() == "na")
            {
                return PlatformId.NA1;
            }
            else if (name.ToLower() == "eune")
            {
                return PlatformId.EUN1;
            }
            else if (name.ToLower() == "oce")
            {
                return PlatformId.OC1;
            }
            else if (name.ToLower() == "lan")
            {
                return PlatformId.LA1;
            }
            else if (name.ToLower() == "las")
            {
                return PlatformId.LA2;
            }
            else if (name.ToLower() == "br")
            {
                return PlatformId.BR1;
            }
            else if (name.ToLower() == "ru")
            {
                return PlatformId.RU;
            }
            else if (name.ToLower() == "tr")
            {
                return PlatformId.TR1;
            }
            else if (name.ToLower() == "jp")
            {
                return PlatformId.JP1;
            }
            else if (name.ToLower() == "oce")
            {
                return PlatformId.OC1;
            }
            else if (name.ToLower() == "kr")
            {
                return PlatformId.KR;
            }
            else if (name.ToLower() == "pbe")
            {
                return PlatformId.PBE1;
            }
            return PlatformId.EUW1;
        }

        public static string GetPlatform(int id)
        {
            if (id == 1)
            {
                return PlatformId.EUW1;
            }
            else if (id == 6)
            {
                return PlatformId.NA1;
            }
            else if (id == 2)
            {
                return PlatformId.EUN1;
            }
            else if (id == 7)
            {
                return PlatformId.OC1;
            }
            else if (id == 4)
            {
                return PlatformId.LA1;
            }
            else if (id == 5)
            {
                return PlatformId.LA2;
            }
            else if (id == 3)
            {
                return PlatformId.BR1;
            }
            else if (id == 8)
            {
                return PlatformId.RU;
            }
            else if (id == 9)
            {
                return PlatformId.TR1;
            }
            else if (id == 10)
            {
                return PlatformId.JP1;
            }
            else if (id == 11)
            {
                return PlatformId.OC1;
            }
            else if (id == 12)
            {
                return PlatformId.KR;
            }
            else if (id == 13)
            {
                return PlatformId.PBE1;
            }
            return PlatformId.EUW1;
        }
    }
}
