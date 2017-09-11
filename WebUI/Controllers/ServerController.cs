using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using AtlasBot;

namespace WebUI.Controllers
{
    public class ServerController : Controller
    {
        private bool botOn = false;
        // GET: Server
        public ActionResult Index()
        {
            return RedirectToAction("Index");
        }
    }
}