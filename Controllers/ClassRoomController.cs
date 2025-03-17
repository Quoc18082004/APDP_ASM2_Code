using Microsoft.AspNetCore.Mvc;

namespace ASM_SIMS.Controllers
{
    public class ClassRoomController : Controller
    {
        public IActionResult Index()
        {
            ViewData["Title"] = "ClassRoom";// Nhận dữ liệu từ Model và truyền nó cho View

            return View();
        }

       
    }
}
