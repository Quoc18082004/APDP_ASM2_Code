using ASM_SIMS.Models;
using Microsoft.AspNetCore.Mvc;
using ASM_SIMS.Helpers;
using ASM_SIMS.DB;

namespace ASM_SIMS.Controllers
{
    public class CategoryController : Controller
    {
        private readonly SimsDataContext _dbcontext;
        public CategoryController(SimsDataContext context)
        {
            _dbcontext = context;
        }
        public IActionResult Index()
        {

            // Tao list de hien thi du lieu
            CategoryViewModel categoryModel = new CategoryViewModel();
            categoryModel.categoryList = new List<CategoryDetail>();
            var data = from m in _dbcontext.Categories
                       select m;
            data.ToList();
            foreach (var item in data)
            {
                categoryModel.categoryList.Add(new CategoryDetail
                {
                    Id = item.Id,
                    NameCategory = item.NameCategory,
                    Description = item.Description,
                    Avartar = item.Avatar,
                    Status = item.Status,
                    UpdatedAt = item.UpdatedAt,
                    CreatedAt = item.CreatedAt

                });
            }
            ViewData["title"] = "Category";
            return View(categoryModel);
        }

        [HttpGet]
        public IActionResult Create()
        {

            CategoryDetail model = new CategoryDetail();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryDetail model, IFormFile ViewAvatar)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    UploadFile uploadFile = new UploadFile(ViewAvatar);
                    string fileAvatar = uploadFile.Upload("images");
                    var dataCreate = new Categories()
                    {
                        NameCategory = model.NameCategory,
                        Description = model.Description,
                        Avatar = fileAvatar,
                        Status = "Active",
                        CreatedAt = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),

                    };
                    _dbcontext.Categories.Add(dataCreate);
                    _dbcontext.SaveChanges(true);
                    TempData["save"] = true;

                }
                catch(Exception ex)
                {
                    TempData["save"] = false;
                    return Ok(ex.Message.ToString());
                }
                return RedirectToAction("Index", "Category");
            }
            return View(model);

        }
    }

}
