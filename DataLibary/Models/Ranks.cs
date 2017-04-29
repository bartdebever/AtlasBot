using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;

namespace DataLibary.Models
{
    public static class Ranks
    {
        public static List<string> BasicRanks()
        {
            List<string> result = new List<string>();
            result.Add("Bronze");
            result.Add("Silver");
            result.Add("Gold");
            result.Add("Platinum");
            result.Add("Diamond");
            result.Add("Master");
            result.Add("Challenger");
            return result;
        }

        public static List<string> QueueRanks()
        {
            List<string> result = new List<string>();
            for (int i = 0; i < 3; i++)
            {
                foreach (string s in BasicRanks())
                {
                    if (i == 0)
                    {
                        result.Add("Solo " + s);
                    }
                    else if (i == 1)
                    {
                        result.Add("Flex " + s);
                    }
                    else if (i == 2)
                    {
                        result.Add("3v3 " + s);
                    }
                }
            }
            return result;
        }

        public static List<string> DivisionRanks()
        {
            List<string> result = new List<string>();
            foreach (string rank in BasicRanks())
            {
                if (rank != "Master" && rank != "Challenger")
                {
                    result.Add(rank + "V");
                    result.Add(rank + "IV");
                    result.Add(rank + "III");
                    result.Add(rank + "II");
                    result.Add(rank + "I");

                }
                else
                {
                    result.Add(rank);
                }
                
            }
            return result;
        }
    }
}
