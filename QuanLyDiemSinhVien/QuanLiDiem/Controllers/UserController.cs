using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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


        [HttpGet]
        public async Task<IActionResult> DKHP()
        {
            // Lấy danh sách các lớp học phần có sẵn
            var danhSachLopHocPhan = await _context.LopHocPhans.ToListAsync();

            // Lấy mã sinh viên từ Session
            var maSV = HttpContext.Session.GetString("MSV");

            if (maSV == null)
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập.";
                return RedirectToAction("Login", "Home");
            }

            // Truyền danh sách lớp học phần vào View
            return View(danhSachLopHocPhan);
        }

        [HttpPost]
        public async Task<IActionResult> DKHP(string maHP)
        {
            var maSV = HttpContext.Session.GetString("MSV");

            if (maSV == null)
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập.";
                return RedirectToAction("Login", "Home");
            }

            // Kiểm tra xem sinh viên có tồn tại trong bảng DanhSachSinhVien hay không
            var sinhVien = await _context.DanhSachSinhVien.FirstOrDefaultAsync(sv => sv.MSSV == maSV);
            if (sinhVien == null)
            {
                TempData["ErrorMessage"] = "Sinh viên không tồn tại.";
                return RedirectToAction("DKHP");
            }

            // Kiểm tra xem sinh viên đã đăng ký lớp học phần này chưa
            var existingRegistration = await _context.SinhViens
                .FirstOrDefaultAsync(dk => dk.MaHP == maHP && dk.SinhVienId == maSV);

            if (existingRegistration != null)
            {
                TempData["ErrorMessage"] = "Bạn đã đăng ký lớp học phần này rồi!";
                return RedirectToAction("DKHP");
            }

            // Kiểm tra xem lớp học phần có tồn tại không
            var lopHocPhan = await _context.LopHocPhans.FirstOrDefaultAsync(lh => lh.MaHP == maHP);
            if (lopHocPhan == null)
            {
                TempData["ErrorMessage"] = "Lớp học phần không tồn tại.";
                return RedirectToAction("DKHP");
            }

            // Tạo mới bản ghi trong bảng SinhVien_HocPhan
            var dangKyHocPhan = new SinhVien_HocPhan
            {
                SinhVienId = maSV,  // Sử dụng SinhVienId thay cho MSSV
                MaHP = maHP,
            };

            _context.SinhViens.Add(dangKyHocPhan); // Đảm bảo thêm vào bảng SinhVien_HocPhan

            // Lưu thay đổi vào cơ sở dữ liệu
            try
            {
                int result = await _context.SaveChangesAsync();
                if (result > 0)
                {
                    TempData["SuccessMessage"] = "Đăng ký lớp học phần thành công!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Đã có lỗi xảy ra trong quá trình đăng ký.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi khi đăng ký lớp học phần: {ex.Message}";
                if (ex.InnerException != null)
                {
                    TempData["ErrorMessage"] += $" Inner Exception: {ex.InnerException.Message}";
                }
            }

            return RedirectToAction("DKHP");
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
        public IActionResult CapNhat()
        {
            // Lấy MSSV từ Session
            var mssv = HttpContext.Session.GetString("MSV");

            if (string.IsNullOrEmpty(mssv))
            {
                return RedirectToAction("Login"); // Nếu chưa đăng nhập, chuyển hướng về trang Login
            }

            // Truy xuất thông tin sinh viên từ cơ sở dữ liệu
            var sinhVien = _context.DanhSachSinhVien.FirstOrDefault(sv => sv.MSSV == mssv);

            if (sinhVien == null)
            {
                return NotFound(); // Sinh viên không tồn tại
            }

            return View(sinhVien);
        }


        [HttpPost]
        public IActionResult CapNhat(DanhSachSinhVien model)
        {
            // Lấy MSSV từ Session
            var mssv = HttpContext.Session.GetString("MSV");

            if (string.IsNullOrEmpty(mssv))
            {
                return RedirectToAction("Login"); // Nếu chưa đăng nhập, chuyển hướng về trang Login
            }

            // Tìm sinh viên trong cơ sở dữ liệu dựa trên MSSV từ Session
            var sinhVien = _context.DanhSachSinhVien.FirstOrDefault(sv => sv.MSSV == mssv);

            if (sinhVien == null)
            {
                return NotFound(); // Sinh viên không tồn tại
            }

            // Cập nhật thông tin từ model
            sinhVien.HoTen = model.HoTen;
            sinhVien.GioiTinh = model.GioiTinh;
            sinhVien.CanCuocCongDan = model.CanCuocCongDan;
            sinhVien.SoDienThoai = model.SoDienThoai;
            sinhVien.DiaChi = model.DiaChi;
            sinhVien.Email = model.Email;
            sinhVien.MaNganh = model.MaNganh;
            sinhVien.TenTaiKhoan = model.TenTaiKhoan;
            sinhVien.MatKhau = model.MatKhau;

            // Lưu thay đổi vào cơ sở dữ liệu
            _context.SaveChanges();

            // Chuyển hướng về trang chi tiết
            return RedirectToAction("ThongTin");
        }


        public IActionResult TimKiem(int? hocKy, string namHoc)
        {
            // Lấy MSSV từ session
            string mssv = HttpContext.Session.GetString("MSV");

            if (string.IsNullOrEmpty(mssv))
            {
                return RedirectToAction("Login"); // Nếu chưa đăng nhập, chuyển hướng về trang Login
            }

            // Lọc dữ liệu theo điều kiện tìm kiếm
            var danhSachDiem = _context.Diem.AsQueryable();

            if (hocKy.HasValue)
            {
                danhSachDiem = danhSachDiem.Where(d => d.HocKy == hocKy);
            }
            if (!string.IsNullOrEmpty(namHoc))
            {
                danhSachDiem = danhSachDiem.Where(d => d.NamHoc == namHoc);
            }

            // Lọc điểm theo MSSV (chỉ lấy điểm của sinh viên hiện tại)
            danhSachDiem = danhSachDiem.Where(d => d.MSSV == mssv);

            // Lấy danh sách ngành học của sinh viên từ MSSV
            var sinhVien = _context.DanhSachSinhVien
                                    .FirstOrDefault(sv => sv.MSSV == mssv);
            string? nganhHoc = sinhVien?.MaNganh;

            // Truyền ngành học vào View
            ViewBag.NganhHoc = nganhHoc;

            // Truyền danh sách điểm vào View
            return View(danhSachDiem.ToList());
        }

    }
}

