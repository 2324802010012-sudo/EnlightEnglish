using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnlightEnglishCenter.Migrations
{
    /// <inheritdoc />
    public partial class Fix_FK_DiemSo_To_HocVien : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DiemSo_NguoiDung_MaHocVien",
                table: "DiemSo");

            migrationBuilder.DropForeignKey(
                name: "FK_TestDauVao_NguoiDung_MaHocVien",
                table: "TestDauVao");

            migrationBuilder.AddForeignKey(
                name: "FK_DiemSo_HocVien_MaHocVien",
                table: "DiemSo",
                column: "MaHocVien",
                principalTable: "HocVien",
                principalColumn: "MaHocVien",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TestDauVao_NguoiDung_MaHocVien",
                table: "TestDauVao",
                column: "MaHocVien",
                principalTable: "NguoiDung",
                principalColumn: "MaNguoiDung",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DiemSo_HocVien_MaHocVien",
                table: "DiemSo");

            migrationBuilder.DropForeignKey(
                name: "FK_TestDauVao_NguoiDung_MaHocVien",
                table: "TestDauVao");

            migrationBuilder.AddForeignKey(
                name: "FK_DiemSo_NguoiDung_MaHocVien",
                table: "DiemSo",
                column: "MaHocVien",
                principalTable: "NguoiDung",
                principalColumn: "MaNguoiDung",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TestDauVao_NguoiDung_MaHocVien",
                table: "TestDauVao",
                column: "MaHocVien",
                principalTable: "NguoiDung",
                principalColumn: "MaNguoiDung",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
