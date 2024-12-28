using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QuanLiDiem.Models;

public partial class LopHocPhan
{
    public string MaHP { get; set; } = null!;

    public string TenHP { get; set; } = null!;

    public string? MaGV { get; set; }

    public DateTime? NgayTao { get; set; }
    public DateTime? NgayBatDau { get; set; }
    public DateTime? NgayKetThuc { get; set; }

    public string? GhiChu { get; set; }

    public virtual GiangVien? MaGVNavigation { get; set; }
}
