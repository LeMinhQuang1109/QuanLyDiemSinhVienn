using QuanLiDiem.Models;
using Microsoft.AspNetCore.Mvc;
using QuanLiDiem.Data;

namespace QuanLiDiem.Controllers
{
    public class SinhVienController : Controller
    {
        private readonly ApplicationDbContext _context;
        public SinhVienController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // Lấy danh sách sinh viên chưa bị duyệt
            var sinhviens = _context.DanhSachDK.ToList();
            return View(sinhviens);
        }

        // GET: SinhVien/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: SinhVien/Delete
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var sinhvien = _context.DanhSachDK.Find(id);
            if (sinhvien != null)
            {
                _context.DanhSachDK.Remove(sinhvien);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }

        // Action "Details" nhận tham số mssv
        public IActionResult Details(string mssv)
        {
            if (string.IsNullOrEmpty(mssv))
            {
                return NotFound(); // Nếu mssv không hợp lệ, trả về lỗi 404
            }

            // Tìm sinh viên theo mssv
            var sinhVien = _context.DanhSachSinhVien.FirstOrDefault(s => s.MSSV == mssv);
            if (sinhVien == null)
            {
                return NotFound(); // Nếu không tìm thấy sinh viên với mssv này
            }

            return View(sinhVien); // Trả về View Details với dữ liệu sinh viên
        }

        // Action để duyệt và chuyển hướng sang Details
        [HttpPost]
        public IActionResult ApproveAndDetails(int id)
        {
            var sinhVien = _context.DanhSachDK.Find(id);
            if (sinhVien == null)
            {
                return NotFound(); // Nếu không tìm thấy sinh viên, trả về lỗi 404
            }

            // Tạo MSSV với định dạng "4451050???"
            Random rand = new Random();
            string mssv = $"4451050{rand.Next(100, 1000):D3}";  // Tạo MSSV với 3 số ngẫu nhiên cuối

            // Xóa sinh viên khỏi danh sách đăng ký
            _context.DanhSachDK.Remove(sinhVien);
            _context.SaveChanges();

            // Tạo đối tượng sinh viên mới để hiển thị trong View Details
            var danhSachSinhVien = new DanhSachSinhVien
            {
                MSSV = mssv,  // Gán MSSV vừa tạo
                HoTen = sinhVien.HoTen,
                CanCuocCongDan = sinhVien.CCCD,
                SoDienThoai = sinhVien.SDT,
                DiaChi = sinhVien.DiaChi,
                MaNganh = sinhVien.NganhHoc,
                VaiTro = "SinhVien",
                TenTaiKhoan = mssv, // Gán tên tài khoản là MSSV
                MatKhau = Guid.NewGuid().ToString().Substring(0, 8) // Tạo mật khẩu ngẫu nhiên
            };

            // Lưu sinh viên mới vào bảng DanhSachSinhVien
            _context.DanhSachSinhVien.Add(danhSachSinhVien);
            _context.SaveChanges();

            // Trả về MSSV để dùng trong JavaScript
            return Json(new { mssv = mssv });
        }
    }
}
