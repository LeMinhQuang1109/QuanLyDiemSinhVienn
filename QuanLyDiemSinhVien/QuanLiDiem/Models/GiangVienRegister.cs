using System.ComponentModel.DataAnnotations;

namespace QuanLiDiem.Models
{
    public class GiangVienRegister
    {
        public int Id { get; set; } // Mã định danh tự động tăng

        [Display(Name = "Họ và tên")]
        [Required(ErrorMessage = "Họ tên là bắt buộc.")]
        public string? TenGV { get; set; }

        [Display(Name = "Email")]
        public string? Email { get; set; }  // Email

        [Display(Name = "Số điện thoại")]
        [Required(ErrorMessage = "Số điện thoại là bắt buộc.")]
        [RegularExpression(@"^0\d{9}$", ErrorMessage = "Số điện thoại phải bắt đầu bằng 0 và có đúng 10 chữ số.")]
        public string? SDT { get; set; }

        [Display(Name = "Địa chỉ")]
        [Required(ErrorMessage = "Địa Chỉ là bắt buộc.")]
        public string? DiaChi { get; set; }

    }
}
