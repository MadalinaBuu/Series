using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SeriesApp.Controllers
{
    public class EpisodesController : Controller
    {
        // GET: Episodes
        public ActionResult Index(int id)
        {
            return View();
        }
        [HttpPost]
        public ActionResult Add()
        {
            return View();
        }
    }
}