using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiotLibary.Exceptions
{
    public class NotLevel30Exception : Exception
    {
        public NotLevel30Exception() : base() { }
        public NotLevel30Exception(string message) : base(message) { }
    }
}
