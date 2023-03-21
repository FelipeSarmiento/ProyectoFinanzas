using System.Security.Claims;
using Finanzas.Data;
using Finanzas.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Finanzas.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        public DashboardData oDashboardData = new DashboardData();

        // GET
        [HttpGet]
        public ActionResult Index()
        {
            DashboardData oDashboardData = new DashboardData();
            string idUser = User.Claims.Where(c => c.Type == "idUser").Select(c => c.Value).FirstOrDefault();
            string currentMonth =
                User.Claims.Where(c => c.Type == "currentMonth").Select(c => c.Value).FirstOrDefault() ?? "";
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
        public IActionResult SelectMonth(int idMonth)
        {
            string nameMonth = "";
            var months = oDashboardData.GetMonths();
            months.ForEach(c =>
            {
                if (c.idMonth == idMonth)
                {
                    nameMonth = c.Name;
                }
            });

            var claimsPrincipal = (ClaimsPrincipal)User;
            var existingClaims = claimsPrincipal.Claims.ToList();
            var existingClaim = existingClaims.FirstOrDefault(c => c.Type == "currentMonth");
            if (existingClaim != null)
            {
                existingClaims.Remove(existingClaim);
            }

            existingClaims.Add(new Claim("currentMonth", nameMonth));
            var newIdentity = new ClaimsIdentity(existingClaims, CookieAuthenticationDefaults.AuthenticationScheme);
            var newPrincipal = new ClaimsPrincipal(newIdentity);

            HttpContext.SignInAsync(newPrincipal);


            return RedirectToAction("Index", "Dashboard");
        }

        [HttpPost]
        public IActionResult AddBill()
        {
            DashboardData oDashboardData = new DashboardData();

            string idUser = User.Claims.Where(c => c.Type == "idUser").Select(c => c.Value).FirstOrDefault();
            string currentMonth =
                User.Claims.Where(c => c.Type == "currentMonth").Select(c => c.Value).FirstOrDefault() ?? "";
            var month = oDashboardData.GetCurrentOrSelectedMonth(currentMonth);

            Bills bills = new Bills();

            bills.idUser = Int32.Parse(idUser);
            bills.idMonth = month.Item1;


            oDashboardData.AddBill(bills);

            var billsList = oDashboardData.GetBills(Int32.Parse(idUser), month.Item1);
            var iconsList = oDashboardData.GetIcons();
            var categoriesList = oDashboardData.GetCategories(Int32.Parse(idUser));
            var paymentMethodsList = oDashboardData.GetPaymentMethods(Int32.Parse(idUser));

            Tuple<List<Bills>, List<Icons>, List<Categories>, List<PaymentMethods>> tuple =
                new Tuple<List<Bills>, List<Icons>, List<Categories>, List<PaymentMethods>>(billsList, iconsList,
                    categoriesList, paymentMethodsList);


            return PartialView("PartialViews/_DashboardContent", tuple);
        }

        public void SaveBills(List<Bills> bills)
        {
            DashboardData oDashboardData = new DashboardData();
            string idUser = User.Claims.Where(c => c.Type == "idUser").Select(c => c.Value).FirstOrDefault();
            string currentMonth =
                User.Claims.Where(c => c.Type == "currentMonth").Select(c => c.Value).FirstOrDefault() ?? "";
            var month = oDashboardData.GetCurrentOrSelectedMonth(currentMonth);
            bills.ForEach(c => c.idUser = Int32.Parse(idUser));
            bills.ForEach(c => c.idMonth = month.Item1);

            oDashboardData.SaveBills(bills);
        }

        public void SaveBalance(Balance balance)
        {
            DashboardData oDashboardData = new DashboardData();
            string idUser = User.Claims.Where(c => c.Type == "idUser").Select(c => c.Value).FirstOrDefault();
            string currentMonth =
                User.Claims.Where(c => c.Type == "currentMonth").Select(c => c.Value).FirstOrDefault() ?? "";
            var month = oDashboardData.GetCurrentOrSelectedMonth(currentMonth);
            balance.idUser = Int32.Parse(idUser);
            balance.idMonth = month.Item1;

            oDashboardData.SaveBalance(balance);
            oDashboardData.UpdateBalance(balance.idUser, balance.idMonth);
        }

        [HttpPost]
        public IActionResult SubmitBothForms(Balance balance, List<Bills> bills)
        {
            this.SaveBills(bills);
            this.SaveBalance(balance);

            string idUser = User.Claims.Where(c => c.Type == "idUser").Select(c => c.Value).FirstOrDefault();
            string currentMonth =
                User.Claims.Where(c => c.Type == "currentMonth").Select(c => c.Value).FirstOrDefault() ?? "";
            var month = oDashboardData.GetCurrentOrSelectedMonth(currentMonth);
            var billsList = oDashboardData.GetBills(Int32.Parse(idUser), month.Item1);
            var iconsList = oDashboardData.GetIcons();
            var categoriesList = oDashboardData.GetCategories(Int32.Parse(idUser));
            var paymentMethodsList = oDashboardData.GetPaymentMethods(Int32.Parse(idUser));

            Tuple<List<Bills>, List<Icons>, List<Categories>, List<PaymentMethods>> tuple =
                new Tuple<List<Bills>, List<Icons>, List<Categories>, List<PaymentMethods>>(billsList, iconsList,
                    categoriesList, paymentMethodsList);
            var partialViewDashboardContent = PartialView("PartialViews/_DashboardContent", tuple);
            var partialViewBalance = PartialView("PartialViews/_BalanceForm", balance);

            var partials = new JsonResult(new { partialViewDashboardContent, partialViewBalance });
            return partials;
        }

        [HttpPost]
        public IActionResult DeleteBill(int idBill)
        {
            string idUser = User.Claims.Where(c => c.Type == "idUser").Select(c => c.Value).FirstOrDefault();
            string currentMonth =
                User.Claims.Where(c => c.Type == "currentMonth").Select(c => c.Value).FirstOrDefault() ?? "";
            var month = oDashboardData.GetCurrentOrSelectedMonth(currentMonth);
            oDashboardData.DeleteBill(idBill);
            oDashboardData.UpdateBalance(Int32.Parse(idUser), month.Item1);
            var billsList = oDashboardData.GetBills(Int32.Parse(idUser), month.Item1);
            var iconsList = oDashboardData.GetIcons();
            var categoriesList = oDashboardData.GetCategories(Int32.Parse(idUser));
            var paymentMethodsList = oDashboardData.GetPaymentMethods(Int32.Parse(idUser));

            Tuple<List<Bills>, List<Icons>, List<Categories>, List<PaymentMethods>> tuple =
                new Tuple<List<Bills>, List<Icons>, List<Categories>, List<PaymentMethods>>(billsList, iconsList,
                    categoriesList, paymentMethodsList);


            return PartialView("PartialViews/_DashboardContent", tuple);
        }

        [HttpPost]
        public IActionResult GetBalanceForm()
        {
            string idUser = User.Claims.Where(c => c.Type == "idUser").Select(c => c.Value).FirstOrDefault();
            string currentMonth =
                User.Claims.Where(c => c.Type == "currentMonth").Select(c => c.Value).FirstOrDefault() ?? "";
            var month = oDashboardData.GetCurrentOrSelectedMonth(currentMonth);
            var balance = oDashboardData.GetBalance(Int32.Parse(idUser), month.Item1);
            return PartialView("PartialViews/_BalanceForm", balance);
        }
    }
}