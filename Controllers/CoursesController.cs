using ASM_SIMS.DB;
using ASM_SIMS.Helpers;
using ASM_SIMS.Models;
using Microsoft.AspNetCore.Mvc;

namespace ASM_SIMS.Controllers
{
    public class CoursesController : Controller
    {
        private readonly SimsDataContext _dbContext;

        public CoursesController(SimsDataContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            var courses = _dbContext.Courses
                .Where(c => c.DeletedAt == null)
                .Select(c => new CourseViewModel
                {
                    Id = c.Id,
                    NameCourse = c.NameCourse,
                    Description = c.Description,
                    CategoryId = c.CategoryId,
                    StartDate = c.StartDate,
                    EndDate = c.EndDate,
                    AvatarCourse = c.AvatarCourse,
                    Vote = c.Vote,
                    Status = c.Status,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt
                }).ToList();

            ViewData["Title"] = "Courses";
            return View(courses);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Categories = _dbContext.Categories.ToList();
            return View(new CourseViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CourseViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string fileAvatar = null;
                    if (model.AvatarCourseFile != null)
                    {
                        var uploadFile = new UploadFile(model.AvatarCourseFile); // SRP: UploadFile chỉ chịu trách nhiệm upload file
                        fileAvatar = uploadFile.Upload("images");
                    }

                    var course = new Courses
                    {
                        NameCourse = model.NameCourse,
                        Description = model.Description,
                        CategoryId = model.CategoryId,
                        StartDate = model.StartDate,
                        EndDate = model.EndDate,
                        AvatarCourse = fileAvatar,
                        Vote = model.Vote,
                        Status = true,
                        CreatedAt = DateTime.Now
                    };
                    _dbContext.Courses.Add(course);
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
            ViewBag.Categories = _dbContext.Categories.ToList();
            return View(model);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var course = _dbContext.Courses.Find(id);
            if (course == null || course.DeletedAt != null) return NotFound();

            var model = new CourseViewModel
            {
                Id = course.Id,
                NameCourse = course.NameCourse,
                Description = course.Description,
                CategoryId = course.CategoryId,
                StartDate = course.StartDate,
                EndDate = course.EndDate,
                AvatarCourse = course.AvatarCourse,
                Vote = course.Vote,
                Status = course.Status
            };
            ViewBag.Categories = _dbContext.Categories.ToList();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(CourseViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var course = _dbContext.Courses.Find(model.Id);
                    if (course == null || course.DeletedAt != null) return NotFound();

                    if (model.AvatarCourseFile != null)
                    {
                        var uploadFile = new UploadFile(model.AvatarCourseFile);
                        course.AvatarCourse = uploadFile.Upload("images");
                    }

                    course.NameCourse = model.NameCourse;
                    course.Description = model.Description;
                    course.CategoryId = model.CategoryId;
                    course.StartDate = model.StartDate;
                    course.EndDate = model.EndDate;
                    course.Vote = model.Vote;
                    course.Status = model.Status;
                    course.UpdatedAt = DateTime.Now;

                    _dbContext.Courses.Update(course);
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
            ViewBag.Categories = _dbContext.Categories.ToList();
            return View(model);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var course = _dbContext.Courses.Find(id);
            if (course == null || course.DeletedAt != null) return NotFound();

            course.DeletedAt = DateTime.Now;
            course.Status = false;
            _dbContext.Courses.Update(course);
            _dbContext.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}

/*
 SOLID:

SRP: Mỗi controller chỉ xử lý một loại thực thể (Student, Teacher, v.v.).
DIP: Tiêm SimsDataContext qua constructor để giảm phụ thuộc trực tiếp.
OCP (Open/Closed Principle): Có thể mở rộng bằng cách thêm action mới mà không sửa code cũ.

Clean Code:
Sử dụng nameof để tránh lỗi chính tả trong redirect.
Tên biến và phương thức rõ ràng, phản ánh mục đích.
Xử lý lỗi bằng try-catch để đảm bảo robust.
 */
