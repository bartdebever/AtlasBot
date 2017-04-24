using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLibary.Models;
using RiotSharp;

namespace DataLibary.Data
{
    public interface IRegionContext
    {
        int GetRegionId(Region region);
        int GetRegionId(User user);
    }
}
