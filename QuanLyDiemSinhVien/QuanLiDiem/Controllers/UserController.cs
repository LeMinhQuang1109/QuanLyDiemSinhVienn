using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLiDiem.Data;
using QuanLiDiem.Models;

namespace QuanLiDiem.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index2(string searchTerm)
        {
            // Lấy danh sách sinh viên từ cơ sở dữ liệu
            var sinhViens = _context.DanhSachSinhVien.AsQueryable();

            // Nếu có từ khóa tìm kiếm, lọc danh sách
            if (!string.IsNullOrEmpty(searchTerm))
            {
                sinhViens = sinhViens.Where(sv =>
                    sv.MSSV.Contains(searchTerm) ||
                    sv.HoTen.Contains(searchTerm));
            }

            // Truyền từ khóa tìm kiếm vào ViewData để giữ lại trong ô tìm kiếm
            ViewData["SearchTerm"] = searchTerm;

            // Trả danh sách sinh viên về view
            return View(sinhViens.ToList());
        }

        // GET: Diems
        public IActionResult Index()
        {
            // Lấy MSSV từ session
            var maSV = HttpContext.Session.GetString("MSV");

            if (maSV == null)
            {
                // Nếu MSSV không tồn tại trong session, điều hướng về trang đăng nhập
                return RedirectToAction("Login", "Home");
            }

            // Lấy danh sách điểm của sinh viên dựa trên MSSV
            var danhSachDiem = _context.Diem
                .Where(d => d.MSSV == maSV.ToString()) // MSSV trong cơ sở dữ liệu là kiểu string
                .ToList();

            // Truyền danh sách điểm vào view
            return View(danhSachDiem);
        }




        // GET: CapNhatTT/Details/{MSSV}
        public async Task<IActionResult> ThongTin(string MSSV)
        {
            // Lấy MSSV từ session
            var maSV = HttpContext.Session.GetString("MSV");

            if (maSV == null)
            {
                return RedirectToAction("Login", "Home");
            }

            // Lấy thông tin sinh viên dựa trên MSSV
            var thongTinSV = await _context.DanhSachSinhVien
                .FirstOrDefaultAsync(d => d.MSSV == maSV); // Tìm một sinh viên duy nhất

            if (thongTinSV == null)
            {
                // Nếu không tìm thấy sinh viên, trả về thông báo lỗi hoặc trang phù hợp
                return NotFound("Không tìm thấy thông tin sinh viên.");
            }

            // Truyền thông tin sinh viên vào view
            return View(thongTinSV);
        }



        // GET: DanhSachSinhVien/Edit
        public async Task<IActionResult> CapNhat(string? MSSV)
        {
            // Lấy MSSV từ session
            var maSV = HttpContext.Session.GetString("MSV");
            Console.WriteLine($"MSSV từ session: {maSV}");

            if (maSV == null)
            {
                return RedirectToAction("Login", "Home");
            }

            // Lấy thông tin sinh viên dựa trên MSSV
            var thongTinSV = await _context.DanhSachSinhVien
                .FirstOrDefaultAsync(d => d.MSSV == maSV); // Tìm một sinh viên duy nhất

            if (thongTinSV == null)
            {
                // Nếu không tìm thấy sinh viên, trả về thông báo lỗi hoặc trang phù hợp
                return NotFound("Không tìm thấy thông tin sinh viên.");
            }

            // Truyền thông tin sinh viên vào view
            return View(thongTinSV);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CapNhat(string mssv, [Bind("MSSV,HoTen,GioiTinh,CanCuocCongDan,SoDienThoai,Email,DiaChi,MaNganh,TenTaiKhoan,MatKhau,VaiTro")] DanhSachSinhVien thongTinSV)
        {
            var maSV = HttpContext.Session.GetString("MSV");
            Console.WriteLine($"MSSV từ session: {maSV}");

            if (maSV == null)
            {
                return RedirectToAction("Login", "Home");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(thongTinSV);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ThongTinSVExists(thongTinSV.MSSV))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(thongTinSV);
        }
        private bool ThongTinSVExists(string mssv)
        {
            return _context.DanhSachSinhVien.Any(e => e.MSSV == mssv);
        }


        public IActionResult TimKiem(string searchTerm, string maNganh, string thongKe)
        {
            // Lấy danh sách sinh viên từ cơ sở dữ liệu
            var sinhViens = _context.DanhSachSinhVien.AsQueryable();

            // Nếu có từ khóa tìm kiếm, lọc danh sách
            if (!string.IsNullOrEmpty(searchTerm))
            {
                sinhViens = sinhViens.Where(sv =>
                    sv.MSSV.Contains(searchTerm) ||
                    sv.HoTen.Contains(searchTerm));
            }

            // Nếu có giá trị tìm kiếm Mã ngành, lọc theo mã ngành
            if (!string.IsNullOrEmpty(maNganh))
            {
                sinhViens = sinhViens.Where(s => s.MaNganh == maNganh);
            }

            // Lấy danh sách kết quả
            var model = sinhViens.ToList();

            // Tính toán số lượng sinh viên trong mã ngành được chọn (nếu có mã ngành)
            var totalByMaNganh = string.IsNullOrEmpty(maNganh) ? 0 : model.Count(s => s.MaNganh == maNganh);

            // Dùng dictionary để ánh xạ mã ngành sang tên ngành
            var maNganhToTenNganh = new Dictionary<string, string>
            {
                { "1", "Công nghệ thông tin" },
                { "2", "Kinh Tế" },
                { "3", "Cơ Khí" },
                // Thêm mã ngành và tên ngành khác nếu cần
            };

            // Kiểm tra nếu mã ngành tồn tại trong dictionary
            string tenNganh = "Không xác định"; // Giá trị mặc định nếu mã ngành không hợp lệ
            if (!string.IsNullOrEmpty(maNganh) && maNganhToTenNganh.ContainsKey(maNganh))
            {
                tenNganh = maNganhToTenNganh[maNganh]; // Lấy tên ngành từ dictionary
            }

            // Thống kê số lượng sinh viên
            ViewData["TotalStudents"] = model.Count;
            ViewData["TenNganh"] = tenNganh;
            ViewData["TotalByMaNganh"] = totalByMaNganh;


            // Thêm thông báo số lượng sinh viên tham gia ngành
            if (!string.IsNullOrEmpty(maNganh))
            {
                ViewData["Message"] = $"Có {totalByMaNganh} sinh viên tham gia ngành {tenNganh}.";
            }

            // Chỉ hiển thị thông báo khi nhấn nút "Thống kê"
            if (!string.IsNullOrEmpty(thongKe))
            {
                ViewData["Message"] = $"Có {totalByMaNganh} sinh viên tham gia ngành {tenNganh}.";
            }

            // Trả danh sách sinh viên về view
            return View(model);
        }

    }
}
