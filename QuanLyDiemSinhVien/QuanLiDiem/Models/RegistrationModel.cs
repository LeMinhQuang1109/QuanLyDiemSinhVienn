using System.ComponentModel.DataAnnotations;

namespace QuanLiDiem.Models
{
    public class RegistrationModel
    {
        public int Id { get; set; } // Mã định danh tự động tăng

        [Display(Name = "Họ và tên")]
        [Required(ErrorMessage = "Họ tên là bắt buộc.")]
        public string? HoTen { get; set; }

        [Display(Name = "CMND/CCCD")]
        [Required(ErrorMessage = "CCCD")]
        [RegularExpression(@"^\d{12}$", ErrorMessage = "CMND phải có đúng 12 chữ số.")]
        public string? CCCD { get; set; }

        [Display(Name = "Số điện thoại")]
        [Required(ErrorMessage = "Số điện thoại là bắt buộc.")]
        [RegularExpression(@"^0\d{9}$", ErrorMessage = "Số điện thoại phải bắt đầu bằng 0 và có đúng 10 chữ số.")]
        public string? SDT { get; set; }

        [Display(Name = "Giới tính")]
        [Required(ErrorMessage = "Giới tính là bắt buộc.")]
        public string? GioiTinh { get; set; }

        [Display(Name = "Địa chỉ")]
        [Required(ErrorMessage = "Địa Chỉ là bắt buộc.")]
        public string? DiaChi { get; set; }

        [Display(Name = "Ngành Học")]
        [Required(ErrorMessage = "Ngành học là bắt buộc.")]
        public string? NganhHoc { get; set; }


        [Display(Name = "Điểm TBM1")]
        [Required(ErrorMessage = "Điểm TBM 1 là bắt buộc.")]
        [Range(0, 10, ErrorMessage = "Điểm TBM 1 phải trong khoảng từ 0 đến 10.")]
        public decimal? DTB1 { get; set; }

        [Display(Name = "Điểm TBM2")]
        [Required(ErrorMessage = "Điểm TBM 2 là bắt buộc.")]
        [Range(0, 10, ErrorMessage = "Điểm TBM 2 phải trong khoảng từ 0 đến 10.")]
        public decimal? DTB2 { get; set; }

        [Display(Name = "Điểm TBM3")]
        [Required(ErrorMessage = "Điểm TBM 3 là bắt buộc.")]
        [Range(0, 10, ErrorMessage = "Điểm TBM 3 phải trong khoảng từ 0 đến 10.")]
        public decimal? DTB3 { get; set; }

        [Display(Name = "Xếp loại")]
        [Required(ErrorMessage = "Xếp loại là bắt buộc.")]
        public string? XepLoai { get; set; }
    }
}
