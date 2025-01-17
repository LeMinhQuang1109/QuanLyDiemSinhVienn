﻿using System;
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
        public IActionResult Index(string searchTerm)
        {
            // Lấy MGV từ session
            var maGV = HttpContext.Session.GetString("MaGV");

            if (string.IsNullOrEmpty(maGV))
            {
                return RedirectToAction("Login", "Home");
            }

            // Lấy danh sách lớp học phần thuộc giảng viên
            var lopHocPhans = _context.LopHocPhans
                .Where(lhp => lhp.MaGV == maGV) // Lọc lớp học phần theo MaGV từ session
                .AsQueryable();

            // Lọc theo từ khóa tìm kiếm (Mã HP hoặc Tên HP)
            if (!string.IsNullOrEmpty(searchTerm))
            {
                lopHocPhans = lopHocPhans.Where(lhp =>
                    lhp.MaHP.Contains(searchTerm) || lhp.TenHP.Contains(searchTerm));
            }

            // Lấy danh sách kết quả
            var model = lopHocPhans.ToList();

            // Thêm thông báo nếu có tìm kiếm
            if (!string.IsNullOrEmpty(searchTerm))
            {
                ViewData["Message"] = $"Tìm thấy {model.Count} lớp học phần.";
            }

            // Truyền dữ liệu tìm kiếm cho View
            ViewData["SearchTerm"] = searchTerm;

            return View(model);
        }



        // GET: Diems
        public async Task<IActionResult> DiemSV()
        {
            return View(await _context.Diem.ToListAsync());
        }

        // GET: Diems/Create/{MSSV}
        public IActionResult NhapDiem(string mssv)
        {
            if (string.IsNullOrEmpty(mssv))
            {
                return NotFound();
            }

            
            // Tạo đối tượng Diem mới và gán MSSV từ query string
            var diem = new Diem
            {
                MSSV = mssv
            };

            // Truyền đối tượng diem vào view
            ViewData["Diem"] = diem;

            return View();  // Truyền đối tượng diem vào view
        }




        // POST: Diems/Create
        [HttpPost]

        public async Task<IActionResult> NhapDiem([Bind("MSSV,MaHP,TenHP,SoTinChi,DiemQuaTrinh,DiemCuoiKy,Diem10,Diem4,KetQua,HocKy,NamHoc")] Diem diem)
        {
            // Lấy thông tin từ bảng LopHocPhan
            var lopHocPhan = await _context.LopHocPhans
                                            .Where(lhp => lhp.MaHP == diem.MaHP)
                                            .FirstOrDefaultAsync();

            if (lopHocPhan != null)
            {
                diem.TenHP = lopHocPhan.TenHP;  // Cập nhật TenHP từ LopHocPhan
            }

            diem.Diem10 = Math.Round(diem.Diem10, 2);
            diem.Diem4 = Math.Round(diem.Diem4, 2);

            diem.Diem10 = diem.DiemQuaTrinh * 0.4 + diem.DiemCuoiKy * 0.6;
            if (diem.Diem10 >= 8.5) diem.Diem4 = 4.0;
            else if (diem.Diem10 >= 7.0) diem.Diem4 = 3.0;
            else if (diem.Diem10 >= 5.5) diem.Diem4 = 2.0;
            else if (diem.Diem10 >= 4.0) diem.Diem4 = 1.0;
            else diem.Diem4 = 0.0;

            diem.KetQua = diem.Diem10 >= 4.0 ? "Đạt" : "Không đạt";
            _context.Add(diem);
            await _context.SaveChangesAsync();
            return RedirectToAction("", "GiangVien");
        }

        // GET: Diems/Edit/5
        public async Task<IActionResult> SuaDiem(int id)
        {
            // Tìm kiếm bản ghi dựa trên Id (khóa chính)
            var diem = await _context.Diem.FirstOrDefaultAsync(d => d.Id == id);
            if (diem == null)
            {
                return NotFound();
            }
            // Lấy danh sách các học phần từ lớp học phần (hoặc bảng tương ứng)
            var danhSachHocPhan = await _context.LopHocPhans.ToListAsync();

            return View(diem); // Trả về view hiển thị thông tin cần sửa
        }

        // POST: Diems/Edit/5
        [HttpPost]
        public async Task<IActionResult> SuaDiem(int id, [Bind("Id,MSSV,MaHP,TenHP,SoTinChi,DiemQuaTrinh,DiemCuoiKy,Diem10,Diem4,KetQua,HocKy,NamHoc")] Diem diem)
        {
            if (id != diem.Id) // Kiểm tra xem Id trong URL và Id của bản ghi có khớp không
            {
                return NotFound();
            }

            try
            {
                // Tính toán lại điểm số
                diem.Diem10 = diem.DiemQuaTrinh * 0.4 + diem.DiemCuoiKy * 0.6;
                diem.Diem10 = Math.Round(diem.Diem10, 2); // Làm tròn đến 2 chữ số thập phân

                // Quy đổi điểm 10 sang điểm 4
                if (diem.Diem10 >= 8.5) diem.Diem4 = 4.0;
                else if (diem.Diem10 >= 7.0) diem.Diem4 = 3.0;
                else if (diem.Diem10 >= 5.5) diem.Diem4 = 2.0;
                else if (diem.Diem10 >= 4.0) diem.Diem4 = 1.0;
                else diem.Diem4 = 0.0;

                diem.Diem4 = Math.Round(diem.Diem4, 2); // Làm tròn điểm 4 nếu cần

                // Xác định kết quả học tập
                diem.KetQua = diem.Diem10 >= 4.0 ? "Đạt" : "Không đạt";

                // Cập nhật dữ liệu trong context
                _context.Update(diem);
                await _context.SaveChangesAsync(); // Lưu thay đổi vào database
            }
            catch (DbUpdateConcurrencyException)
            {
                // Xử lý lỗi cạnh tranh dữ liệu (nếu xảy ra)
                if (!DiemExists(diem.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw; // Nếu lỗi khác thì ném ngoại lệ
                }
            }

            // Điều hướng về trang tìm kiếm hoặc danh sách
            return RedirectToAction("DiemSV", "GiangVien");
        }

        // Phương thức kiểm tra sự tồn tại của bản ghi
        private bool DiemExists(int id)
        {
            return _context.Diem.Any(e => e.Id == id); // Kiểm tra dựa trên khóa chính (Id)
        }

        public IActionResult DSSV(string maHP, string searchTerm)
        {
            var sinhViens = _context.DanhSachSinhVien.AsQueryable();

            // Nếu có mã học phần, lọc danh sách theo mã học phần
            if (!string.IsNullOrEmpty(maHP))
            {
                sinhViens = sinhViens.Where(sv => sv.SinhVienHocPhans
                                .Any(svhp => svhp.MaHP.ToString() == maHP.ToString()));

            }

            // Nếu có từ khóa tìm kiếm, lọc thêm
            if (!string.IsNullOrEmpty(searchTerm))
            {
                sinhViens = sinhViens.Where(sv => sv.MSSV.Contains(searchTerm) || sv.HoTen.Contains(searchTerm));
            }

            ViewData["SearchTerm"] = searchTerm;
            return View(sinhViens.ToList());
        }





        // GET: CapNhatTT/Details/{MGV}
        public async Task<IActionResult> ThongTinGV(string MaGV)
        {
            // Lấy MGV từ session
            var maGV = HttpContext.Session.GetString("MaGV");

            if (maGV == null)
            {
                return RedirectToAction("Login", "Home");
            }

            // Lấy thông tin giảng viên dựa trên MGV
            var thongTinGV = await _context.GiangViens
                .FirstOrDefaultAsync(d => d.MaGV == maGV);

            if (thongTinGV == null)
            {
                // Nếu không tìm thấy giảng viên, trả về thông báo lỗi hoặc trang phù hợp
                return NotFound("Không tìm thấy thông tin giảng viên.");
            }

            // Truyền thông tin sinh viên vào view
            return View(thongTinGV);
        }



        // GET: DanhSachGiangVien/Edit
        public IActionResult CapNhat()
        {
            // Lấy MSSV từ Session
            var magv = HttpContext.Session.GetString("MaGV");

            if (string.IsNullOrEmpty(magv))
            {
                return RedirectToAction("Login"); // Nếu chưa đăng nhập, chuyển hướng về trang Login
            }

            // Truy xuất thông tin sinh viên từ cơ sở dữ liệu
            var giangVien = _context.GiangViens.FirstOrDefault(gv => gv.MaGV == magv);

            if (giangVien == null)
            {
                return NotFound(); // Sinh viên không tồn tại
            }

            return View(giangVien);
        }


        [HttpPost]
        public IActionResult CapNhat(GiangVien model)
        {
            // Lấy MGV từ Session
            var magv = HttpContext.Session.GetString("MaGV");

            if (string.IsNullOrEmpty(magv))
            {
                return RedirectToAction("Login"); // Nếu chưa đăng nhập, chuyển hướng về trang Login
            }

            // Tìm giảng viên trong cơ sở dữ liệu dựa trên MSSV từ Session
            var giangVien = _context.GiangViens.FirstOrDefault(gv => gv.MaGV == magv);

            if (giangVien == null)
            {
                return NotFound(); 
            }

            // Cập nhật thông tin từ model
            giangVien.TenGV = model.TenGV;
            giangVien.SoDienThoai = model.SoDienThoai;
            giangVien.DiaChi = model.DiaChi;
            giangVien.Email = model.Email;
            giangVien.TenTaiKhoan = model.TenTaiKhoan;
            giangVien.MatKhau = model.MatKhau;

            // Lưu thay đổi vào cơ sở dữ liệu
            _context.SaveChanges();

            // Chuyển hướng về trang chi tiết
            return RedirectToAction("ThongTinGV");
        }






    }
}