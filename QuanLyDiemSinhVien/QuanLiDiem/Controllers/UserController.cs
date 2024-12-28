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
