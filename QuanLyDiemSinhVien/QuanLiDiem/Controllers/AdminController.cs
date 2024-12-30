using QuanLiDiem.Models;
using Microsoft.AspNetCore.Mvc;
using QuanLiDiem.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace QuanLiDiem.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Phương thức hiển thị danh sách giảng viên
        public IActionResult DSGiangVien()
        {
            // Lấy danh sách giảng viên từ cơ sở dữ liệu
            var danhSachGiangVien = _context.GiangViens.ToList();

            // Truyền danh sách sang view
            return View(danhSachGiangVien);
        }

        [HttpPost]
        public IActionResult XoaGiangVien(string maGV)
        {
            var giangVien = _context.GiangViens.FirstOrDefault(gv => gv.MaGV == maGV);

            if (giangVien != null)
            {
                _context.GiangViens.Remove(giangVien);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Xóa giảng viên thành công.";
            }
            else
            {
                TempData["ErrorMessage"] = "Giảng viên không tồn tại.";
            }

            return RedirectToAction("DanhSachGiangVien");
        }



        // Action để hiển thị danh sách lớp học phần
        public IActionResult DSLopHP()
        {
            // Lấy danh sách lớp học phần từ database
            var lopHocPhans = _context.LopHocPhans.Include(lhp => lhp.MaGVNavigation).ToList();
            // Gán danh sách giảng viên vào ViewBag
            ViewBag.GiangVienList = _context.GiangViens
                .Select(gv => new { gv.MaGV, gv.TenGV })
                .ToList();

            return View(lopHocPhans);
        }

        [HttpGet]
        public IActionResult IndexLHP(string filter)
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

        public IActionResult TaoLHP()
        {
            // Lấy danh sách giảng viên từ database và truyền vào ViewBag
            ViewBag.GiangVienList = _context.GiangViens
                .Select(gv => new { gv.MaGV, gv.TenGV })
                .ToList();

            // Trả về View cho form Create
            return View();
        }

        [HttpPost]
        public IActionResult TaoLHP(LopHocPhan lopHocPhan)
        {
            // Kiểm tra ngày kết thúc không được trước ngày bắt đầu
            if (lopHocPhan.NgayKetThuc < lopHocPhan.NgayBatDau)
            {
                TempData["DayErrorMessage"] = "Ngày kết thúc không thể trước ngày bắt đầu!";
                return RedirectToAction("TaoLHP");
            }

            if (ModelState.IsValid)
            {
                // Gán ngày tạo nếu chưa có
                if (lopHocPhan.NgayTao == null)
                {
                    lopHocPhan.NgayTao = DateTime.Now;
                }

                Random rand = new Random();
                lopHocPhan.MaHP = $"2411050{rand.Next(100, 1000):D3}";  // Tạo MaHP với 3 số ngẫu nhiên cuối


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


        [HttpGet]
        public IActionResult SuaLHP(String id)
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
        public IActionResult SuaLHP(LopHocPhan updatedLopHocPhan)
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
                    return RedirectToAction("SuaLHP", new { id = updatedLopHocPhan.MaHP });
                    //return RedirectToAction("Index");
                }
                catch (DbUpdateException ex)
                {
                    TempData["EditErrorMessage"] = "Có lỗi xảy ra khi lưu dữ liệu";
                    return RedirectToAction("SuaLHP", new { id = updatedLopHocPhan.MaHP });
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
        public IActionResult XoaLHP(string id)
        {
            var lopHocPhan = _context.LopHocPhans.FirstOrDefault(lhp => lhp.MaHP == id);

            if (lopHocPhan == null)
            {
                TempData["Error"] = "Không tìm thấy lớp học phần!";
                return RedirectToAction("DSLopHP");
            }

            // Xóa lớp học phần khỏi bảng LopHocPhan
            _context.LopHocPhans.Remove(lopHocPhan);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Lớp học phần đã được xóa thành công!";
            return RedirectToAction("DSLopHP");
        }

        // Tìm kiếm
        public IActionResult TimKiem(string? search)
        {
            // Lấy danh sách từ database
            var data = _context.LopHocPhans
                .Include(lhp => lhp.MaGVNavigation) // Include để lấy thông tin giảng viên
                .AsQueryable();

            // Nếu có từ khóa tìm kiếm
            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower(); // Chuyển từ khóa về chữ thường để so sánh không phân biệt hoa/thường
                data = data.Where(lhp =>
                    lhp.MaHP.ToLower().Contains(search) ||  // Tìm theo mã học phần
                    lhp.TenHP.ToLower().Contains(search)    // Tìm theo tên học phần
                );
            }

            // Truyền từ khóa tìm kiếm vào ViewBag để hiển thị lại trong giao diện
            ViewBag.SearchQuery = search;

            // Trả về view hiển thị danh sách (DSLopHP) với dữ liệu đã lọc
            return View("DSLopHP", data.ToList());
        }


        [HttpGet]
        public IActionResult DSLopHP(string filter)
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
            return View(lopHocPhanList);
        }




        public IActionResult DuyetSV()
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
            return RedirectToAction(nameof(DuyetSV));
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
    
