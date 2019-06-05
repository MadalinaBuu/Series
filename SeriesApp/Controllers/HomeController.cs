using SeriesApp.Models;
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

                    if (Request.Form["check_public"] != null) dbSerial.Public = 1;
                    else dbSerial.Public = 0;

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
        public ActionResult Edit(Series list)
        {
            var db = new MainDbContext();

            if (ModelState.IsValid)
            {
                list.Name = list.Name;
                if (list.Public == 1) { list.Public = 1; }
                else { list.Public = 0; }

                db.Entry(list).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(list);
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
    }
}