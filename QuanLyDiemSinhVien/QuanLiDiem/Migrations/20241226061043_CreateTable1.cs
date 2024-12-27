using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLiDiem.Migrations
{
    /// <inheritdoc />
    public partial class CreateTable1 : Migration
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
                name: "Diem",
                columns: table => new
                {
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
                    table.PrimaryKey("PK_Diem", x => x.MSSV);
                    table.ForeignKey(
                        name: "FK_Diem_DanhSachSinhVien_MSSV",
                        column: x => x.MSSV,
                        principalTable: "DanhSachSinhVien",
                        principalColumn: "MSSV",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DanhSachDK");

            migrationBuilder.DropTable(
                name: "Diem");

            migrationBuilder.DropTable(
                name: "DanhSachSinhVien");
        }
    }
}
