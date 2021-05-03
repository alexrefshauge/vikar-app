using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace vikar_app.Controllers
{
    public class HjemController : Controller
    {
        
        DataHandler DH = new DataHandler();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Om()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Kontakt()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult Profil()
        {
            return View();
        }
        public ActionResult RedigerProfil()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GemProfil(string fnavn, string enavn, int tlf, string email, int område, int alder, string bio)
        {
            DH.AddValues(fnavn, enavn, alder, tlf, email, område, bio);

            return Redirect("/hjem/profil");
        }

        [HttpPost]
        public ActionResult OpretBruger(string fnavn, string enavn, string email, string password)
        {
            if (!DH.CheckEmail(email))
            {
                DH.AddUser(fnavn, enavn, email, password);
                return Redirect("/hjem/login");
            } else
            {
                return Redirect("/email-taget");
            }
        }

        [HttpPost]
        public ActionResult LoginBruger(string email, string password)
        {
            if (DH.CheckLogin(email, password))
            {
                //gem email i cookie
                HttpCookie cookie = new HttpCookie("Email");
                cookie.Value = email;
                cookie.Expires = DateTime.Now.AddDays(1);
                HttpContext.Response.Cookies.Add(cookie);

                return Redirect("/hjem/profil");
            } else{

                //Email findes ikke eller password matcher ikke
                return Redirect("/hjem/login");
            }
        }
        public ActionResult Gennemse(string searchString)
        {
            System.Diagnostics.Debug.WriteLine(searchString);
            if (searchString != null)
            {
                if (searchString.Length > 0)
                { 
                ViewBag.HitList = DH.SearchByName(searchString);
                } else
                {
                    searchString = null;
                }
            }

            return View();
        }
    }
}