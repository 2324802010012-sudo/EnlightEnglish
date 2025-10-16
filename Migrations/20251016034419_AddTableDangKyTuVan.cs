using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnlightEnglishCenter.Migrations
{
    /// <inheritdoc />
    public partial class AddTableDangKyTuVan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DkHocVienLopHoc_DkHocVienLopHoc_DkHocVienLopHocMaHocVien_DkHocVienLopHocMaLop",
                table: "DkHocVienLopHoc");

            migrationBuilder.DropForeignKey(
                name: "FK_DkHocVienLopHoc_LopHoc_MaLop",
                table: "DkHocVienLopHoc");

            migrationBuilder.DropForeignKey(
                name: "FK_DkHocVienLopHoc_NguoiDung_MaHocVien",
                table: "DkHocVienLopHoc");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DkHocVienLopHoc",
                table: "DkHocVienLopHoc");

            migrationBuilder.DropIndex(
                name: "IX_DkHocVienLopHoc_DkHocVienLopHocMaHocVien_DkHocVienLopHocMaLop",
                table: "DkHocVienLopHoc");

            migrationBuilder.DropColumn(
                name: "DkHocVienLopHocMaHocVien",
                table: "DkHocVienLopHoc");

            migrationBuilder.DropColumn(
                name: "DkHocVienLopHocMaLop",
                table: "DkHocVienLopHoc");

            migrationBuilder.DropColumn(
                name: "MaDangKy",
                table: "DkHocVienLopHoc");

            migrationBuilder.RenameTable(
                name: "DkHocVienLopHoc",
                newName: "DK_HocVien_LopHoc");

            migrationBuilder.RenameIndex(
                name: "IX_DkHocVienLopHoc_MaLop",
                table: "DK_HocVien_LopHoc",
                newName: "IX_DK_HocVien_LopHoc_MaLop");

            migrationBuilder.AlterColumn<DateTime>(
                name: "NgayDangKy",
                table: "DK_HocVien_LopHoc",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "date",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_DK_HocVien_LopHoc",
                table: "DK_HocVien_LopHoc",
                columns: new[] { "MaHocVien", "MaLop" });

            migrationBuilder.CreateTable(
                name: "DangKyTuVans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HoTen = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SoDienThoai = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DangKyTuVans", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_DK_HocVien_LopHoc_LopHoc_MaLop",
                table: "DK_HocVien_LopHoc",
                column: "MaLop",
                principalTable: "LopHoc",
                principalColumn: "MaLop",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DK_HocVien_LopHoc_NguoiDung_MaHocVien",
                table: "DK_HocVien_LopHoc",
                column: "MaHocVien",
                principalTable: "NguoiDung",
                principalColumn: "MaNguoiDung",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DK_HocVien_LopHoc_LopHoc_MaLop",
                table: "DK_HocVien_LopHoc");

            migrationBuilder.DropForeignKey(
                name: "FK_DK_HocVien_LopHoc_NguoiDung_MaHocVien",
                table: "DK_HocVien_LopHoc");

            migrationBuilder.DropTable(
                name: "DangKyTuVans");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DK_HocVien_LopHoc",
                table: "DK_HocVien_LopHoc");

            migrationBuilder.RenameTable(
                name: "DK_HocVien_LopHoc",
                newName: "DkHocVienLopHoc");

            migrationBuilder.RenameIndex(
                name: "IX_DK_HocVien_LopHoc_MaLop",
                table: "DkHocVienLopHoc",
                newName: "IX_DkHocVienLopHoc_MaLop");

            migrationBuilder.AlterColumn<DateTime>(
                name: "NgayDangKy",
                table: "DkHocVienLopHoc",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DkHocVienLopHocMaHocVien",
                table: "DkHocVienLopHoc",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DkHocVienLopHocMaLop",
                table: "DkHocVienLopHoc",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaDangKy",
                table: "DkHocVienLopHoc",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_DkHocVienLopHoc",
                table: "DkHocVienLopHoc",
                columns: new[] { "MaHocVien", "MaLop" });

            migrationBuilder.CreateIndex(
                name: "IX_DkHocVienLopHoc_DkHocVienLopHocMaHocVien_DkHocVienLopHocMaLop",
                table: "DkHocVienLopHoc",
                columns: new[] { "DkHocVienLopHocMaHocVien", "DkHocVienLopHocMaLop" });

            migrationBuilder.AddForeignKey(
                name: "FK_DkHocVienLopHoc_DkHocVienLopHoc_DkHocVienLopHocMaHocVien_DkHocVienLopHocMaLop",
                table: "DkHocVienLopHoc",
                columns: new[] { "DkHocVienLopHocMaHocVien", "DkHocVienLopHocMaLop" },
                principalTable: "DkHocVienLopHoc",
                principalColumns: new[] { "MaHocVien", "MaLop" });

            migrationBuilder.AddForeignKey(
                name: "FK_DkHocVienLopHoc_LopHoc_MaLop",
                table: "DkHocVienLopHoc",
                column: "MaLop",
                principalTable: "LopHoc",
                principalColumn: "MaLop",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DkHocVienLopHoc_NguoiDung_MaHocVien",
                table: "DkHocVienLopHoc",
                column: "MaHocVien",
                principalTable: "NguoiDung",
                principalColumn: "MaNguoiDung",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
