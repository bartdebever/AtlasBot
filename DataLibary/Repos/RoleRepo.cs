using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLibary.Data;
using DataLibary.Models;

namespace DataLibary.Repos
{
    public class RoleRepo
    {
        private IRoleContext context;

        public RoleRepo(IRoleContext context)
        {
            this.context = context;
        }
        Role GetRoleById(int id)
        {
            return context.GetRoleById(id);
        }

        List<Role> GetAllRoles()
        {
            return context.GetAllRoles();
        }

        List<Role> GetRolesPerServer(Server server)
        {
            return context.GetRolesPerServer(server);
        }
    }
}
