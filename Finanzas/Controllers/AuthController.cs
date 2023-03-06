using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Finanzas.Data;
using Finanzas.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Finanzas.Controllers
{
    public class AuthController : Controller
    {
        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return View("LoginPage");
        }

        [HttpPost]
        public async Task<IActionResult> Index(Users oUser, string returnUrl)
        {
            var oUserLogin = new UsersData();
            var result = oUserLogin.LoginUser(oUser);
            if (result.Item1)
            {
                var claims = new List<Claim>();
                claims.Add(new Claim("idUser", result.Item3.idUser.ToString()));
                claims.Add(new Claim(ClaimTypes.Name, result.Item3.Name));
                claims.Add(new Claim(ClaimTypes.Email, result.Item3.Email));

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);


                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                await HttpContext.SignInAsync(claimsPrincipal);
               
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return LocalRedirect(returnUrl);
                }
                return RedirectToAction("Index", "Home");
                
            }

            ViewBag.Message = result.Item2;

            return View("LoginPage");
        }

        public ActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return View("RegisterPage");
        }

        [HttpPost]
        public ActionResult Register(Users oUser)
        {
            var oUserRegister = new UsersData();
            var result = oUserRegister.RegisterUser(oUser);
            if (result.Item1)
            {
                ViewBag.Message = result.Item2;
                return View("LoginPage");
            }
            else
            {
                ViewBag.Message = result.Item2;
                return View("RegisterPage");
            }
        }

        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}