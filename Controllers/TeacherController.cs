using Microsoft.AspNetCore.Mvc;
using ASM_SIMS.Models;
using ASM_SIMS.DB;
using Microsoft.EntityFrameworkCore;

namespace ASM_SIMS.Controllers
{
    public class TeacherController : Controller
    {
        private readonly SimsDataContext _dbContext;

        public TeacherController(SimsDataContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IActionResult Index()
        {
            var teachers = _dbContext.Teachers
                 .Where(t => t.DeletedAt == null)
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


        [HttpGet]
        public IActionResult Create()
        {
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
                    var teacher = new Teacher
                    {
                        AccountId = 1, // Giả định tạm thời
                        FullName = model.FullName,
                        Email = model.Email,
                        Phone = model.Phone,
                        Address = model.Address,
                        Status = "Active",
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
                    ModelState.AddModelError("", $"Lỗi: {ex.Message}");
                }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var teacher = _dbContext.Teachers.Find(id);
            if (teacher == null || teacher.DeletedAt != null) return NotFound();

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(TeacherViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var teacher = _dbContext.Teachers.Find(model.Id);
                    if (teacher == null || teacher.DeletedAt != null) return NotFound();

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
                    ModelState.AddModelError("", $"Lỗi: {ex.Message}");
                }
            }
            return View(model);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var teacher = _dbContext.Teachers.Find(id);
            if (teacher == null || teacher.DeletedAt != null) return NotFound();

            teacher.DeletedAt = DateTime.Now;
            teacher.Status = "Deleted";
            _dbContext.Teachers.Update(teacher);
            _dbContext.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
