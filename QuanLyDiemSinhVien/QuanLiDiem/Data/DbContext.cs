using Microsoft.EntityFrameworkCore;
using QuanLiDiem.Models;

namespace QuanLiDiem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<DanhSachSinhVien> DanhSachSinhVien { get; set; }
        public DbSet<RegistrationModel> DanhSachDK { get; set; }
        public DbSet<Diem> Diem { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Định nghĩa khóa chính cho bảng DanhSachSinhVien
            modelBuilder.Entity<DanhSachSinhVien>()
                .HasKey(sv => sv.MSSV); // Chỉ định MSSV là khóa chính
            // Thiết lập quan hệ giữa Diem và DanhSachSinhVien
            modelBuilder.Entity<Diem>()
            .HasOne(d => d.SinhVien)
            .WithMany(s => s.Diem)
            .HasForeignKey(d => d.MSSV)
            .OnDelete(DeleteBehavior.Cascade);
        }

    }
}
