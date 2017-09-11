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
            var coachModel = CoachlistHolder.list.Single(c => c.Id == id);
            return View(coachModel);
        }

        public ActionResult Edit(Coach coach)
        {
            return View(coach);
        }
    }
}