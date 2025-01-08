using System.ComponentModel.DataAnnotations;

public class LoginViewModel
{
    [Display(Name = "Tài khoản")]
    [Required(ErrorMessage = "Tên đăng nhập là bắt buộc.")]
    public string? TenTaiKhoan { get; set; }

    [Display(Name = "Mật khẩu")]
    [Required(ErrorMessage = "Mật khẩu là bắt buộc.")]
    public string? MatKhau { get; set; }
}
