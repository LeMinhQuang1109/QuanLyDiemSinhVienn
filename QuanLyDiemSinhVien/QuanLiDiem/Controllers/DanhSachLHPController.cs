using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLiDiem.Data;
using QuanLiDiem.Models;

namespace QuanLiDiem.Controllers
{
    public class DanhSachLHPController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DanhSachLHPController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Action hiển thị danh sách sinh viên theo mã học phần
        public async Task<IActionResult> Index(string? maHP)
        {
            if (string.IsNullOrEmpty(maHP))
            {
                return NotFound("Mã học phần không được cung cấp.");
            }

            // Lấy thông tin lớp học phần
            var lopHocPhan = await _context.LopHocPhans
                .Include(lhp => lhp.MaGVNavigation) // Lấy thông tin giảng viên
                .FirstOrDefaultAsync(lhp => lhp.MaHP == maHP);

            if (lopHocPhan == null)
            {
                return NotFound($"Không tìm thấy lớp học phần với mã: {maHP}");
            }

            // Lấy danh sách sinh viên trong lớp học phần
            var danhSachSinhVien = await _context.Diem
                .Where(d => d.MaHP == maHP) // Lọc theo mã học phần
                .Include(d => d.SinhVien) // Kèm theo thông tin sinh viên
                .ThenInclude(sv => sv.NganhHoc) // Kèm theo ngành học
                .Select(d => new DanhSachLHP
                {
                    MSSV = d.SinhVien.MSSV,
                    HoTen = d.SinhVien.HoTen,
                    Email = d.SinhVien.Email,
                    DiemCuoiKy = d.DiemCuoiKy,
                    MaHP = d.MaHP,
                    TenNganh = d.SinhVien.NganhHoc.TenNganh
                })
                .ToListAsync();

            // Truyền thông tin lớp học phần qua ViewBag (nếu cần hiển thị tên lớp, giảng viên,...)
            ViewBag.TenHP = lopHocPhan.TenHP;

            return View("DSLHP", danhSachSinhVien); // Truyền danh sách sinh viên vào view
        }
    }
}
