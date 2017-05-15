using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLibary.Models
{
    public static class Queues
    {
        public static List<string> GetQueues()
        {
            List<string> result = new List<string>();
            result.Add("3v3 normals");
            result.Add("3v3 ranked");
            result.Add("normals");
            result.Add("solo/duo");
            result.Add("flex");
            result.Add("featured gamemode");
            result.Add("aram");
            return result;
        }
    }
}
