using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnlightEnglishCenter.Migrations
{
    /// <inheritdoc />
    public partial class AddLienHeKhachHangTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_DangKyTuVans",
                table: "DangKyTuVans");

            migrationBuilder.RenameTable(
                name: "DangKyTuVans",
                newName: "DangKyTuVan");

            migrationBuilder.AddColumn<string>(
                name: "LoTrinhHoc",
                table: "KhoaHoc",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "MaLop",
                table: "DK_HocVien_LopHoc",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("Relational:ColumnOrder", 1);

            migrationBuilder.AlterColumn<int>(
                name: "MaHocVien",
                table: "DK_HocVien_LopHoc",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("Relational:ColumnOrder", 0);

            migrationBuilder.AlterColumn<string>(
                name: "SoDienThoai",
                table: "DangKyTuVan",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "HoTen",
                table: "DangKyTuVan",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "DangKyTuVan",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DangKyTuVan",
                table: "DangKyTuVan",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "HocVien",
                columns: table => new
                {
                    MaHocVien = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HoTen = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DienThoai = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DiaChi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GioiTinh = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgaySinh = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TrinhDo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KhoaHoc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LopHoc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayBatDauHoc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TinhTrangHoc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayDangKy = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HocVien", x => x.MaHocVien);
                });

            migrationBuilder.CreateTable(
                name: "LienHeKhachHangs",
                columns: table => new
                {
                    MaLienHe = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HoTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DienThoai = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    KhoaHoc = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    NgayLienHe = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LienHeKhachHangs", x => x.MaLienHe);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HocVien");

            migrationBuilder.DropTable(
                name: "LienHeKhachHangs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DangKyTuVan",
                table: "DangKyTuVan");

            migrationBuilder.DropColumn(
                name: "LoTrinhHoc",
                table: "KhoaHoc");

            migrationBuilder.RenameTable(
                name: "DangKyTuVan",
                newName: "DangKyTuVans");

            migrationBuilder.AlterColumn<int>(
                name: "MaLop",
                table: "DK_HocVien_LopHoc",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("Relational:ColumnOrder", 1);

            migrationBuilder.AlterColumn<int>(
                name: "MaHocVien",
                table: "DK_HocVien_LopHoc",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("Relational:ColumnOrder", 0);

            migrationBuilder.AlterColumn<string>(
                name: "SoDienThoai",
                table: "DangKyTuVans",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "HoTen",
                table: "DangKyTuVans",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "DangKyTuVans",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_DangKyTuVans",
                table: "DangKyTuVans",
                column: "Id");
        }
    }
}
