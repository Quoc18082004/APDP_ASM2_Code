using ASM_SIMS.DB;
using ASM_SIMS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ASM_SIMS.Controllers
{
    public class TeacherController : Controller
    {
        private readonly SimsDataContext _dbContext;

        // DIP: Tiêm SimsDataContext qua constructor để giảm phụ thuộc trực tiếp
        public TeacherController(SimsDataContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Hiển thị danh sách giảng viên
        public IActionResult Index()
        {

            // kiem tra session
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
            {
                return RedirectToAction("Index", "Login");
            }

            var teachers = _dbContext.Teachers
                .Where(t => t.DeletedAt == null)
                .Include(t => t.Account) // Include Account để lấy thông tin tài khoản nếu cần
                .Select(t => new TeacherViewModel
                {
                    Id = t.Id,
                    FullName = t.FullName,
                    Email = t.Email,
                    Phone = t.Phone,
                    Address = t.Address,
                    Status = t.Status,
                    CreatedAt = t.CreatedAt,
                    UpdatedAt = t.UpdatedAt
                }).ToList();

            ViewData["Title"] = "Teachers";
            return View(teachers);
        }

        // Hiển thị form thêm giảng viên
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Courses = new SelectList(_dbContext.Courses, "Id", "NameCourse"); // Load danh sách khóa học
            return View(new TeacherViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(TeacherViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var account = new Account
                    {
                        RoleId = 2,
                        Username = model.Email.Split('@')[0],
                        Password = "defaultPassword123",
                        Email = model.Email,
                        Phone = model.Phone,
                        Address = model.Address ?? "",
                        CreatedAt = DateTime.Now
                    };
                    _dbContext.Accounts.Add(account);
                    _dbContext.SaveChanges();

                    var teacher = new Teacher
                    {
                        AccountId = account.Id,
                        FullName = model.FullName,
                        Email = model.Email,
                        Phone = model.Phone,
                        Address = model.Address,
                        CourseId = model.CourseId, // Gán CourseId từ form
                        Status = model.Status,
                        CreatedAt = DateTime.Now
                    };
                    _dbContext.Teachers.Add(teacher);
                    _dbContext.SaveChanges();
                    TempData["save"] = true;
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["save"] = false;
                    ModelState.AddModelError("", $"Lỗi khi thêm giảng viên: {ex.Message} | Inner: {ex.InnerException?.Message}");
                }
            }
            ViewBag.Courses = new SelectList(_dbContext.Courses, "Id", "NameCourse"); // Load lại nếu lỗi
            return View(model);
        }

        // Hiển thị form sửa giảng viên
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var teacher = _dbContext.Teachers
                .Include(t => t.Account)
                .FirstOrDefault(t => t.Id == id && t.DeletedAt == null);

            if (teacher == null)
            {
                return NotFound();
            }

            var model = new TeacherViewModel
            {
                Id = teacher.Id,
                FullName = teacher.FullName,
                Email = teacher.Email,
                Phone = teacher.Phone,
                Address = teacher.Address,
                Status = teacher.Status
            };
            return View(model);
        }

        // Xử lý sửa giảng viên
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(TeacherViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var teacher = _dbContext.Teachers
                        .FirstOrDefault(t => t.Id == model.Id && t.DeletedAt == null);

                    if (teacher == null)
                    {
                        return NotFound();
                    }

                    teacher.FullName = model.FullName;
                    teacher.Email = model.Email;
                    teacher.Phone = model.Phone;
                    teacher.Address = model.Address;
                    teacher.Status = model.Status;
                    teacher.UpdatedAt = DateTime.Now;

                    _dbContext.Teachers.Update(teacher);
                    _dbContext.SaveChanges();
                    TempData["save"] = true;
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["save"] = false;
                    ModelState.AddModelError("", $"Lỗi khi sửa giảng viên: {ex.Message} | Inner: {ex.InnerException?.Message}");
                }
            }
            return View(model);
        }

        // Xử lý xóa giảng viên (xóa mềm)
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var teacher = _dbContext.Teachers
                .FirstOrDefault(t => t.Id == id && t.DeletedAt == null);

            if (teacher == null)
            {
                return NotFound();
            }

            try
            {
                teacher.DeletedAt = DateTime.Now;
                teacher.Status = "Deleted";
                _dbContext.Teachers.Remove(teacher);
                _dbContext.SaveChanges();
                TempData["save"] = true;
            }
            catch (Exception ex)
            {
                TempData["save"] = false;
                ModelState.AddModelError("", $"Lỗi khi xóa giảng viên: {ex.Message}");
            }
            return RedirectToAction(nameof(Index));
        }
    }
}