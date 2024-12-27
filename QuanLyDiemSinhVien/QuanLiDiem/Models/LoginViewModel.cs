using System.ComponentModel.DataAnnotations;

public class LoginViewModel
{
    [Required(ErrorMessage = "Tên đăng nhập là bắt buộc.")]
    public string? TenTaiKhoan { get; set; }

    [Required(ErrorMessage = "Mật khẩu là bắt buộc.")]
    public string? MatKhau { get; set; }
}
