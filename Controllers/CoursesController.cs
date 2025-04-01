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
                // Tạo list để hiển thị dữ liệu
                CourseViewModel courseModel = new CourseViewModel();
                courseModel.courseList = new List<CourseDetail>();
                var data = from c in _dbContext.Courses
                           where c.DeletedAt == null
                           select c;
                data.ToList();
                foreach (var item in data)
                {
                    courseModel.courseList.Add(new CourseDetail
                    {
                        Id = item.Id,
                        NameCourse = item.NameCourse,
                        Description = item.Description,
                        CategoryId = item.CategoryId,
                        StartDate = item.StartDate,
                        EndDate = item.EndDate,
                        AvatarCourse = item.AvatarCourse,
                        Vote = item.Vote,
                        Status = item.Status,
                        CreatedAt = item.CreatedAt,
                        UpdatedAt = item.UpdatedAt

                    });
                }
                ViewData["title"] = "Courses";
                return View(courseModel);
            }

            [HttpGet]
            public IActionResult Create()
            {
                ViewBag.Categories = _dbContext.Categories.ToList();
                return View(new CourseDetail());
            }

            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Create(CourseDetail model, IFormFile ViewAvatar)
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        string fileAvatar = null;
                        if (ViewAvatar != null)
                        {
                            UploadFile uploadFile = new UploadFile(ViewAvatar);
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
                            CreatedAt = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
                            DeletedAt = null
                        };
                        _dbContext.Courses.Add(course);
                        _dbContext.SaveChanges();
                        TempData["save"] = true;
                    }
                    catch (Exception ex)
                    {
                        TempData["save"] = false;
                        return Ok(ex.Message.ToString());
                    }
                    return RedirectToAction("Index", "Courses");
                }
                ViewBag.Categories = _dbContext.Categories.ToList();
                return View(model);
            }

            [HttpGet]
            public IActionResult Edit(int id)
            {
                var course = _dbContext.Courses.Find(id);
                if (course == null || course.DeletedAt != null) return NotFound();

                var model = new CourseDetail
                {
                    Id = course.Id,
                    NameCourse = course.NameCourse,
                    Description = course.Description,
                    CategoryId = course.CategoryId,
                    StartDate = course.StartDate,
                    EndDate = course.EndDate,
                    AvatarCourse = course.AvatarCourse,
                    Vote = course.Vote,
                    Status = course.Status,
                    CreatedAt = course.CreatedAt,
                    UpdatedAt = course.UpdatedAt

                };
                ViewBag.Categories = _dbContext.Categories.ToList();
                return View(model);
            }

            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Edit(CourseDetail model, IFormFile ViewAvatar)
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        var course = _dbContext.Courses.Find(model.Id);
                        if (course == null || course.DeletedAt != null) return NotFound();

                        if (ViewAvatar != null)
                        {
                            var uploadFile = new UploadFile(ViewAvatar);
                            course.AvatarCourse = uploadFile.Upload("images");
                        }

                        course.NameCourse = model.NameCourse;
                        course.Description = model.Description;
                        course.CategoryId = model.CategoryId;
                        course.StartDate = model.StartDate;
                        course.EndDate = model.EndDate;
                        course.Vote = model.Vote;
                        course.Status = model.Status;
                        course.UpdatedAt = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                        _dbContext.Courses.Update(course);
                        _dbContext.SaveChanges();
                        TempData["save"] = true;
                    }
                    catch (Exception ex)
                    {
                        TempData["save"] = false;
                        return Ok(ex.Message.ToString());
                    }
                    return RedirectToAction("Index", "Courses");
                }
                ViewBag.Categories = _dbContext.Categories.ToList();
                return View(model);
            }


            [HttpGet]
            public async Task<IActionResult> Delete(int id)
            {
                try
                {
                    var course = _dbContext.Courses.FirstOrDefault(c => c.Id == id);
                    if (course == null)
                    {
                        return NotFound();
                    }

                    _dbContext.Courses.Remove(course);
                    await _dbContext.SaveChangesAsync();
                    TempData["save"] = true;
                }
                catch (Exception ex)
                {
                    TempData["save"] = false;
                    TempData["error"] = $"Failed to delete course: {ex.Message}";
                }
                return RedirectToAction("Index");
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
