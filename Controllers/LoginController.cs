using ASM_SIMS.Models;
using Microsoft.AspNetCore.Mvc;

namespace ASM_SIMS.Controllers
{
    public class LoginController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Index(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                //xu ly nhan du lieu tu form
                string email = model.email;
                string password = model.password;
                if (email.Equals("admin@gmail.com") && password.Equals("admin123"))
                {
                    //login thanh cong
                    return RedirectToAction("Index", "Dashboard");
                }
                else
                {
                    //login fail
                    ViewData["MessageLogin"] = "Account Invalid";

                }

            }
            return View(model);

        }
        [HttpPost]
        public IActionResult Logout()
        {
            return RedirectToAction("Index", "Login");
        }
    }
}
