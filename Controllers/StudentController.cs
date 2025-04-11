
using ASM_SIMS.DB; 
using ASM_SIMS.Models; 
using Microsoft.AspNetCore.Mvc; 
using Microsoft.EntityFrameworkCore; 

namespace ASM_SIMS.Controllers
{
    
    public class StudentController : Controller
    {
        private readonly SimsDataContext _dbContext;

        // Constructor với Dependency Injection để lấy DbContext
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

            // Truy vấn danh sách sinh viên chưa bị xóa, bao gồm lớp và khóa học
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

            // Truyền dữ liệu lớp và khóa học cho View
            ViewBag.ClassRooms = _dbContext.ClassRooms.ToList();
            ViewBag.Courses = _dbContext.Courses.ToList();
            ViewData["Title"] = "Students";
            return View(students);
        }

        // Action hiển thị form tạo mới sinh viên
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.ClassRooms = _dbContext.ClassRooms.ToList();
            ViewBag.Courses = _dbContext.Courses.ToList();
            return View(new StudentViewModel()); // Truyền model trống
        }

        // Xử lý việc tạo mới sinh viên
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(StudentViewModel model)
        {
            // Kiểm tra nếu chưa đăng nhập thì không cho vào
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
            {
                return RedirectToAction("Index", "Login");
            }

            if (ModelState.IsValid)
            {


                bool emailExists = _dbContext.Students.Any(s =>
                s.Email == model.Email &&
                s.CourseId == model.CourseId &&
                s.DeletedAt == null);

                if (emailExists)
                {
                    ModelState.AddModelError("Email", "Email already exists.");
                }

                bool phoneExists = _dbContext.Students.Any(s =>
                    s.Phone == model.Phone &&
                    s.CourseId == model.CourseId &&
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


                // 4. Tạo tài khoản cho sinh viên
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

                // 5. Thêm sinh viên vào hệ thống
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
                    CreatedAt = DateTime.Now
                };
                _dbContext.Students.Add(student);
                _dbContext.SaveChanges();

                TempData["save"] = true;
                return RedirectToAction(nameof(Index));
            }

            // Nếu ModelState không hợp lệ
            ViewBag.ClassRooms = _dbContext.ClassRooms.ToList();
            ViewBag.Courses = _dbContext.Courses.ToList();
            return View(model);
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

            // Map dữ liệu từ Student sang StudentViewModel
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

        // Xử lý việc chỉnh sửa sinh viên
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(StudentViewModel model)
        {
            if (ModelState.IsValid)
            {

                bool emailExists = _dbContext.Students.Any(s =>
                s.Email == model.Email &&
                s.CourseId == model.CourseId &&
                s.DeletedAt == null);

                if (emailExists)
                {
                    ModelState.AddModelError("Email", "Email already exists.");
                }

                bool phoneExists = _dbContext.Students.Any(s =>
                    s.Phone == model.Phone &&
                    s.CourseId == model.CourseId &&
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

                // Nếu có lỗi, return lại view
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

                    // Cập nhật thông tin
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
                    ModelState.AddModelError("", $"Error while updating student: {ex.Message} | Inner: {ex.InnerException?.Message}");
                }

            }

            ViewBag.ClassRooms = _dbContext.ClassRooms.ToList();
            ViewBag.Courses = _dbContext.Courses.ToList();
            return View(model);
        }

        // Xử lý xóa sinh viên (xóa mềm nếu cần hoặc xóa luôn)
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var student = _dbContext.Students
                .Include(s => s.Account)
                .FirstOrDefault(s => s.Id == id && s.DeletedAt == null);

            if (student == null)
            {
                return NotFound(); // Không tìm thấy sinh viên
            }

            try
            {
                // Nếu sinh viên có tài khoản thì xóa luôn tài khoản
                if (student.Account != null)
                {
                    _dbContext.Accounts.Remove(student.Account);
                }

                // Xóa sinh viên
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
