using QuanLiDiem.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Diem
{
    [Key]
    public int Id { get; set; } 

    
    [Required]
    [Display(Name = "Mã số sinh viên")]
    public string MSSV { get; set; }

    [Required]
    [Display(Name = "Mã học phần")]
    public string MaHP { get; set; }

    [Required]
    [Range(1, 4, ErrorMessage = "Số tín chỉ phải từ 1 đến 4")]
    [Display(Name = "Số tín chỉ")]
    public int SoTinChi { get; set; }

    [Required]
    [Range(0, 10, ErrorMessage = "Điểm quá trình phải từ 0 đến 10")]
    [Display(Name = "Điểm quá trình")]
    public double DiemQuaTrinh { get; set; } 

    [Required]
    [Range(0, 10, ErrorMessage = "Điểm cuối kỳ phải từ 0 đến 10")]
    [Display(Name = "Điểm cuối kỳ")]
    public double DiemCuoiKy { get; set; }

    [Display(Name = "Điểm hệ 10")]
    public double Diem10 { get; set; }

    [Display(Name = "Điểm hệ 4")]
    public double Diem4 { get; set; }

    [Display(Name = "Kết quả")]
    public string KetQua { get; set; }

    [Required]
    [Display(Name = "Học kỳ")]
    public int HocKy { get; set; }

    [Required]
    [Display(Name = "Năm học")]
    public string NamHoc { get; set; }

   

}
