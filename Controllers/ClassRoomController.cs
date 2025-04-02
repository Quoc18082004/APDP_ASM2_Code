using ASM_SIMS.DB;
using ASM_SIMS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASM_SIMS.Controllers
{
    public class ClassRoomController : Controller
    {
        private readonly SimsDataContext _dbContext;

        // DIP (Dependency Inversion Principle): Phụ thuộc vào abstraction (SimsDataContext)
        public ClassRoomController(SimsDataContext dbContext)
        {
            _dbContext = dbContext; // Clean Code: Tên biến rõ ràng
        }

        // Hiển thị danh sách lớp học
        public IActionResult Index()
        {

            // kiem tra session
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
            {
                return RedirectToAction("Index", "Login");
            }

            var classRooms = _dbContext.ClassRooms
                .Where(c => c.DeletedAt == null)
                .Include(c => c.Course)
                .Include(c => c.Teacher)
                .Select(c => new ClassRoomViewModel
                {
                    Id = c.Id,
                    ClassName = c.ClassName,
                    CourseId = c.CourseId,
                    TeacherId = c.TeacherId,
                    StartDate = c.StartDate,
                    EndDate = c.EndDate,
                    Schedule = c.Schedule, 
                    Location = c.Location,
                    Status = c.Status,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt
                }).ToList();

            // Gán ViewBag.Courses và ViewBag.Teachers
            ViewBag.Courses = _dbContext.Courses
                .Where(c => c.DeletedAt == null)
                .ToList();
            ViewBag.Teachers = _dbContext.Teachers
                .Where(t => t.DeletedAt == null)
                .ToList();

            ViewData["Title"] = "Class Rooms";
            return View(classRooms);
        }
        // GET: Hiển thị form tạo mới lớp học
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Courses = _dbContext.Courses
                .Where(c => c.DeletedAt == null)
                .ToList();
            ViewBag.Teachers = _dbContext.Teachers
                .Where(t => t.DeletedAt == null)
                .ToList();
            return View(new ClassRoomViewModel());
        }

        // POST: Xử lý thêm mới lớp học
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ClassRoomViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var classRoom = new ClassRoom
                    {
                        ClassName = model.ClassName,
                        CourseId = model.CourseId,
                        TeacherId = model.TeacherId,
                        StartDate = model.StartDate,
                        EndDate = model.EndDate,
                        Schedule = model.Schedule, 
                        Location = model.Location,
                        Status = "Active",
                        CreatedAt = DateTime.Now
                    };
                    _dbContext.ClassRooms.Add(classRoom);
                    _dbContext.SaveChanges();
                    TempData["save"] = true;
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["save"] = false;
                    ModelState.AddModelError("", $"Lỗi: {ex.Message}");
                    System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}"); 
                }
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                ModelState.AddModelError("", "Validation errors: " + string.Join(", ", errors));
                System.Diagnostics.Debug.WriteLine("Validation errors: " + string.Join(", ", errors)); 
            }
            ViewBag.Courses = _dbContext.Courses
                .Where(c => c.DeletedAt == null)
                .ToList();
            ViewBag.Teachers = _dbContext.Teachers
                .Where(t => t.DeletedAt == null)
                .ToList();
            return View(model);
        }

        // GET: Hiển thị form chỉnh sửa lớp học
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var classRoom = _dbContext.ClassRooms.Find(id);
            if (classRoom == null || classRoom.DeletedAt != null) return NotFound();

            var model = new ClassRoomViewModel
            {
                Id = classRoom.Id,
                ClassName = classRoom.ClassName,
                CourseId = classRoom.CourseId,
                TeacherId = classRoom.TeacherId,
                StartDate = classRoom.StartDate, 
                EndDate = classRoom.EndDate,     
                Location = classRoom.Location,
                Status = classRoom.Status
            };
            ViewBag.Courses = _dbContext.Courses.ToList();
            ViewBag.Teachers = _dbContext.Teachers.ToList();
            return View(model);
        }

        // POST: Xử lý chỉnh sửa lớp học
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ClassRoomViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var classRoom = _dbContext.ClassRooms.FirstOrDefault(c => c.Id == model.Id);
                    if (classRoom == null)
                    {
                        TempData["save"] = false;
                        return RedirectToAction(nameof(Index));
                    }

                    // Cập nhật các trường
                    classRoom.ClassName = model.ClassName;
                    classRoom.CourseId = model.CourseId;
                    classRoom.TeacherId = model.TeacherId;
                    classRoom.StartDate = model.StartDate;
                    classRoom.EndDate = model.EndDate;
                    classRoom.Schedule = model.Schedule;
                    classRoom.Location = model.Location;
                    classRoom.Status = model.Status;
                    classRoom.UpdatedAt = DateTime.Now;

                    _dbContext.SaveChanges();
                    TempData["save"] = true;
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["save"] = false;
                    ModelState.AddModelError("", $"Lỗi: {ex.Message}");
                    System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
                }
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                ModelState.AddModelError("", "Validation errors: " + string.Join(", ", errors));
                System.Diagnostics.Debug.WriteLine("Validation errors: " + string.Join(", ", errors));
            }

            ViewBag.Courses = _dbContext.Courses
                .Where(c => c.DeletedAt == null)
                .ToList();
            ViewBag.Teachers = _dbContext.Teachers
                .Where(t => t.DeletedAt == null)
                .ToList();
            return View(model);
        }
        // POST: Xử lý xóa lớp học
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            try
            {
                var classRoom = _dbContext.ClassRooms.FirstOrDefault(c => c.Id == id);
                if (classRoom == null)
                {
                    TempData["save"] = false;
                    return RedirectToAction(nameof(Index));
                }

                _dbContext.ClassRooms.Remove(classRoom); 
                _dbContext.SaveChanges();
                TempData["save"] = true;
            }
            catch (Exception ex)
            {
                TempData["save"] = false;
                System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
            }
            return RedirectToAction(nameof(Index));
        }
    }
    
}