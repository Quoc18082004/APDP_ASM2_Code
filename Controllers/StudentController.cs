using ASM_SIMS.DB;
using ASM_SIMS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Linq;

namespace ASM_SIMS.Controllers
{
    public class StudentController : Controller
    {
        private readonly SimsDataContext _dbContext;

        public StudentController(SimsDataContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Action hiển thị danh sách sinh viên
        public IActionResult Index()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
            {
                return RedirectToAction("Index", "Login");
            }


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
                    Avatar = s.Avatar 
                }).ToList();

            ViewBag.ClassRooms = _dbContext.ClassRooms.ToList();
            ViewBag.Courses = _dbContext.Courses.ToList();
            ViewData["Title"] = "Students";
            return View(students);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.ClassRooms = _dbContext.ClassRooms.ToList();
            ViewBag.Courses = _dbContext.Courses.ToList();
            return View(new StudentViewModel());
        }

        // Xử lý việc tạo mới sinh viên
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(StudentViewModel model)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
                return RedirectToAction("Index", "Login");

            // Kiểm tra ảnh bắt buộc
            if (model.ViewAvatar == null || model.ViewAvatar.Length == 0)
            {
                ModelState.AddModelError("ViewAvatar", "Please choose an avatar.");
            }

            // Kiểm tra trùng email & phone
            bool emailExists = _dbContext.Students.Any(s =>
                s.Email == model.Email &&
                s.CourseId == model.CourseId &&
                s.DeletedAt == null); // Không cần check Id vì đang tạo mới

            if (emailExists)
            {
                ModelState.AddModelError("Email", "Email already exists!");
            }

            bool phoneExists = _dbContext.Students.Any(s =>
                s.Phone == model.Phone &&
                s.CourseId == model.CourseId &&
                s.DeletedAt == null);

            if (phoneExists)
            {
                ModelState.AddModelError("Phone", "Phone number already exists.");
            }

            // Nếu có lỗi, load lại View
            if (!ModelState.IsValid)
            {
                ViewBag.ClassRooms = _dbContext.ClassRooms.ToList();
                ViewBag.Courses = _dbContext.Courses.ToList();
                return View(model);
            }

            // Tạo tài khoản mới
            var account = new Account
            {
                RoleId = 1,
                Username = model.Email.Split('@')[0],
                Password = "123456",
                Email = model.Email,
                Phone = model.Phone,
                Address = model.Address,
                CreatedAt = DateTime.Now
            };
            _dbContext.Accounts.Add(account);
            _dbContext.SaveChanges();

            // Xử lý lưu ảnh
            string avatarFileName = Guid.NewGuid().ToString() + Path.GetExtension(model.ViewAvatar.FileName);
            var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "avatars");
            Directory.CreateDirectory(uploadFolder);

            var filePath = Path.Combine(uploadFolder, avatarFileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                model.ViewAvatar.CopyTo(stream);
            }

            // Tạo mới sinh viên
            var student = new Student
            {
                AccountId = account.Id,
                FullName = model.FullName,
                Email = model.Email,
                Phone = model.Phone,
                Address = model.Address,
                ClassRoomId = model.ClassRoomId,
                CourseId = model.CourseId,
                Status = "Active",
                CreatedAt = DateTime.Now,
                Avatar = $"/uploads/avatars/{avatarFileName}"
            };
            _dbContext.Students.Add(student);
            _dbContext.SaveChanges();

            TempData["save"] = true;
            return RedirectToAction(nameof(Index));
        }

        // Hiển thị form chỉnh sửa thông tin sinh viên
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
                Avatar = student.Avatar,
                Status = student.Status
            };

            ViewBag.ClassRooms = _dbContext.ClassRooms.ToList();
            ViewBag.Courses = _dbContext.Courses.ToList();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(StudentViewModel model, IFormFile? ViewAvatar)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra trùng email & phone
                bool emailExists = _dbContext.Students.Any(s =>
                    s.Email == model.Email &&
                    s.CourseId == model.CourseId &&
                    s.Id != model.Id &&
                    s.DeletedAt == null);

                if (emailExists)
                {
                    ModelState.AddModelError("Email", "Email already exists!");
                }

                bool phoneExists = _dbContext.Students.Any(s =>
                    s.Phone == model.Phone &&
                    s.CourseId == model.CourseId &&
                    s.Id != model.Id &&
                    s.DeletedAt == null);

                if (phoneExists)
                {
                    ModelState.AddModelError("Phone", "Phone number already exists.");
                }

                if (!ModelState.IsValid)
                {
                    ViewBag.ClassRooms = _dbContext.ClassRooms.ToList();
                    ViewBag.Courses = _dbContext.Courses.ToList();
                    return View(model);
                }

                try
                {
                    var student = _dbContext.Students.FirstOrDefault(s => s.Id == model.Id && s.DeletedAt == null);
                    if (student == null)
                    {
                        return NotFound();
                    }

                    // Cập nhật thông tin cơ bản
                    student.FullName = model.FullName;
                    student.Email = model.Email;
                    student.Phone = model.Phone;
                    student.Address = model.Address;
                    student.ClassRoomId = model.ClassRoomId;
                    student.CourseId = model.CourseId;
                    student.Status = model.Status;
                    student.UpdatedAt = DateTime.Now;

                    // Nếu có upload ảnh mới thì xử lý
                    if (ViewAvatar != null)
                    {
                        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "avatars");
                        Directory.CreateDirectory(uploadsFolder);

                        var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(ViewAvatar.FileName)}";
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            ViewAvatar.CopyTo(stream);
                        }

                        student.Avatar = $"/uploads/avatars/{uniqueFileName}";
                    }

                    _dbContext.Students.Update(student);
                    _dbContext.SaveChanges();

                    TempData["save"] = true;
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["save"] = false;
                    ModelState.AddModelError("", $"Error: {ex.Message}");
                }
            }

            ViewBag.ClassRooms = _dbContext.ClassRooms.ToList();
            ViewBag.Courses = _dbContext.Courses.ToList();
            return View(model);
        }

        // Xử lý xóa sinh viên
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var student = _dbContext.Students
                .Include(s => s.Account)
                .FirstOrDefault(s => s.Id == id && s.DeletedAt == null);

            if (student == null)
            {
                return NotFound();
            }

            try
            {
                if (student.Account != null)
                {
                    _dbContext.Accounts.Remove(student.Account);
                }

                _dbContext.Students.Remove(student);
                _dbContext.SaveChanges();

                TempData["save"] = true;
            }
            catch (Exception ex)
            {
                TempData["save"] = false;
                ModelState.AddModelError("", $"Error deleting student: {ex.Message} | Inner: {ex.InnerException?.Message}");
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
