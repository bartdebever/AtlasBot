using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLibary.Data;
using DataLibary.Models;
using RiotSharp;

namespace DataLibary.Repos
{
    public class RegionRepo
    {
        private IRegionContext context;

        public RegionRepo(IRegionContext context)
        {
            this.context = context;
        }

        public int GetRegionId(Region region)
        {
            return context.GetRegionId(region);
        }

        public int GetRegionId(User user)
        {
            return context.GetRegionId(user);
        }

        public List<string> GetAllRegions()
        {
            return context.GetAllRegions();
        }
    }
}
