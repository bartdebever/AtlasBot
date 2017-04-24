using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiotLibary.Exceptions
{
    public class NoRankedGamesException : Exception
    {
        public NoRankedGamesException() : base() { }
        public NoRankedGamesException(string message) : base() { }
    }
}
