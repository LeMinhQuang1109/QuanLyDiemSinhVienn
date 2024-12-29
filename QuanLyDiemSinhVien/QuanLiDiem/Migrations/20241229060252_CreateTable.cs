using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLiDiem.Migrations
{
    /// <inheritdoc />
    public partial class CreateTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DanhSachDK",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HoTen = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CCCD = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SDT = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DiaChi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NganhHoc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DTB1 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DTB2 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DTB3 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    XepLoai = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DanhSachDK", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DanhSachSinhVien",
                columns: table => new
                {
                    MSSV = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    HoTen = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GioiTinh = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CanCuocCongDan = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SoDienThoai = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DiaChi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaNganh = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TenTaiKhoan = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MatKhau = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VaiTro = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DanhSachSinhVien", x => x.MSSV);
                });

            migrationBuilder.CreateTable(
                name: "GiangVien",
                columns: table => new
                {
                    MaGV = table.Column<string>(type: "char(10)", unicode: false, fixedLength: true, maxLength: 10, nullable: false),
                    TenGV = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SoDienThoai = table.Column<string>(type: "char(10)", unicode: false, fixedLength: true, maxLength: 10, nullable: true),
                    DiaChi = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__GiangVie__2725AEF38D82FDC2", x => x.MaGV);
                });

            migrationBuilder.CreateTable(
                name: "Diem",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MSSV = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MaHP = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SoTinChi = table.Column<int>(type: "int", nullable: false),
                    DiemQuaTrinh = table.Column<double>(type: "float", nullable: false),
                    DiemCuoiKy = table.Column<double>(type: "float", nullable: false),
                    Diem10 = table.Column<double>(type: "float", nullable: false),
                    Diem4 = table.Column<double>(type: "float", nullable: false),
                    KetQua = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HocKy = table.Column<int>(type: "int", nullable: false),
                    NamHoc = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Diem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Diem_DanhSachSinhVien_MSSV",
                        column: x => x.MSSV,
                        principalTable: "DanhSachSinhVien",
                        principalColumn: "MSSV",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LopHocPhan",
                columns: table => new
                {
                    MaHP = table.Column<string>(type: "char(10)", unicode: false, fixedLength: true, maxLength: 10, nullable: false),
                    TenHP = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MaGV = table.Column<string>(type: "char(10)", unicode: false, fixedLength: true, maxLength: 10, nullable: true),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NgayBatDau = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NgayKetThuc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__LopHocPh__2725A6EC1EE62B30", x => x.MaHP);
                    table.ForeignKey(
                        name: "FK__LopHocPhan__MaGV__5165187F",
                        column: x => x.MaGV,
                        principalTable: "GiangVien",
                        principalColumn: "MaGV");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Diem_MSSV",
                table: "Diem",
                column: "MSSV");

            migrationBuilder.CreateIndex(
                name: "IX_LopHocPhan_MaGV",
                table: "LopHocPhan",
                column: "MaGV");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DanhSachDK");

            migrationBuilder.DropTable(
                name: "Diem");

            migrationBuilder.DropTable(
                name: "LopHocPhan");

            migrationBuilder.DropTable(
                name: "DanhSachSinhVien");

            migrationBuilder.DropTable(
                name: "GiangVien");
        }
    }
}
