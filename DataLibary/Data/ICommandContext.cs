using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLibary.Models;

namespace DataLibary.Data
{
    public interface ICommandContext
    {
        void AddCommand(Server server, Command command);
        void RemoveCommand(Server server, Command command);
        void UpdateCommand(Server server, Command command);
    }
}
