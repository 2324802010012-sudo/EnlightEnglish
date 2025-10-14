using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnlightEnglishCenter.Migrations
{
    /// <inheritdoc />
    public partial class AddTable_DkHocVienLopHoc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DK_HocVien_LopHoc");

            migrationBuilder.AddColumn<DateTime>(
                name: "KhoaDenNgay",
                table: "NguoiDung",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResetToken",
                table: "NguoiDung",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ResetTokenExpiry",
                table: "NguoiDung",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SoLanSaiMatKhau",
                table: "NguoiDung",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "NgayDongCuoi",
                table: "HocPhi",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DkHocVienLopHoc",
                columns: table => new
                {
                    MaHocVien = table.Column<int>(type: "int", nullable: false),
                    MaLop = table.Column<int>(type: "int", nullable: false),
                    MaDangKy = table.Column<int>(type: "int", nullable: false),
                    NgayDangKy = table.Column<DateTime>(type: "date", nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DkHocVienLopHocMaHocVien = table.Column<int>(type: "int", nullable: true),
                    DkHocVienLopHocMaLop = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DkHocVienLopHoc", x => new { x.MaHocVien, x.MaLop });
                    table.ForeignKey(
                        name: "FK_DkHocVienLopHoc_DkHocVienLopHoc_DkHocVienLopHocMaHocVien_DkHocVienLopHocMaLop",
                        columns: x => new { x.DkHocVienLopHocMaHocVien, x.DkHocVienLopHocMaLop },
                        principalTable: "DkHocVienLopHoc",
                        principalColumns: new[] { "MaHocVien", "MaLop" });
                    table.ForeignKey(
                        name: "FK_DkHocVienLopHoc_LopHoc_MaLop",
                        column: x => x.MaLop,
                        principalTable: "LopHoc",
                        principalColumn: "MaLop",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DkHocVienLopHoc_NguoiDung_MaHocVien",
                        column: x => x.MaHocVien,
                        principalTable: "NguoiDung",
                        principalColumn: "MaNguoiDung",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DkHocVienLopHoc_DkHocVienLopHocMaHocVien_DkHocVienLopHocMaLop",
                table: "DkHocVienLopHoc",
                columns: new[] { "DkHocVienLopHocMaHocVien", "DkHocVienLopHocMaLop" });

            migrationBuilder.CreateIndex(
                name: "IX_DkHocVienLopHoc_MaLop",
                table: "DkHocVienLopHoc",
                column: "MaLop");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DkHocVienLopHoc");

            migrationBuilder.DropColumn(
                name: "KhoaDenNgay",
                table: "NguoiDung");

            migrationBuilder.DropColumn(
                name: "ResetToken",
                table: "NguoiDung");

            migrationBuilder.DropColumn(
                name: "ResetTokenExpiry",
                table: "NguoiDung");

            migrationBuilder.DropColumn(
                name: "SoLanSaiMatKhau",
                table: "NguoiDung");

            migrationBuilder.DropColumn(
                name: "NgayDongCuoi",
                table: "HocPhi");

            migrationBuilder.CreateTable(
                name: "DK_HocVien_LopHoc",
                columns: table => new
                {
                    MaHocVien = table.Column<int>(type: "int", nullable: false),
                    MaLop = table.Column<int>(type: "int", nullable: false),
                    NgayDangKy = table.Column<DateOnly>(type: "date", nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DK_HocVien_LopHoc", x => new { x.MaHocVien, x.MaLop });
                    table.ForeignKey(
                        name: "FK_DK_HocVien_LopHoc_LopHoc_MaLop",
                        column: x => x.MaLop,
                        principalTable: "LopHoc",
                        principalColumn: "MaLop",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DK_HocVien_LopHoc_NguoiDung_MaHocVien",
                        column: x => x.MaHocVien,
                        principalTable: "NguoiDung",
                        principalColumn: "MaNguoiDung",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DK_HocVien_LopHoc_MaLop",
                table: "DK_HocVien_LopHoc",
                column: "MaLop");
        }
    }
}
