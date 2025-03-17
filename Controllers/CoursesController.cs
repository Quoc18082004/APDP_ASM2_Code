using Microsoft.AspNetCore.Mvc;

namespace ASM_SIMS.Controllers
{
    public class CoursesController : Controller
    {
        public IActionResult Index()
        {
            ViewData["Title"] = "Courses";// Nhận dữ liệu từ Model và truyền nó cho View
            ViewBag.NameCourse = "C#";
            ViewBag.TimeCourse = "6 months";
            ViewBag.PriceCourse = "100$";
            ViewBag.TeacherCourse = "Nguyen Anh Tuan";
            ViewBag.AddressCourse = "Ha Noi";
            ViewBag.PhoneCourse = "0123456789";
            ViewBag.EmailCourse = "C#@gmail.com";


            return View();
        }
    }
}
