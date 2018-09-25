using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using VacationsPortal.Models;

namespace VacationsPortal.Controllers
{
    public enum Privilege
    {
        Na,
        Admin,
        Vacations,
        Travel
    }

    public class HomeController : Controller
    {
        private readonly CIDvNEXtEntities _db = new CIDvNEXtEntities();
        
        private bool IsUserAuthenticated(ref Privilege privilege)
        {
            var loggedUserEmail = "v-gamoha@microsoft.com";
            //var loggedUserEmail = ClaimsPrincipal.Current.FindFirst(ClaimTypes.Name).Value;
            var authUser = _db.AuthUsers.FirstOrDefault(u => u.Email == loggedUserEmail);
            if (authUser?.Privilege != null)
            {
                privilege = (Privilege) authUser.Privilege;
                return true;
            }

            return false;

            //return true;
        }

        public ActionResult Index()
        {
            var privilege = Privilege.Na;
            if (IsUserAuthenticated(ref privilege))
            {
                switch (privilege)
                {
                    case Privilege.Admin:
                        return RedirectToAction("Index", "Employees");
                    case Privilege.Vacations:
                        return RedirectToAction("Index", "Employees");
                    case Privilege.Travel:
                        return RedirectToAction("Index", "Trips");
                }

                ViewBag.ErrorMsg = "Not authenticated user.";
                return View("Error");
            }

            ViewBag.ErrorMsg = "Not authenticated user.";
            return View("Error");
        }

        //public ActionResult About()
        //{
        //    ViewBag.Message = "Your application description page.";

        //    return View();
        //}

        //public ActionResult Contact()
        //{
        //    ViewBag.Message = "Your contact page.";

        //    return View();
        //}
    }
}