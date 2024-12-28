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
    public class GiangVienController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GiangVienController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: DanhSachSinhVien
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

        // Hiển thị danh sách giảng viên và giao diện thêm/sửa
        public IActionResult CreateGV()
        {
            var giangViens = _context.GiangViens.ToList();
            ViewBag.GiangViens = giangViens; // Truyền danh sách giảng viên vào ViewBag
            return View(new GiangVien()); // Truyền model rỗng để dùng cho form thêm
        }

        // Thêm hoặc sửa giảng viên - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateOrEdit(GiangVien giangVien)
        {
            if (ModelState.IsValid)
            {
                var existingGV = _context.GiangViens.Find(giangVien.MaGV);

                if (existingGV == null) // Nếu không tồn tại, thực hiện thêm mới
                {
                    _context.GiangViens.Add(giangVien);
                }
                else // Nếu tồn tại, thực hiện cập nhật
                {
                    existingGV.TenGV = giangVien.TenGV;
                    existingGV.Email = giangVien.Email;
                    existingGV.SoDienThoai = giangVien.SoDienThoai;
                    existingGV.DiaChi = giangVien.DiaChi;
                    _context.GiangViens.Update(existingGV);
                }

                _context.SaveChanges();
                return RedirectToAction(nameof(CreateGV)); // Quay lại giao diện chính
            }

            ViewBag.GiangViens = _context.GiangViens.ToList(); // Truyền lại danh sách nếu có lỗi
            return View("CreateGV", giangVien);
        }

        // Xóa giảng viên
        public IActionResult Delete(string id)
        {
            var giangVien = _context.GiangViens.Find(id);

            // Kiểm tra nếu giảng viên còn được tham chiếu bởi lớp học phần
            var hasRelatedLopHocPhans = _context.LopHocPhans.Any(lhp => lhp.MaGV == id);
            if (hasRelatedLopHocPhans)
            {
                // Lưu thông báo vào TempData để hiển thị trên giao diện
                TempData["ErrorMessage"] = "Không thể xóa giảng viên vì giảng viên này vẫn còn trong danh sách lớp học phần.";
                return RedirectToAction(nameof(CreateGV)); // Trở về trang danh sách giảng viên
            }

            // Nếu không còn tham chiếu, tiến hành xóa
            if (giangVien != null)
            {
                _context.GiangViens.Remove(giangVien);
                _context.SaveChanges(); // Lưu thay đổi
            }

            return RedirectToAction(nameof(CreateGV)); // Trở về trang danh sách giảng viên
        }

    }
}