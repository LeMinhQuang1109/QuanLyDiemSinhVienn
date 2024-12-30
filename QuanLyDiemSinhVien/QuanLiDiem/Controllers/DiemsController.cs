using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLiDiem.Data;

namespace qlydiem.Controllers
{
    public class DiemsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DiemsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Diems
        public async Task<IActionResult> Index()
        {
            return View(await _context.Diem.ToListAsync());
        }

        // GET: Diems/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var diem = await _context.Diem
                .FirstOrDefaultAsync(m => m.MSSV == id);
            if (diem == null)
            {
                return NotFound();
            }

            return View(diem);
        }

        public IActionResult NhapDiem(string searchTerm)
        {
            var danhSachSinhVien = _context.DanhSachSinhVien
                .Where(sv => string.IsNullOrEmpty(searchTerm) || sv.MSSV.Contains(searchTerm) || sv.HoTen.Contains(searchTerm))
                .ToList();

            var danhSachMaHP = _context.LopHocPhans.Select(lhp => lhp.MaHP).ToList();

            // Sử dụng ViewBag để truyền dữ liệu
            ViewBag.DanhSachSinhVien = new SelectList(danhSachSinhVien, "MSSV", "HoTen");
            ViewBag.DanhSachMaHP = new SelectList(danhSachMaHP);

            var model = new Diem();  // Tạo model trống hoặc lấy từ database
            return View(model);
        }






        // POST: Diems/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]

        public async Task<IActionResult> Create([Bind("MSSV,MaHP,SoTinChi,DiemQuaTrinh,DiemCuoiKy,Diem10,Diem4,KetQua,HocKy,NamHoc")] Diem diem)
        {


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
            return RedirectToAction("GiangVien", "DSSV");
        }

        // GET: Diems/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var diem = await _context.Diem.FindAsync(id);
            if (diem == null)
            {
                return NotFound();
            }
            return View(diem);
        }

        // POST: Diems/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]

        public async Task<IActionResult> Edit(string id, [Bind("MSSV,MaHP,SoTinChi,DiemQuaTrinh,DiemCuoiKy,Diem10,Diem4,KetQua,HocKy,NamHoc")] Diem diem)
        {
            if (id != diem.MSSV)
            {
                return NotFound();
            }


            try
            {
                diem.Diem10 = diem.DiemQuaTrinh * 0.4 + diem.DiemCuoiKy * 0.6;
                if (diem.Diem10 >= 8.5) diem.Diem4 = 4.0;
                else if (diem.Diem10 >= 7.0) diem.Diem4 = 3.0;
                else if (diem.Diem10 >= 5.5) diem.Diem4 = 2.0;
                else if (diem.Diem10 >= 4.0) diem.Diem4 = 1.0;
                else diem.Diem4 = 0.0;

                diem.KetQua = diem.Diem10 >= 4.0 ? "Đạt" : "Không đạt";

                _context.Update(diem);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DiemExists(diem.MSSV))
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

        // GET: Diems/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var diem = await _context.Diem
                .FirstOrDefaultAsync(m => m.MSSV == id);
            if (diem == null)
            {
                return NotFound();
            }

            return View(diem);
        }

        // POST: Diems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var diem = await _context.Diem.FindAsync(id);
            if (diem != null)
            {
                _context.Diem.Remove(diem);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DiemExists(string id)
        {
            return _context.Diem.Any(e => e.MSSV == id);
        }
    }
}
