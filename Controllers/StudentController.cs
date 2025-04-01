using ASM_SIMS.DB;
using ASM_SIMS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASM_SIMS.Controllers
{
    public class StudentController : Controller
    {
        private readonly SimsDataContext _dbContext;

        public StudentController(SimsDataContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Hiển thị danh sách sinh viên
        public IActionResult Index()
        {
            var students = _dbContext.Students
                .Where(s => s.DeletedAt == null)
                .Include(s => s.ClassRoom)
                .Include(s => s.Course)
                .Select(s => new StudentViewModel
                {
                    Id = s.Id,
                    FullName = s.FullName,
                    Email = s.Email,
                    Phone = s.Phone,
                    Address = s.Address,
                    ClassRoomId = s.ClassRoomId,
                    CourseId = s.CourseId,
                    Status = s.Status,
                    CreatedAt = s.CreatedAt,
                    UpdatedAt = s.UpdatedAt
                }).ToList();

            ViewBag.ClassRooms = _dbContext.ClassRooms.ToList();
            ViewBag.Courses = _dbContext.Courses.ToList();
            ViewData["Title"] = "Students";
            return View(students);
        }

        // Hiển thị form thêm sinh viên
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.ClassRooms = _dbContext.ClassRooms.ToList();
            ViewBag.Courses = _dbContext.Courses.ToList();
            return View(new StudentViewModel());
        }

        // Xử lý thêm sinh viên
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(StudentViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Tạo Account mới cho sinh viên
                    var account = new Account
                    {
                        RoleId = 1, // Giả định RoleId = 1 (Student), thay đổi nếu cần
                        Username = model.Email.Split('@')[0], // Tạo username từ email
                        Password = "defaultPassword123", // Mật khẩu mặc định, nên mã hóa trong thực tế
                        Email = model.Email,
                        Phone = model.Phone,
                        Address = model.Address ?? "", // Gán Address hoặc chuỗi rỗng nếu null
                        CreatedAt = DateTime.Now
                    };
                    _dbContext.Accounts.Add(account);
                    _dbContext.SaveChanges(); // Lưu Account để lấy Id

                    // Tạo Student với AccountId vừa tạo
                    var student = new Student
                    {
                        AccountId = account.Id, // Sử dụng Id của Account mới
                        FullName = model.FullName,
                        Email = model.Email,
                        Phone = model.Phone,
                        Address = model.Address,
                        ClassRoomId = model.ClassRoomId,
                        CourseId = model.CourseId,
                        Status = model.Status,
                        CreatedAt = DateTime.Now
                    };
                    _dbContext.Students.Add(student);
                    _dbContext.SaveChanges();
                    TempData["save"] = true;
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["save"] = false;
                    ModelState.AddModelError("", $"Lỗi khi thêm sinh viên: {ex.Message} | Inner: {ex.InnerException?.Message}");
                }
            }
            ViewBag.ClassRooms = _dbContext.ClassRooms.ToList();
            ViewBag.Courses = _dbContext.Courses.ToList();
            return View(model);
        }

        // Hiển thị form sửa sinh viên
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var student = _dbContext.Students
                .Include(s => s.ClassRoom)
                .Include(s => s.Course)
                .FirstOrDefault(s => s.Id == id && s.DeletedAt == null);

            if (student == null)
            {
                return NotFound();
            }

            var model = new StudentViewModel
            {
                Id = student.Id,
                FullName = student.FullName,
                Email = student.Email,
                Phone = student.Phone,
                Address = student.Address,
                ClassRoomId = student.ClassRoomId,
                CourseId = student.CourseId,
                Status = student.Status
            };
            ViewBag.ClassRooms = _dbContext.ClassRooms.ToList();
            ViewBag.Courses = _dbContext.Courses.ToList();
            return View(model);
        }

        // Xử lý sửa sinh viên
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(StudentViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var student = _dbContext.Students
                        .FirstOrDefault(s => s.Id == model.Id && s.DeletedAt == null);

                    if (student == null)
                    {
                        return NotFound();
                    }

                    student.FullName = model.FullName;
                    student.Email = model.Email;
                    student.Phone = model.Phone;
                    student.Address = model.Address;
                    student.ClassRoomId = model.ClassRoomId;
                    student.CourseId = model.CourseId;
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
                    ModelState.AddModelError("", $"Lỗi khi sửa sinh viên: {ex.Message} | Inner: {ex.InnerException?.Message}");
                }
            }
            ViewBag.ClassRooms = _dbContext.ClassRooms.ToList();
            ViewBag.Courses = _dbContext.Courses.ToList();
            return View(model);
        }

        // Xử lý xóa sinh viên (xóa mềm)
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var student = _dbContext.Students
                .FirstOrDefault(s => s.Id == id && s.DeletedAt == null);

            if (student == null)
            {
                return NotFound();
            }

            try
            {
                student.DeletedAt = DateTime.Now;
                student.Status = "Deleted";
                _dbContext.Students.Remove(student);
                _dbContext.SaveChanges();
                TempData["save"] = true;
            }
            catch (Exception ex)
            {
                TempData["save"] = false;
                ModelState.AddModelError("", $"Lỗi khi xóa sinh viên: {ex.Message}");
            }
            return RedirectToAction(nameof(Index));
        }
    }
}