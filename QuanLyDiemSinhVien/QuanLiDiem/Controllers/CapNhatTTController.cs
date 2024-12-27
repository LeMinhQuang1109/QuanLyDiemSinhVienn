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
    public class CapNhatTTController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CapNhatTTController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: DanhSachSinhVien

        // GET: DanhSachSinhVien
        public IActionResult Index(string searchTerm)
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

        // GET: CapNhatTT/Details/{MSSV}
        public async Task<IActionResult> Details(string MSSV)
        {
            if (string.IsNullOrEmpty(MSSV))
            {
                return NotFound();
            }

            var thongTinSV = await _context.DanhSachSinhVien
                .FirstOrDefaultAsync(m => m.MSSV == MSSV);

            if (thongTinSV == null)
            {
                return NotFound();
            }

            return View(thongTinSV);
        }


        // GET: DanhSachSinhVien/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: DanhSachSinhVien/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MSSV,HoTen,GioiTinh,CanCuocCongDan,SoDienThoai,Email,DiaChi,MaNganh,TenTaiKhoan,MatKhau,VaiTro")] DanhSachSinhVien thongTinSV)
        {
            if (ModelState.IsValid)
            {
                _context.Add(thongTinSV);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(thongTinSV);
        }


        // GET: DanhSachSinhVien/Edit
        public async Task<IActionResult> Edit(string? MSSV)
        {
            if (MSSV == null)
            {
                return NotFound();
            }

            var thongTinSV = await _context.DanhSachSinhVien
                .FirstOrDefaultAsync(m => m.MSSV == MSSV);

            if (thongTinSV == null)
            {
                return NotFound();
            }

            return View(thongTinSV);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string mssv, [Bind("MSSV,HoTen,GioiTinh,CanCuocCongDan,SoDienThoai,Email,DiaChi,MaNganh,TenTaiKhoan,MatKhau,VaiTro")] DanhSachSinhVien thongTinSV)
        {
            if (mssv != thongTinSV.MSSV)
            {
                return NotFound();
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


        // GET: DanhSachSinhVien/Delete/5
        public async Task<IActionResult> Delete(string? mssv)
        {
            if (mssv == null)
            {
                return NotFound();
            }

            var thongTinSV = await _context.DanhSachSinhVien
                .FirstOrDefaultAsync(m => m.MSSV == mssv);
            if (thongTinSV == null)
            {
                return NotFound();
            }

            return View(thongTinSV);
        }

        // POST: DanhSachSinhVien/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string mssv)
        {
            var thongTinSV = await _context.DanhSachSinhVien.FindAsync(mssv);
            if (thongTinSV != null)
            {
                _context.DanhSachSinhVien.Remove(thongTinSV);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ThongTinSVExists(string mssv)
        {
            return _context.DanhSachSinhVien.Any(e => e.MSSV == mssv);
        }
    }
}
