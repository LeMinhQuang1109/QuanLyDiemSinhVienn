using QuanLiDiem.Models;
using System;
using System.Collections.Generic;

namespace QuanLiDiem.Models;

public partial class GiangVien
{
    public string MaGV { get; set; } = null!;

    public string TenGV { get; set; } = null!;

    public string? Email { get; set; }

    public string? SoDienThoai { get; set; }

    public string? DiaChi { get; set; }

    public virtual ICollection<LopHocPhan> LopHocPhans { get; set; } = new List<LopHocPhan>();
}
