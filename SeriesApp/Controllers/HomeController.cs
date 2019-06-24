﻿using SeriesApp.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Web.Mvc;

namespace SeriesApp.Controllers
{
    public class HomeController : Controller
    {
        Claim sessionEmail = ClaimsPrincipal.Current.FindFirst(ClaimTypes.Email);

        public ActionResult Index()
        {
            var db = new MainDbContext();
            return View(db.Series.ToList());
        }
        [HttpPost]
        public ActionResult Index(Series list, string Seen)
        {
            if (ModelState.IsValid)
            {
                using (var dbSeries = new MainDbContext())
                {
                    var userId = dbSeries.Users.Where(u => u.Email == sessionEmail.Value).Select(u => u.Id).ToList();
                    string name = Request.Form["name"];

                    var dbSerial = dbSeries.Series.Create();

                    dbSerial.User_ID = userId[0];
                    dbSerial.Name = name;

                    dbSerial.Public = Convert.ToBoolean(Request.Form["Public"]) == true;

                    string source = "Images/" + Utile.RemoveSpecialCharacters(name + Path.GetExtension(Request.Form["source"])).ToLower();
                    dbSerial.Source = source;

                    bool serialAlreadyExists = dbSeries.Series.Any(s => s.Source == source && s.Name == name);

                    if (!String.IsNullOrEmpty(name) && !serialAlreadyExists)
                    {
                        SaveImage(Request.Form["source"], name);
                        dbSeries.Series.Add(dbSerial);
                        dbSeries.SaveChanges();
                    }
                }
            }
            else
            {
                ModelState.AddModelError("", "Incorrect format has been placed");
            }
            var listTable = new MainDbContext();
            return View(listTable.Series.ToList());
        }
        public void SaveImage(string url, string filename)
        {
            byte[] data;
            string ext = String.Empty;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Ssl3 | (SecurityProtocolType)3072;
            using (WebClient client = new WebClient())
            {
                data = client.DownloadData(url);
                ext = Path.GetExtension(url);
            }
            try
            {
                System.IO.File.WriteAllBytes(Server.MapPath("Images/") + Utile.RemoveSpecialCharacters(filename).ToLower() + ext, data);
            }
            catch(Exception ex) { }
        }


        [HttpGet]
        public ActionResult Edit(int id)
        {
            var db = new MainDbContext();
            var model = new Series();
            model = db.Series.Find(id);
            return View(model);
        }
        [HttpPost]
        public ActionResult Edit(Series serial)
        {
            var db = new MainDbContext();

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
            var db = new MainDbContext();
            var model = db.Series.Find(id);

            if (model == null)
            {
                return HttpNotFound();
            }

            db.Series.Remove(model);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult Seen(int? id)
        {
            var db = new MainDbContext();
            var list = db.Series.Find(id);
            if (id != null)
            {
                if (ModelState.IsValid)
                {
                    list.Seen = 1;

                    db.Entry(list).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        public JsonResult AddEpisodes(int id)
        {
            var db = new MainDbContext();
            var result = false;
            using (db)
            {
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
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Carousel()
        {
            MainDbContext db = new MainDbContext();
            List<Series> series = new List<Series>();
            int id = 0;
            if (!User.Identity.IsAuthenticated)
            {
                series = db.Series.Where(s => s.Public == true).ToList();
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
    }
}