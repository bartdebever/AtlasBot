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
    public class CoachController : Controller
    {
        // GET: Coach
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Details(int id)
        {
            var coachModel = CoachlistHolder.list.Single(c => c.Id == id);
            return View(coachModel);
        }

        public ActionResult Edit(int id)
        {
            //check login
            CoachlistHolder.Update();
            var coachModel = CoachlistHolder.list.Single(c => c.Id == id);
            return View(coachModel);
        }
        [HttpPost]
        public ActionResult Edit(int id, string bio, string name, string timezone, string availability, ICollection<string> champions)
        {
            var coachModel = CoachlistHolder.list.Single(c => c.Id == id);
            return View(coachModel);
        }
    }
}