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

    // Action GET để hiển thị form đăng ký dành cho giảng viên
    [HttpGet]
    public IActionResult RegisterGV()
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
            return RedirectToAction("Login");
        }
        return View(registration);
    }

    // Action POST để xử lý đăng ký
    [HttpPost]
    public async Task<IActionResult> RegisterGV(GiangVienRegister registration)
    {
        if (ModelState.IsValid)
        {
            // Bỏ qua kiểm tra tên tài khoản đã tồn tại, thêm trực tiếp người dùng vào cơ sở dữ liệu
            _context.Add(registration);
            await _context.SaveChangesAsync();
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
        // Tạo tài khoản admin mặc định nếu chưa có
        var adminDefault = _context.DanhSachSinhVien
            .FirstOrDefault(u => u.TenTaiKhoan == "admin");

        if (adminDefault == null)
        {
            var admin = new DanhSachSinhVien
            {
                TenTaiKhoan = "admin",
                MatKhau = "123456", // Mật khẩu mặc định
                HoTen = "Quản trị viên",
                MSSV = "ADMIN001",
                VaiTro = "Admin"
            };

            _context.DanhSachSinhVien.Add(admin);
            _context.SaveChanges();
        }
        if (ModelState.IsValid)
        {
            // Kiểm tra trong bảng DanhSachSinhVien
            var user = _context.DanhSachSinhVien
                .FirstOrDefault(u => u.TenTaiKhoan == login.TenTaiKhoan && u.MatKhau == login.MatKhau);

            // Kiểm tra trong bảng GiangVien nếu không tìm thấy trong DanhSachSinhVien
            if (user == null)
            {
                var userGV = _context.GiangViens
                    .FirstOrDefault(u => u.TenTaiKhoan == login.TenTaiKhoan && u.MatKhau == login.MatKhau);

                if (userGV != null)
                {
                    // Lưu thông tin giảng viên vào session
                    HttpContext.Session.SetString("HoTen", userGV.TenGV);
                    HttpContext.Session.SetString("MaGV", userGV.MaGV);
                    HttpContext.Session.SetString("Role", "GiangVien");


                    // Chuyển hướng giảng viên đến trang tương ứng
                    return RedirectToAction("Index", "GiangVien");
                }
            }
            else
            {
                // Lưu thông tin sinh viên vào session
                HttpContext.Session.SetString("HoTen", user.HoTen);
                HttpContext.Session.SetString("MSV", user.MSSV);
                HttpContext.Session.SetString("Role", "SinhVien");


                // Kiểm tra quyền của người dùng và chuyển hướng
                if (user.VaiTro == "Admin")
                {
                    return RedirectToAction("DuyetSV", "Admin");
                }
                else if (user.VaiTro == "Sinh Viên")
                {
                    return RedirectToAction("TimKiem", "User");
                }
                else
                {
                    // Nếu không có vai trò, chuyển hướng mặc định
                    return RedirectToAction("Index");
                }
            }

            // Nếu không tìm thấy trong cả hai bảng
            ViewData["ErrorMessage"] = "Tên đăng nhập hoặc mật khẩu không đúng.";
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
    // Action POST để xử lý đổi mật khẩu
    [HttpPost]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = _context.DanhSachSinhVien
                .FirstOrDefault(u => u.TenTaiKhoan == model.TenTaiKhoan);

            var userGV = _context.GiangViens
                .FirstOrDefault(u => u.TenTaiKhoan == model.TenTaiKhoan);

            if (user != null && user.MatKhau == model.MatKhauCu)
            {
                if (model.MatKhauMoi == model.XacNhanMatKhau)
                {
                    user.MatKhau = model.MatKhauMoi;
                    _context.Update(user);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Mật khẩu đã được thay đổi thành công.";
                    return RedirectToAction("ChangePassword");
                }
                else
                {
                    ModelState.AddModelError("", "Mật khẩu mới và mật khẩu xác nhận không khớp.");
                }
            }
            else if (userGV != null && userGV.MatKhau == model.MatKhauCu)
            {
                if (model.MatKhauMoi == model.XacNhanMatKhau)
                {
                    userGV.MatKhau = model.MatKhauMoi;
                    _context.Update(userGV);
                    await _context.SaveChangesAsync();

                    // Lưu tên tài khoản vào session
                    HttpContext.Session.SetString("HoTen", userGV.TenGV);
                    HttpContext.Session.SetString("MGV", userGV.MaGV);

                    TempData["SuccessMessage"] = "Mật khẩu đã được thay đổi thành công.";
                    return RedirectToAction("Index", "GiangVien");
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
        HttpContext.Session.Clear();

        // Chuyển hướng về trang Home sau khi đăng xuất
        return RedirectToAction("Index", "Home");
    }
}
