using ASM_SIMS.DB;
using ASM_SIMS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASM_SIMS.Controllers
{
    public class ClassRoomController : Controller
    {
        private readonly SimsDataContext _dbContext;

        public ClassRoomController(SimsDataContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
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
                    Schedule = c.Schedule,
                    Location = c.Location,
                    Status = c.Status,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt
                }).ToList();

            ViewData["Title"] = "Class Rooms";
            return View(classRooms);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Courses = _dbContext.Courses.ToList();
            ViewBag.Teachers = _dbContext.Teachers.ToList();
            return View(new ClassRoomViewModel());
        }

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
                }
            }
            ViewBag.Courses = _dbContext.Courses.ToList();
            ViewBag.Teachers = _dbContext.Teachers.ToList();
            return View(model);
        }

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
                Schedule = classRoom.Schedule,
                Location = classRoom.Location,
                Status = classRoom.Status
            };
            ViewBag.Courses = _dbContext.Courses.ToList();
            ViewBag.Teachers = _dbContext.Teachers.ToList();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ClassRoomViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var classRoom = _dbContext.ClassRooms.Find(model.Id);
                    if (classRoom == null || classRoom.DeletedAt != null) return NotFound();

                    classRoom.ClassName = model.ClassName;
                    classRoom.CourseId = model.CourseId;
                    classRoom.TeacherId = model.TeacherId;
                    classRoom.Schedule = model.Schedule;
                    classRoom.Location = model.Location;
                    classRoom.Status = model.Status;
                    classRoom.UpdatedAt = DateTime.Now;

                    _dbContext.ClassRooms.Update(classRoom);
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
            ViewBag.Courses = _dbContext.Courses.ToList();
            ViewBag.Teachers = _dbContext.Teachers.ToList();
            return View(model);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var classRoom = _dbContext.ClassRooms.Find(id);
            if (classRoom == null || classRoom.DeletedAt != null) return NotFound();

            classRoom.DeletedAt = DateTime.Now;
            classRoom.Status = "Deleted";
            _dbContext.ClassRooms.Update(classRoom);
            _dbContext.SaveChanges();
            return RedirectToAction(nameof(Index));
        }


    }
}
