using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DataLibary.Models;
using DataLibary.MSSQLContext;

namespace WebUI.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Detail(int id)
        {
            User user = new UserRepo(new UserContext()).GetUserById(id);
            return View(user);
        }
    }
}