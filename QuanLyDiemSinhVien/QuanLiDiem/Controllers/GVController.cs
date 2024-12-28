using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLiDiem.Models;
using QuanLiDiem.Data;

namespace QuanLiDiem.Controllers
{
    public class GVController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GVController(ApplicationDbContext context)
        {
            _context = context;
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
