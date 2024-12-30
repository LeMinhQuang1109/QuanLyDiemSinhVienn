using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLiDiem.Models
{
    public class SinhVien_HocPhan
    {
        [Key]
        public int Id { get; set; } 

        [ForeignKey("SinhVien")]
        public string SinhVienId { get; set; }
        public DanhSachSinhVien SinhVien { get; set; }

        [ForeignKey("LopHocPhan")]
        public string MaHP { get; set; }
        public LopHocPhan LopHocPhan { get; set; }
    }
}
