using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Finanzas.Data;
using Finanzas.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection.Internal;

namespace Finanzas.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        // GET
        [HttpGet]
        public ActionResult Index()
        {
            DashboardData oDashboardData = new DashboardData();
            string idUser = User.Claims.Where(c => c.Type == "idUser").Select(c => c.Value).FirstOrDefault();
            string currentMonth = User.Claims.Where(c => c.Type == "currentMonth").Select(c => c.Value).FirstOrDefault() ?? "";
            var month = oDashboardData.GetCurrentOrSelectedMonth(currentMonth);
            ViewBag.Months = oDashboardData.GetMonths();
            ViewBag.Categories = oDashboardData.GetCategories(Int32.Parse(idUser));
            ViewBag.PaymentMethods = oDashboardData.GetPaymentMethods(Int32.Parse(idUser));
            ViewBag.Bills = oDashboardData.GetBills(Int32.Parse(idUser), month.Item1);
            ViewBag.CurrentMonth = month;
            ViewBag.Icons = oDashboardData.GetIcons();
            ViewBag.Balance = oDashboardData.GetBalance(Int32.Parse(idUser), month.Item1);
            
            return View("index");
        }
        
        [HttpPost]
        public ActionResult AddCategory(Categories oCategories)
        {
            string idUser = User.Claims.Where(c => c.Type == "idUser").Select(c => c.Value).FirstOrDefault();
            
            DashboardData oDashboardData = new DashboardData();
            
            oCategories.idUser = Int32.Parse(idUser);
            
            oDashboardData.AddCategory(oCategories);
            

            return RedirectToAction("Index", "Dashboard");
        }
        
        [HttpPost]
        public ActionResult AddPaymentMethod(PaymentMethods oPaymentMethods)
        {
            string idUser = User.Claims.Where(c => c.Type == "idUser").Select(c => c.Value).FirstOrDefault();
            
            DashboardData oDashboardData = new DashboardData();
            
            oPaymentMethods.idUser = Int32.Parse(idUser);
            
            oDashboardData.AddPaymentMethod(oPaymentMethods);

            return RedirectToAction("Index", "Dashboard");
        }

        [HttpPost]
        public IActionResult SelectMonth(Month Month)
        {
            
            
            var claimsPrincipal = (ClaimsPrincipal)User;
            var existingClaims = claimsPrincipal.Claims.ToList();
            var existingClaim = existingClaims.FirstOrDefault(c => c.Type == "currentMonth");
            if (existingClaim != null)
            {
                existingClaims.Remove(existingClaim);
            }
            existingClaims.Add(new Claim("currentMonth", Month.Name));
            var newIdentity = new ClaimsIdentity(existingClaims, CookieAuthenticationDefaults.AuthenticationScheme);
            var newPrincipal = new ClaimsPrincipal(newIdentity);

            HttpContext.SignInAsync(newPrincipal);
            
            
            
            return RedirectToAction("Index", "Dashboard");
        }

        [HttpPost]
        public ActionResult AddBill()
        {
            DashboardData oDashboardData = new DashboardData();
            
            string idUser = User.Claims.Where(c => c.Type == "idUser").Select(c => c.Value).FirstOrDefault();
            string currentMonth = User.Claims.Where(c => c.Type == "currentMonth").Select(c => c.Value).FirstOrDefault() ?? "";
            var month = oDashboardData.GetCurrentOrSelectedMonth(currentMonth);

            Bills bills = new Bills();

            bills.idUser = Int32.Parse(idUser);
            bills.idMonth = month.Item1;
            
            
            oDashboardData.AddBill(bills);


            return RedirectToAction("Index", "Dashboard");
        }
        
        public void SaveBills(List<Bills> bills)
        {
            DashboardData oDashboardData = new DashboardData();
            string idUser = User.Claims.Where(c => c.Type == "idUser").Select(c => c.Value).FirstOrDefault();
            string currentMonth = User.Claims.Where(c => c.Type == "currentMonth").Select(c => c.Value).FirstOrDefault() ?? "";
            var month = oDashboardData.GetCurrentOrSelectedMonth(currentMonth);
            bills.ForEach(c => c.idUser = Int32.Parse(idUser));
            bills.ForEach(c => c.idMonth = month.Item1);
            
             oDashboardData.SaveBills(bills);
        }
        
        public void SaveBalance(Balance balance)
        {
            DashboardData oDashboardData = new DashboardData();
            string idUser = User.Claims.Where(c => c.Type == "idUser").Select(c => c.Value).FirstOrDefault();
            string currentMonth = User.Claims.Where(c => c.Type == "currentMonth").Select(c => c.Value).FirstOrDefault() ?? "";
            var month = oDashboardData.GetCurrentOrSelectedMonth(currentMonth);
            balance.idUser = Int32.Parse(idUser);
            balance.idMonth = month.Item1;
            
            oDashboardData.SaveBalance(balance);

        }

        [HttpPost]
        public ActionResult SubmitBothForms(Balance balance, List<Bills> bills)
        {
            this.SaveBills(bills);
            this.SaveBalance(balance);
            return RedirectToAction("Index", "Dashboard");
        }
        
        [HttpPost]
        
        public ActionResult DeleteBill(int idBill)
        {
            DashboardData oDashboardData = new DashboardData();
            oDashboardData.DeleteBill(idBill);

            return RedirectToAction("Index", "Dashboard");
        }

    }
}