using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DataLibary.Models;
using DataLibary.MSSQLContext;
using EmbededBot;

namespace WebUI.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        public ActionResult Index()
        {
            var user = Program.Client.GetUser(111211693870161920);
            return View(user);
        }
        [HttpPost]
        public ActionResult Detail(int id)
        {
            User user = new UserRepo(new UserContext()).GetUserById(id);
            return View(user);
        }
    }
}