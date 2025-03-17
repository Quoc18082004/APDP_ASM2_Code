using Microsoft.AspNetCore.Mvc;

namespace ASM_SIMS.Controllers
{
    public class StudentController : Controller
    {
        public IActionResult Index()
        {
            ViewData["Title"] = "Student";// Nhận dữ liệu từ Model và truyền nó cho View

            return View();
        }
    }
}
