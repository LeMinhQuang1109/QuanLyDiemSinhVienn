using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace QuanLiDiem.Models
{
    public class DanhSachSinhVien
    {
        [Key]  // Đánh dấu MSSV là khóa chính
        public string? MSSV { get; set; }  // Mã số sinh viên

        [Display(Name = "Họ tên")]
        public string? HoTen { get; set; }  // Họ tên

        [Display(Name = "Giới tính")]
        public string? GioiTinh { get; set; }  // Giới tính

        [Display(Name = "CMND/CCCD")]
        [Required(ErrorMessage = "CMND là bắt buộc.")]
        [RegularExpression(@"^\d{12}$", ErrorMessage = "CMND phải có đúng 12 chữ số.")]
        public string? CanCuocCongDan { get; set; }  // Căn cước công dân

        [Display(Name = "Số điện thoại")]
        [Required(ErrorMessage = "Số điện thoại là bắt buộc.")]
        [RegularExpression(@"^0\d{9}$", ErrorMessage = "Số điện thoại phải bắt đầu bằng 0 và có đúng 10 chữ số.")]
        public string? SoDienThoai { get; set; }  // Số điện thoại

        [Display(Name = "Email")]
        public string? Email { get; set; }  // Email

        [Display(Name = "Địa chỉ")]
        [Required(ErrorMessage = "Địa chỉ là bắt buộc.")]
        public string? DiaChi { get; set; }  // Địa chỉ

        [Display(Name = "Mã ngành")]
        public string? MaNganh { get; set; }  // Mã ngành

        [Display(Name = "Tên tài khoản")]
        public string? TenTaiKhoan { get; set; }  // Tên tài khoản

        [Display(Name = "Mật khẩu")]
        public string? MatKhau { get; set; }  // Mật khẩu

        [Display(Name = "Vai trò")]
        public string? VaiTro { get; set; }  // Vai trò

        public ICollection<Diem> Diems { get; set; }
    }
}
