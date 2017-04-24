using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiotLibary.Exceptions
{
    public class ChampionNotFoundException : Exception
    {
        public ChampionNotFoundException() : base() { }
        public ChampionNotFoundException(string message) : base(message) { }
    }
}
