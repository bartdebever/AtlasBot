using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLibary.Models
{
    public class Rank
    {
        public int Id { get; private set; }
        public string Tier { get; private set; }
        public string Division { get; private set; }

        public Rank(int id, string tier)
        {
            this.Id = id;
            this.Tier = tier;
        }

        public Rank(int id, string tier, string division) : this(id, tier)
        {
            this.Division = division;
        }
    }
}
