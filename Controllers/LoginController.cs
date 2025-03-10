using Microsoft.AspNetCore.Mvc;

namespace ASM_SIMS.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View(); // View tu dong di chuyen vao package Views/Login/Index.cshtml
        }
    }
}
