using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Finanzas.Data;
using Finanzas.Models;

namespace Finanzas.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        // GET
        [HttpGet]
        public ActionResult Index()
        {
            string idUser = User.Claims.Where(c => c.Type == "idUser").Select(c => c.Value).FirstOrDefault();
            DashboardData oDashboardData = new DashboardData();
            ViewBag.Months = oDashboardData.GetMonths();
            ViewBag.Categories = oDashboardData.GetCategories(Int32.Parse(idUser));
            ViewBag.PaymentMethods = oDashboardData.GetPaymentMethods(Int32.Parse(idUser));
            ViewBag.CurrentMonth = oDashboardData.GetCurrentOrSelectedMonth(ViewBag.NameCurrentMonth ?? "");
            
            return View("index");
        }
        
        [HttpPost]
        public ActionResult AddCategory(Categories oCategories)
        {
            string idUser = User.Claims.Where(c => c.Type == "idUser").Select(c => c.Value).FirstOrDefault();
            
            DashboardData oDashboardData = new DashboardData();
            
            oCategories.idUser = Int32.Parse(idUser);
            
            oDashboardData.AddCategory(oCategories);
            
            ViewBag.Categories = oDashboardData.GetCategories(Int32.Parse(idUser));

            return View("index");
        }
        
        [HttpPost]
        public ActionResult AddPaymentMethod(PaymentMethods oPaymentMethods)
        {
            string idUser = User.Claims.Where(c => c.Type == "idUser").Select(c => c.Value).FirstOrDefault();
            
            DashboardData oDashboardData = new DashboardData();
            
            oPaymentMethods.idUser = Int32.Parse(idUser);
            
            oDashboardData.AddPaymentMethod(oPaymentMethods);

            return Index();
        }

        [HttpPost]
        public ActionResult SelectMonth(Month Month)
        {
            ViewBag.NameCurrentMonth = Month.Name;
            return Index();
        }
    }
}