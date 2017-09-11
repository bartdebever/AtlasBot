using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Keys;
using RiotSharp;
using RiotSharp.Http;
using RiotSharp.Misc;
using Keys = Keys.Keys;

namespace TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            RiotApi riotApi = RiotApi.GetInstance(global::Keys.Keys.riotKey, 500, 30000);
            Console.WriteLine(riotApi.GetSummonerByName(Region.euw, "BortTheBeaver").Id);
            Console.ReadLine();
        }
    }
}
