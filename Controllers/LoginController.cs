using ASM_SIMS.DB;
using ASM_SIMS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASM_SIMS.Controllers
{
    public class LoginController : Controller
    {
        private readonly SimsDataContext _dbContext;

        // DIP: Tiêm SimsDataContext qua constructor
        public LoginController(SimsDataContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GET: Login
        [HttpGet]
        public IActionResult Index()
        {
            return View(new LoginViewModel());
        }

        // POST: Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra thông tin đăng nhập từ database
                var account = await _dbContext.Accounts
                    .FirstOrDefaultAsync(a => a.Email == model.Email && a.Password == model.Password && a.DeletedAt == null);

                if (account != null)
                {
                    // Lưu thông tin vào session
                    HttpContext.Session.SetString("UserId", account.Id.ToString());
                    HttpContext.Session.SetString("Username", account.Username);
                    return RedirectToAction("Index", "Dashboard");
                }
                else
                {
                    ViewData["MessageLogin"] = "Invalid email or password";
                }
            }
            return View(model);
        }

        // GET: Register
        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        // POST: Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra email hoặc username đã tồn tại chưa
                if (await _dbContext.Accounts.AnyAsync(a => a.Email == model.Email || a.Username == model.Username))
                {
                    ModelState.AddModelError("", "Email or Username already exists");
                    return View(model);
                }

                var account = new Account
                {
                    RoleId = 1, // Giả định RoleId = 1 là user thường, có thể thay đổi sau
                    Username = model.Username,
                    Password = model.Password, // Lưu ý: Nên mã hóa mật khẩu trong thực tế
                    Email = model.Email,
                    Phone = model.Phone,
                    Address = model.Address,
                    CreatedAt = DateTime.Now
                };

                _dbContext.Accounts.Add(account);
                await _dbContext.SaveChangesAsync();

                TempData["MessageRegister"] = "Registration successful! Please sign in.";
                return RedirectToAction("Index");
            }
            return View(model);
        }

        // POST: Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); // Xóa session
            return RedirectToAction("Index", "Login");
        }
    }
}
