using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RiotSharp;

namespace ToolKit
{
    public static class RiotSharpAddition
    {
        public static Platform RegionToPlatform(Region region)
        {
            if ((region == Region.euw))
            {
                return Platform.EUW1;
            }
            else if (region == Region.na)
            {
                return Platform.NA1;
            }
            else if (region == Region.eune)
            {
                return Platform.EUN1;
            }
            else if (region == Region.oce)
            {
                return Platform.OC1;
            }
            else if (region == Region.lan)
            {
                return Platform.LA1;
            }
            else if (region == Region.las)
            {
                return Platform.LA2;
            }
            else if (region == Region.br)
            {
                return Platform.BR1;
            }
            else if (region == Region.ru)
            {
                return Platform.RU;
            }
            else if (region == Region.kr)
            {
                return Platform.KR;
            }
            else if (region == Region.tr)
            {
                return Platform.TR1;
            }
            return Platform.EUW1;
        }
    }
}
