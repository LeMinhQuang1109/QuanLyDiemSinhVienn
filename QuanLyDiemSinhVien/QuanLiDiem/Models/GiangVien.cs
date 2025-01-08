using QuanLiDiem.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QuanLiDiem.Models;

public partial class GiangVien
{
    [Key]
    public string MaGV { get; set; }

    public string TenGV { get; set; }

    public string? Email { get; set; }

    public string? SoDienThoai { get; set; }

    public string? DiaChi { get; set; }

    [Display(Name = "Tên tài khoản")]
    public string? TenTaiKhoan { get; set; }  // Tên tài khoản

    [Display(Name = "Mật khẩu")]
    public string? MatKhau { get; set; }  // Mật khẩu

    [Display(Name = "Vai trò")]
    public string? VaiTro { get; set; }  // Vai trò

    public virtual ICollection<LopHocPhan> LopHocPhans { get; set; } = new List<LopHocPhan>();
}
