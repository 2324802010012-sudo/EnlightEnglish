using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnlightEnglishCenter.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCascadeDelete_TestDauVao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestDauVao_NguoiDung_MaHocVien",
                table: "TestDauVao");

            migrationBuilder.AddForeignKey(
                name: "FK_TestDauVao_NguoiDung_MaHocVien",
                table: "TestDauVao",
                column: "MaHocVien",
                principalTable: "NguoiDung",
                principalColumn: "MaNguoiDung",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestDauVao_NguoiDung_MaHocVien",
                table: "TestDauVao");

            migrationBuilder.AddForeignKey(
                name: "FK_TestDauVao_NguoiDung_MaHocVien",
                table: "TestDauVao",
                column: "MaHocVien",
                principalTable: "NguoiDung",
                principalColumn: "MaNguoiDung",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
