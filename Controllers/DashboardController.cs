using Microsoft.AspNetCore.Mvc;

namespace ASM_SIMS.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {

            ViewData["Title"] = "Dashboard";// Nhận dữ liệu từ Model và truyền nó cho View

            return View();
        }
    }
}
