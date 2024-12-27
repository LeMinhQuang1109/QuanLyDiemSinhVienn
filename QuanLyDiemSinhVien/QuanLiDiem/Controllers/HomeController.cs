using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using QuanLiDiem.Models;
using QuanLiDiem.Data;
using System.Linq;
using System.Threading.Tasks;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    // Action GET để hiển thị form đăng ký
    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    // Action POST để xử lý đăng ký
    [HttpPost]
    public async Task<IActionResult> Register(RegistrationModel registration)
    {
        if (ModelState.IsValid)
        {
            // Bỏ qua kiểm tra tên tài khoản đã tồn tại, thêm trực tiếp người dùng vào cơ sở dữ liệu
            _context.Add(registration);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đăng ký thành công, vui lòng đăng nhập.";
            return RedirectToAction("Login");
        }
        return View(registration);
    }

    // Action GET để hiển thị form đăng nhập
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Login(LoginViewModel login)
    {
        if (ModelState.IsValid)
        {
            var user = _context.DanhSachSinhVien
                .FirstOrDefault(u => u.TenTaiKhoan == login.TenTaiKhoan && u.MatKhau == login.MatKhau);

            if (user != null)
            {
                // Lưu tên tài khoản vào session
                HttpContext.Session.SetString("HoTen", user.HoTen);
                HttpContext.Session.SetString("MSV", user.MSSV);

                // Kiểm tra quyền của người dùng và chuyển hướng đến trang tương ứng
                if (user.VaiTro == "Admin")
                {
                    return RedirectToAction("DuyetSV", "Admin");
                }
                else if (user.VaiTro == "User")
                {
                    return RedirectToAction("Index", "User");
                }
                else if (user.VaiTro == "GiangVien")
                {
                    return RedirectToAction("TimKiem", "GiangVien");
                }
                else
                {
                    // Nếu không có vai trò, có thể chuyển hướng về trang mặc định
                    return RedirectToAction("Index");
                }
            }
            else
            {
                // Thêm thông báo lỗi vào ViewData để hiển thị trong modal
                ViewData["ErrorMessage"] = "Tên đăng nhập hoặc mật khẩu không đúng.";
            }
        }
        return View(login);
    }


    // Action GET để hiển thị trang Index
    public IActionResult Index()
    {
        return View();
    }

    // Action GET để hiển thị form đổi mật khẩu
    [HttpGet]
    public IActionResult ChangePassword()
    {
        return View();
    }

    // Action POST để xử lý đổi mật khẩu
    [HttpPost]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = _context.DanhSachSinhVien
                .FirstOrDefault(u => u.TenTaiKhoan == model.TenTaiKhoan);

            if (user != null && user.MatKhau == model.MatKhauCu)
            {
                if (model.MatKhauMoi == model.XacNhanMatKhau)
                {
                    user.MatKhau = model.MatKhauMoi;
                    _context.Update(user);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Mật khẩu đã được thay đổi thành công.";
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Mật khẩu mới và mật khẩu xác nhận không khớp.");
                }
            }
            else
            {
                ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu cũ không đúng.");
            }
        }
        return View(model);
    }

    // Action GET để xử lý đăng xuất
    [HttpGet]
    public IActionResult Logout()
    {
        // Xóa session Username
        HttpContext.Session.Remove("HoTen");

        // Chuyển hướng về trang Home sau khi đăng xuất
        return RedirectToAction("Index", "Home");
    }
}
