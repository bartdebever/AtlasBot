using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLibary.Models
{
    public static class Roles
    {
        public static List<string> NormalRoles()
        {
            List<string> result = new List<string>();
            result.Add("Top");
            result.Add("Jungle");
            result.Add("Mid");
            result.Add("ADC");
            result.Add("Support");
            result.Add("Fill");
            return result;
        }

        public static List<string> MainRoles()
        {
            List<string> result = new List<string>();
            foreach (var role in NormalRoles())
            {
                result.Add(role + "-Main");
            }
            return result;
        }

        public static List<string> MainsRoles()
        {
            List<string> result = new List<string>();
            foreach (var role in NormalRoles())
            {
                result.Add(role + "-Mains");
            }
            return result;

        }
    }
}
