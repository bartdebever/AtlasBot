using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DataLibary.Models;
using DataLibary.MSSQLContext;

namespace WebUI.Controllers
{
    public class CoachController : Controller
    {
        // GET: Coach
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Details(int id)
        {
            //temporary
            var coachModel = new CoachRepo(new CoachContext()).GetAllCoaches().Single(c => c.Id == id);
            return View(coachModel);
        }
    }
}