using ASM_SIMS.Models;
using Microsoft.AspNetCore.Mvc;

namespace ASM_SIMS.Controllers
{
    public class CategoryController : Controller
    {
        public IActionResult Index()
        {
            
            return View();
        }

        [HttpGet]
        public IActionResult Create()
        {
           
            CategoryDetail model = new CategoryDetail();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryDetail model, IFormFile Avatar)
        {

            if (ModelState.IsValid)
            {
                return Ok(Avatar);
            }
            return View(model);
        }
        
    }
}
