using _04._23._25Homework.Data;
using _04._23._25Homework.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace _04._23._25Homework.Web.Controllers
{
    public class HomeController : Controller
    {
        private string _connectionString = @"Data Source=.\sqlexpress;Initial Catalog=SimpleAds2;Integrated Security=true;Trust Server Certificate=true;";

        public IActionResult Index()
        {
            AdRepository repo = new(_connectionString);
            AdsViewModel vm = new()
            {
                Ads = repo.GetAds(),
            };
            if (User.Identity.IsAuthenticated)
            {
                vm.UserId = repo.UserByEmail(User.Identity.Name).Id;
            }
            return View(vm);
        }

        [Authorize]
        public IActionResult MyAccount()
        {
            AdRepository repo = new(_connectionString);
            int UserId = repo.UserByEmail(User.Identity.Name).Id;
            AdsViewModel vm = new()
            {
                Ads = repo.GetAds(UserId)
            };
            return View(vm);
        }

        public IActionResult NewAd()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Redirect("/account/login");
            }
            return View();
        }

        [Authorize]
        [HttpPost]
        public IActionResult NewAd(SimpleAd ad)
        {
            AdRepository repo = new(_connectionString);
            ad.UserId = repo.UserByEmail(User.Identity.Name).Id;    
            repo.NewAd(ad);
            return Redirect("/");
        }

        [Authorize]
        [HttpPost]
        public IActionResult DeleteAd(int id)
        {
            AdRepository repo = new(_connectionString);
            repo.DeleteAd(id);
            return Redirect("/");
        }
    }
}
