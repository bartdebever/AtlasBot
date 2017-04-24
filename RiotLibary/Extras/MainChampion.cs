using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiotLibary.Extras
{
    public class MainChampion : IComparable
    {
        public long Id { get; private set; }
        public int Count { get; private set; }
        public string Name { get; set; }
        public MainChampion(long id)
        {
            this.Id = id;
            this.Count = 1;
        }

        public void IncreaseCount()
        {
            this.Count += 1;
        }

        public int CompareTo(object obj)
        {
            return (obj as MainChampion).Count.CompareTo(this.Count); 
        }
    }
}
