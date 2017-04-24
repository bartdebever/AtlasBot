using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolKit.Exception
{
    public class RegionNotFoundException : System.Exception
    {
        public RegionNotFoundException() : base() { }
        public RegionNotFoundException(string message) : base(message) { }
    }
}
