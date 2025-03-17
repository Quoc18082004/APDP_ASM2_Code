using Microsoft.AspNetCore.Mvc;
using ASM_SIMS.Models;

namespace ASM_SIMS.Controllers
{
    public class TeacherController : Controller
    {
        public IActionResult Index()
        {
            ViewData["Title"] = "Teacher";// Nhận dữ liệu từ Model và truyền nó cho View
            ViewData["NameTeacher"] = "Nguyen Anh Tuan";
            ViewData["AgeTeacher"] = 30;
            ViewData["AddressTeacher"] = "Ha Noi";
            ViewData["PhoneTeacher"] = "0123456789";
            ViewData["EmailTeacher"] = "anhtuannguyen21@gmail.com";
            ViewData["GenderTeacher"] = "Male";

            TeacherViewModel teacher = new TeacherViewModel
            {
                Id = 1,
                Name = "Nguyen Anh Tuan",
                Age = 30,
                Address = "Ha Noi",

            };

            List<TeacherViewModel> teacherList = new List<TeacherViewModel>()
            {
                new TeacherViewModel { Id = 1, Name = "Nguyen Anh Tuan", Age = 30, Address = "Ha Noi" },
                new TeacherViewModel { Id = 2, Name = "Truong A Hoang", Age = 30, Address = "Ha Nam" },
                new TeacherViewModel { Id = 3, Name = "Nguyen Thai Tuan", Age = 30, Address = "Ha Giang" },
                new TeacherViewModel { Id = 4, Name = "Thai Anh Quang", Age = 30, Address = "Bac Giang" },
                new TeacherViewModel { Id = 5, Name = "Tran Anh Minh", Age = 30, Address = "Bac Ninh" },

            };

            return View(teacherList);

        }
    }
}
