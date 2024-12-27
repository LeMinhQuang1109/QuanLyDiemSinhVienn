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
    public class TimKiemController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TimKiemController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: DanhSachSinhVien
        public IActionResult Index(string searchTerm, string maNganh, string thongKe)
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