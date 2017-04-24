using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLibary.Models
{
    public class Command
    {
        public int Id { get; private set; }
        public ulong ServerId { get; private set; }
        public CommandType CommandType { get; private set; }
    }
}
