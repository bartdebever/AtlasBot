using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using DataLibary.Models;
using DataLibary.MSSQLContext;
using EmbededBot;

namespace WebUI.Controllers
{
    public class LoLCoachController : ApiController
    {
        List<Coach> loMCoaches = new List<Coach>();
        private bool update = true;

        private List<Coach> LoMCoaches
        {
            get
            {
                if (update)
                {
                    loMCoaches = CoachlistHolder.list;
                    update = false;
                }
                return loMCoaches;
            }
        }
        public IEnumerable<Coach> GetLoMCoaches()
        {
            return LoMCoaches;
        }

        public IHttpActionResult GetLoMCoach(int id)
        {
            var coach = LoMCoaches.FirstOrDefault(c => c.Id == id);
            if (coach == null)
            {
                return NotFound();
            }
            return Ok(coach);
        }

        public IEnumerable<Coach> GetLomCoachesFilter(string id)
        {
            List<Coach> result = new List<Coach>();
            result = LoMCoaches.Where(c => c.Name.ToLower().Contains(id.ToLower()) || c.Champions.ToLower().Contains(id.ToLower())).ToList();
            foreach (var loMCoach in LoMCoaches)
            {
                if (!result.Contains(loMCoach))
                {
                    foreach (var role in loMCoach.Roles)
                    {
                        if (role.ToLower().Contains(id.ToLower()) && !result.Contains(loMCoach))
                        {
                            result.Add(loMCoach);
                        }
                    }
                }
            }
            return result;
        }
    }
}