using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using TvShows.Models;
using TvShows.Services;

namespace TvShows.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        Claim sessionEmail = ClaimsPrincipal.Current.FindFirst(ClaimTypes.Email);
        public SeriesEntities db;

        public UtilService utils;

        public HomeController()
        {
            db = new SeriesEntities();
            utils = new UtilService();
        }


        public ActionResult Index()
        {
            return View(db.Series.ToList());
        }
        [HttpPost]
        public ActionResult Index(Series list, string Seen)
        {
            if (ModelState.IsValid)
            {
                List<int> userId = db.Users.Where(u => u.Email == sessionEmail.Value).Select(u => u.ID).ToList();
                string name = Request.Form["name"];

                var dbSerial = db.Series.Create();
                dbSerial.User_ID = userId[0];
                dbSerial.Name = name;

                dbSerial.PublicS = Convert.ToInt32(Request.Form["Public"]);
                dbSerial.Seen = 0;

                string source = "Images/" + utils.RemoveSpecialCharacters(name + Path.GetExtension(Request.Form["source"])).ToLower();
                dbSerial.Source = source;

                bool serialAlreadyExists = db.Series.Any(s => s.Source == source && s.Name == name);

                if (!string.IsNullOrEmpty(name) && !serialAlreadyExists)
                {
                    SaveImage(Request.Form["source"], name);
                    db.Series.Add(dbSerial);
                    db.SaveChanges();
                }
            }
            else
            {
                ModelState.AddModelError("", "Incorrect format has been placed");
            }
            return View(db.Series.ToList());
        }
        public void SaveImage(string url, string filename)
        {
            byte[] data;
            string ext = string.Empty;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Ssl3 | (SecurityProtocolType)3072;
            using (WebClient client = new WebClient())
            {
                data = client.DownloadData(url);
                ext = Path.GetExtension(url);
            }
            try
            {
                System.IO.File.WriteAllBytes(Server.MapPath("Images/") + utils.RemoveSpecialCharacters(filename).ToLower() + ext, data);
            }
            catch (Exception) { }
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            Series serial = new Series();
            serial = db.Series.Find(id);
            return View(serial);
        }
        [HttpPost]
        public ActionResult Edit(Series serial)
        {
            if (ModelState.IsValid)
            {
                db.Entry(serial).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(serial);
        }
        [HttpGet]
        public ActionResult Delete(int id)
        {
            Series serial = db.Series.Find(id);
            if (serial == null)
                return HttpNotFound();

            db.Series.Remove(serial);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult Seen(int? id)
        {
            Series serial = db.Series.Find(id);
            if (id != null)
            {
                if (ModelState.IsValid)
                {
                    serial.Seen = 1;

                    db.Entry(serial).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        public JsonResult AddEpisodes(int id)
        {
            bool result = false;
            for (int i = 1; i <= Convert.ToInt32(Request.Form["episodesNo"]); i++)
            {
                var dbEpisode = db.Episodes.Create();
                dbEpisode.No = i;
                dbEpisode.Season = Convert.ToInt32(Request.Form["seasonNo"]);
                dbEpisode.Title = "Episode " + i;
                dbEpisode.Serial = id;
                dbEpisode.Seen = 0;
                db.Episodes.Add(dbEpisode);
                db.SaveChanges();
                result = true;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Carousel()
        {
            List<Series> series = new List<Series>();
            int id = 0;
            if (!User.Identity.IsAuthenticated)
            {
                series = db.Series.Where(s => s.PublicS == 1).ToList();
            }
            else
            {
                try
                {
                    id = Convert.ToInt32(Url.RequestContext.HttpContext.Request.UrlReferrer.Segments[3]);
                }
                catch (Exception ex) { }
                series = id > 0 ? db.Series.Where(s => s.ID == id).ToList() : db.Series.ToList();
            }
            return View("~/Views/Home/_Carousel.cshtml", series);
        }
        //public ActionResult GetEpisode(int? id)
        //{
        //    List<Episode> series = db.Episodes.Where(s => s.Serial == id).ToList();
        //    return PartialView("~/Views/Home/_SeasonsAndEpisodes.cshtml", series);
        //}
        [HttpPost]
        public ActionResult SeenEpisode(int? id)
        {
            bool result = false;
            Episode episode = db.Episodes.Find(id);
            if (id != null)
            {
                if (ModelState.IsValid)
                {
                    episode.Seen = 1;

                    db.Entry(episode).State = EntityState.Modified;
                    db.SaveChanges();
                    result = true;
                }
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}