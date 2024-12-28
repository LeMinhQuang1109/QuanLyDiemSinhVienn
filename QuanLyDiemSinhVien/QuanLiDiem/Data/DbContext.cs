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
        public DbSet<GiangVien> GiangViens { get; set; }
        public DbSet<LopHocPhan> LopHocPhans { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Định nghĩa khóa chính cho bảng DanhSachSinhVien
            modelBuilder.Entity<DanhSachSinhVien>()
                .HasKey(sv => sv.MSSV); // Chỉ định MSSV là khóa chính
            // Thiết lập quan hệ giữa Diem và DanhSachSinhVien
            modelBuilder.Entity<Diem>()
            .HasOne(d => d.SinhVien)
            .WithMany(s => s.Diems)
            .HasForeignKey(d => d.MSSV)
            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<GiangVien>(entity =>
            {
                entity.HasKey(e => e.MaGV).HasName("PK__GiangVie__2725AEF38D82FDC2");

                entity.ToTable("GiangVien");

                entity.Property(e => e.MaGV)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .IsFixedLength()
                    .HasColumnName("MaGV");
                entity.Property(e => e.DiaChi).HasMaxLength(200);
                entity.Property(e => e.Email).HasMaxLength(100);
                entity.Property(e => e.SoDienThoai)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .IsFixedLength();
                entity.Property(e => e.TenGV)
                    .HasMaxLength(100)
                    .HasColumnName("TenGV");
            });

            modelBuilder.Entity<LopHocPhan>(entity =>
            {
                entity.HasKey(e => e.MaHP).HasName("PK__LopHocPh__2725A6EC1EE62B30");

                entity.ToTable("LopHocPhan");

                entity.Property(e => e.MaHP)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .IsFixedLength()
                    .HasColumnName("MaHP");
                entity.Property(e => e.GhiChu).HasMaxLength(200);
                entity.Property(e => e.MaGV)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .IsFixedLength()
                    .HasColumnName("MaGV");
                entity.Property(e => e.TenHP)
                    .HasMaxLength(100)
                    .HasColumnName("TenHP");

                entity.HasOne(d => d.MaGVNavigation).WithMany(p => p.LopHocPhans)
                    .HasForeignKey(d => d.MaGV)
                    .HasConstraintName("FK__LopHocPhan__MaGV__5165187F");
            });
        }

    }
}
