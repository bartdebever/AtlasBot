using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiotLibary.Exceptions
{
    class SummonerNotFoundException : Exception
    {
        public SummonerNotFoundException() : base() { }
        public SummonerNotFoundException(string message): base(message) { }
    }
}
