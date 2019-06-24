using SeriesApp.CustomLibraries;
using SeriesApp.Models;
using System;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace SeriesApp.Controllers
{
    [AllowAnonymous]
    public class AuthController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            var db = new MainDbContext();
            return View(db.Series.Where(x => x.Public == true).ToList());
        }
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(Users model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            using (var db = new MainDbContext())
            {
                var emailCheck = db.Users.FirstOrDefault(u => u.Email == model.Email);
                var getPassword = db.Users.Where(u => u.Email == model.Email).Select(u => u.Password);
                var materializePassword = getPassword.ToList();
                var password = materializePassword[0];
                var decryptedPassword = CustomDecrypt.Decrypt(password);

                if (model.Email != null && model.Password == decryptedPassword)
                {
                    var getName = db.Users.Where(u => u.Email == model.Email).Select(u => u.Name);
                    var materializeName = getName.ToList();
                    var name = materializeName[0];

                    var getEmail = db.Users.Where(u => u.Email == model.Email).Select(u => u.Email);
                    var materializeEmail = getEmail.ToList();
                    var email = materializeEmail[0];

                    var identity = new ClaimsIdentity(new[] {
                            new Claim(ClaimTypes.Name, name),
                            new Claim(ClaimTypes.Email, email)
                     }, "ApplicationCookie");
                    var ctx = Request.GetOwinContext();
                    var authManager = ctx.Authentication;
                    authManager.SignIn(identity);
                    return RedirectToAction("Index", "Home");
                }
            }
            ModelState.AddModelError("", "Invalid email or password");

            return View(model);
        }
        public ActionResult Logout()
        {
            var ctx = Request.GetOwinContext();
            var authManager = ctx.Authentication;

            authManager.SignOut("ApplicationCookie");
            return RedirectToAction("Login", "Auth");
        }
        public ActionResult Registration()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Registration(Users model)
        {
            if (string.IsNullOrEmpty(model.Name))
            {
                ModelState.AddModelError("Name", "The Name field is required.");
            }
            if (ModelState.IsValid)
            {
                using (var db = new MainDbContext())
                {
                    var queryUser = db.Users.FirstOrDefault(u => u.Email == model.Email);
                    if (queryUser == null)
                    {
                        var encryptedPassword = CustomEnrypt.Encrypt(model.Password);
                        var user = db.Users.Create();
                        user.Email = model.Email;
                        user.Password = encryptedPassword;
                        user.Name = model.Name;
                        db.Users.Add(user);
                        db.SaveChanges();
                    }
                    else
                    {
                        return RedirectToAction("Registration");
                    }
                }
            }
            else
            {
                ModelState.AddModelError("", "One or more fields have been");
            }
            return View();
        }
    }
}