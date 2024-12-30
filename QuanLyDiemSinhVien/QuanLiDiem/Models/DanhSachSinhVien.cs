using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace QuanLiDiem.Models
{
    public class DanhSachSinhVien
    {
        [Key] 
        public string? MSSV { get; set; }  // Mã số sinh viên

        [Display(Name = "Họ tên")]
        public string? HoTen { get; set; }  // Họ tên

        [Display(Name = "Giới tính")]
        public string? GioiTinh { get; set; }  // Giới tính

        [Display(Name = "CMND/CCCD")]
        [RegularExpression(@"^\d{12}$", ErrorMessage = "CMND phải có đúng 12 chữ số.")]
        public string? CanCuocCongDan { get; set; }


        [Display(Name = "Số điện thoại")]
        [RegularExpression(@"^0\d{9}$", ErrorMessage = "Số điện thoại phải bắt đầu bằng 0 và có đúng 10 chữ số.")]
        public string? SoDienThoai { get; set; }  // Số điện thoại

        [Display(Name = "Email")]
        public string? Email { get; set; }  // Email

        [Display(Name = "Địa chỉ")]
        public string? DiaChi { get; set; }  // Địa chỉ

        [Display(Name = "Mã ngành")]
        public string? MaNganh { get; set; }  // Mã ngành

        [Display(Name = "Tên tài khoản")]
        public string? TenTaiKhoan { get; set; }  // Tên tài khoản

        [Display(Name = "Mật khẩu")]
        public string? MatKhau { get; set; }  // Mật khẩu

        [Display(Name = "Vai trò")]
        public string? VaiTro { get; set; }  // Vai trò

        // Mối quan hệ với học phần
        public ICollection<SinhVien_HocPhan> SinhVienHocPhans { get; set; }

    }
}
