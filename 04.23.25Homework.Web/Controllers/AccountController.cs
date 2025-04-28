using _04._23._25Homework.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace _04._23._25Homework.Web.Controllers
{
    public class AccountController : Controller
    {
        private string _connectionString = @"Data Source=.\sqlexpress;Initial Catalog=SimpleAds2;Integrated Security=true;Trust Server Certificate=true;";

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login()
        {
            Console.WriteLine("in regular login");
            if (TempData["message"] != null)
            {
                Console.WriteLine("inside if");
                ViewBag.Message = TempData["message"];
            }
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            Console.WriteLine("in post login");
            var repo = new AdRepository(_connectionString);
            var user = repo.Login(email, password);
            if (user == null)
            {
                TempData["message"] = "Invalid login";
                return Redirect("/account/login");
            }

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Email, email)
            };

            HttpContext.SignInAsync(new ClaimsPrincipal(
                    new ClaimsIdentity(claims, "Cookies", ClaimTypes.Email, "roles"))
                ).Wait();

            return Redirect("/home");
        }

        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SignUp(string name, string email, string password)
        {
            AdRepository repo = new(_connectionString);
            User user = new()
            {
                Name = name,
                Email = email
            };
            repo.NewUser(user, password);
            return Redirect("/account/login");
        }

        public IActionResult Logout()
        {
            HttpContext.SignOutAsync().Wait();
            return Redirect("/");
        }
    }
}
