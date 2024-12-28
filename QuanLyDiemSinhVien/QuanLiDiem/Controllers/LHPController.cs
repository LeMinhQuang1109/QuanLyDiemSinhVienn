using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLiDiem.Data;
using QuanLiDiem.Models;
using System.Linq;

namespace QuanLiDiem.Controllers
{
    public class LopHocPhanController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LopHocPhanController(ApplicationDbContext context)
        {
            _context = context;
        }


        // Action để hiển thị danh sách lớp học phần
        public IActionResult Index()
        {
            // Lấy danh sách lớp học phần từ database
            var lopHocPhans = _context.LopHocPhans.Include(lhp => lhp.MaGVNavigation).ToList();
            // Gán danh sách giảng viên vào ViewBag
            ViewBag.GiangVienList = _context.GiangViens
                .Select(gv => new { gv.MaGV, gv.TenGV })
                .ToList();

            return View(lopHocPhans);
        }

        public IActionResult Create()
        {
            // Lấy danh sách giảng viên từ database và truyền vào ViewBag
            ViewBag.GiangVienList = _context.GiangViens
                .Select(gv => new { gv.MaGV, gv.TenGV })
                .ToList();

            // Trả về View cho form Create
            return View();
        }

        [HttpPost]
        public IActionResult Create(LopHocPhan lopHocPhan)
        {
            // Kiểm tra ngày kết thúc không được trước ngày bắt đầu
            if (lopHocPhan.NgayKetThuc < lopHocPhan.NgayBatDau)
            {
                TempData["DayErrorMessage"] = "Ngày kết thúc không thể trước ngày bắt đầu!";
                return RedirectToAction("Create");
            }

            // Kiểm tra mã lớp học phần có trùng hay không
            if (_context.LopHocPhans.Any(l => l.MaHP == lopHocPhan.MaHP))
            {
                TempData["EditErrorMessage"] = "Lớp học phần đã tồn tại!";
                return RedirectToAction("Create");
            }

            if (ModelState.IsValid)
            {
                // Gán ngày tạo nếu chưa có
                if (lopHocPhan.NgayTao == null)
                {
                    lopHocPhan.NgayTao = DateTime.Now;
                }

                // Thêm lớp học phần vào database
                _context.LopHocPhans.Add(lopHocPhan);
                _context.SaveChanges();

                // Quay lại trang danh sách sau khi thêm thành công
                TempData["EditSuccessMessage"] = "Tạo lớp học phần thành công!";
                ViewBag.GiangVienList = _context.GiangViens
                .Select(gv => new { gv.MaGV, gv.TenGV })
                .ToList();
                return View(new LopHocPhan());
            }

            // Nếu có lỗi, giữ lại danh sách giảng viên để hiển thị trên View
            ViewBag.GiangVienList = _context.GiangViens
                .Select(gv => new { gv.MaGV, gv.TenGV })
                .ToList();

            // Trả về View Create với Model lỗi
            return View(lopHocPhan);
        }


        //tìm kiếm
        public IActionResult Search(string? search)
        {
            // Lấy danh sách từ database
            var data = _context.LopHocPhans
                .Include(lhp => lhp.MaGVNavigation) // Include để lấy thông tin giảng viên
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                data = data.Where(lhp =>
                    lhp.MaHP.ToLower().Contains(search) ||
                    lhp.TenHP.ToLower().Contains(search) ||
                    lhp.MaGV.ToLower().Contains(search) ||
                    (lhp.MaGVNavigation.TenGV != null &&
                     lhp.MaGVNavigation.TenGV.ToLower().Contains(search)) ||
                    lhp.GhiChu.ToLower().Contains(search)
                );
            }

            ViewBag.SearchQuery = search;

            return View("Index", data.ToList());
        }

        //lọc

        [HttpGet]
        public IActionResult Index(string filter)
        {
            var lopHocPhans = _context.LopHocPhans
                .Include(lhp => lhp.MaGVNavigation)
                .AsQueryable(); // Sử dụng AsQueryable để linh hoạt áp dụng điều kiện

            if (!string.IsNullOrEmpty(filter))
            {
                switch (filter.ToLower())
                {
                    case "date":
                        // Sắp xếp theo ngày tạo (mới nhất -> cũ nhất)
                        lopHocPhans = lopHocPhans.OrderByDescending(l => l.NgayTao)
                                                 .ThenBy(l => l.MaHP); // Nếu ngày giống nhau, xếp theo mã lớp
                        break;
                    case "tenlophocphan":
                        // Sắp xếp theo tên lớp học phần (A -> Z)
                        lopHocPhans = lopHocPhans.OrderBy(l => l.TenHP)
                                                 .ThenBy(l => l.MaHP); // Nếu tên giống nhau, xếp theo mã lớp
                        break;
                    case "tengiangvien":
                        // Sắp xếp theo tên giảng viên (A -> Z)
                        lopHocPhans = lopHocPhans.OrderBy(l => l.MaGVNavigation.TenGV)
                                                 .ThenBy(l => l.MaHP); // Nếu tên giống nhau, xếp theo mã lớp
                        break;
                    case "malop":
                        // Sắp xếp theo mã lớp (A -> Z, sau đó theo số nếu giống nhau)
                        lopHocPhans = lopHocPhans.OrderBy(l => l.MaHP);
                        break;
                    default:
                        break;
                }
            }

            var lopHocPhanList = lopHocPhans.ToList();
            return View(lopHocPhanList); // Trả về danh sách đã sắp xếp
        }


        [HttpGet]
        public IActionResult Edit(string id)
        {
            // Tìm lớp học phần theo mã
            var lopHocPhan = _context.LopHocPhans
                .FirstOrDefault(lhp => lhp.MaHP == id);

            if (lopHocPhan == null)
            {
                return NotFound(); // Không tìm thấy lớp học phần
            }

            var giangVienList = _context.GiangViens
            .Select(gv => new { gv.MaGV, gv.TenGV })
            .ToList();

            ViewBag.GiangVienList = giangVienList;
            // Trả về View và truyền model lớp học phần vào view
            return View(lopHocPhan);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(LopHocPhan updatedLopHocPhan)
        {

            if (updatedLopHocPhan.NgayKetThuc < updatedLopHocPhan.NgayBatDau)
            {
                TempData["DayErrorMessage"] = "Ngày kết thúc không thể trước ngày bắt đầu.";
                return RedirectToAction("Edit", new { id = updatedLopHocPhan.MaHP });
            }


            if (ModelState.IsValid)
            {
                // Tìm lớp học phần trong database
                var lopHocPhan = _context.LopHocPhans
                    .FirstOrDefault(lhp => lhp.MaHP == updatedLopHocPhan.MaHP);

                if (lopHocPhan == null)
                {
                    return NotFound();
                }

                // Cập nhật thông tin lớp học phần
                if (!string.IsNullOrEmpty(updatedLopHocPhan.TenHP))
                {
                    lopHocPhan.TenHP = updatedLopHocPhan.TenHP;
                }

                if (!string.IsNullOrEmpty(updatedLopHocPhan.MaGV))
                {
                    lopHocPhan.MaGV = updatedLopHocPhan.MaGV;
                }

                if (updatedLopHocPhan.NgayBatDau.HasValue)
                {
                    lopHocPhan.NgayBatDau = updatedLopHocPhan.NgayBatDau;
                }

                if (updatedLopHocPhan.NgayKetThuc.HasValue)
                {
                    lopHocPhan.NgayKetThuc = updatedLopHocPhan.NgayKetThuc;
                }

                if (!string.IsNullOrEmpty(updatedLopHocPhan.GhiChu))
                {
                    lopHocPhan.GhiChu = updatedLopHocPhan.GhiChu;
                }
                else
                {
                    lopHocPhan.GhiChu = null;  // Hoặc gán giá trị trống nếu bạn muốn ghi chú trống
                }

                lopHocPhan.NgayTao = DateTime.Now; // Thời gian cập nhật

                try
                {
                    _context.SaveChanges();
                    TempData["EditSuccessMessage"] = "Lớp học phần đã được cập nhật thành công!";
                    return RedirectToAction("Edit", new { id = updatedLopHocPhan.MaHP });
                    //return RedirectToAction("Index");
                }
                catch (DbUpdateException ex)
                {
                    TempData["EditErrorMessage"] = "Có lỗi xảy ra khi lưu dữ liệu";
                    return RedirectToAction("Edit", new { id = updatedLopHocPhan.MaHP });
                }
            }

            // Nếu có lỗi, lấy lại danh sách giảng viên
            var giangVienList = _context.GiangViens
                .Select(gv => new { gv.MaGV, gv.TenGV })
                .ToList();
            ViewBag.GiangVienList = giangVienList;

            return View(updatedLopHocPhan);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(string id)
        {
            var lopHocPhan = _context.LopHocPhans.FirstOrDefault(lhp => lhp.MaHP == id);

            if (lopHocPhan == null)
            {
                TempData["Error"] = "Không tìm thấy lớp học phần!";
                return RedirectToAction("Index");
            }

            // Xóa lớp học phần khỏi bảng LopHocPhan
            _context.LopHocPhans.Remove(lopHocPhan);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Lớp học phần đã được xóa thành công!";
            return RedirectToAction("Index");
        }
    }
}

