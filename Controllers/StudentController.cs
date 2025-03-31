using ASM_SIMS.DB;
using ASM_SIMS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASM_SIMS.Controllers
{
    public class StudentController : Controller
    {
        private readonly SimsDataContext _dbContext; // DIP: Phụ thuộc vào abstraction (DbContext) thay vì instance cụ thể

        public StudentController(SimsDataContext dbContext)
        {
            _dbContext = dbContext; // Clean Code: Tên biến rõ ràng, phản ánh mục đích
        }

        // Hiển thị danh sách sinh viên
        public IActionResult Index()
        {
            var students = _dbContext.Students
                .Where(s => s.DeletedAt == null) // Clean Code: Lọc dữ liệu không bị xóa mềm
                .Select(s => new StudentViewModel
                {
                    Id = s.Id,
                    FullName = s.FullName,
                    Email = s.Email,
                    Phone = s.Phone,
                    Address = s.Address,
                    Status = s.Status,
                    CreatedAt = s.CreatedAt,
                    UpdatedAt = s.UpdatedAt
                }).ToList();

            ViewData["Title"] = "Students";
            return View(students);
        }


        [HttpGet]
        public IActionResult Create()
        {
            return View(new StudentViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(StudentViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var student = new Student
                    {
                        AccountId = 1, // Giả định tạm thời, sẽ thay bằng logic tạo Account
                        FullName = model.FullName,
                        Email = model.Email,
                        Phone = model.Phone,
                        Address = model.Address,
                        Status = "Active",
                        CreatedAt = DateTime.Now
                    };
                    _dbContext.Students.Add(student);
                    _dbContext.SaveChanges();
                    TempData["save"] = true;
                    return RedirectToAction(nameof(Index)); // Clean Code: Sử dụng nameof để tránh lỗi chính tả
                }
                catch (Exception ex)
                {
                    TempData["save"] = false;
                    ModelState.AddModelError("", $"Lỗi: {ex.Message}");
                }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var student = _dbContext.Students.Find(id);
            if (student == null || student.DeletedAt != null) return NotFound();

            var model = new StudentViewModel
            {
                Id = student.Id,
                FullName = student.FullName,
                Email = student.Email,
                Phone = student.Phone,
                Address = student.Address,
                Status = student.Status
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(StudentViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var student = _dbContext.Students.Find(model.Id);
                    if (student == null || student.DeletedAt != null) return NotFound();

                    student.FullName = model.FullName;
                    student.Email = model.Email;
                    student.Phone = model.Phone;
                    student.Address = model.Address;
                    student.Status = model.Status;
                    student.UpdatedAt = DateTime.Now;

                    _dbContext.Students.Update(student);
                    _dbContext.SaveChanges();
                    TempData["save"] = true;
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["save"] = false;
                    ModelState.AddModelError("", $"Lỗi: {ex.Message}");
                }
            }
            return View(model);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var student = _dbContext.Students.Find(id);
            if (student == null || student.DeletedAt != null) return NotFound();

            student.DeletedAt = DateTime.Now;
            student.Status = "Deleted";
            _dbContext.Students.Update(student);
            _dbContext.SaveChanges();
            return RedirectToAction(nameof(Index));
        }


    }
}
