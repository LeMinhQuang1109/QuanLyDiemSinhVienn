using System.ComponentModel.DataAnnotations;

namespace QuanLiDiem.Models
{
    public class ChangePasswordViewModel
    {
        [Display(Name = "Mã Sinh Viên")]
        public string? TenTaiKhoan { get; set; }

        [Display(Name = "Mật khẩu cũ")]
        public string? MatKhauCu { get; set; }

        [Display(Name = "Mật khẩu mới")]
        public string? MatKhauMoi { get; set; }

        [Display(Name = "Xác nhận mật khẩu mới")]
        public string? XacNhanMatKhau { get; set; }
    }
}

