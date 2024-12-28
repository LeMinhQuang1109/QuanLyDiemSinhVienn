using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using QuanLiDiem.Models;

namespace QuanLiDiem.Models
{
    public class DanhSachLHP
    {
            public string? MSSV { get; set; }
            public string? HoTen { get; set; }
            public string TenNganh { get; set; }
            public string MaHP { get; set; } = null!;
            public string? Email { get; set; }
            public double DiemCuoiKy { get; set; }
            //public virtual DanhSachSinhVien? DanhSachSinhVien { get; set; }
            //public virtual LopHocPhan? LopHocPhan{ get; set; }
            //public virtual NganhHoc? NganhHoc {  get; set; } 
            //public virtual Diem? Diem {  get; set; }

    }
}
