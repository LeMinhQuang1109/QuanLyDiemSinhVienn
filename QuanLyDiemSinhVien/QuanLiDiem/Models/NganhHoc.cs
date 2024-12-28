using System.ComponentModel.DataAnnotations;

namespace QuanLiDiem.Models
{
    public class NganhHoc
    {
         [Key]
         public string MaNganh { get; set; }

         [Required]
         public string TenNganh { get; set; }
    }

}

