using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnlightEnglishCenter.Migrations
{
    /// <inheritdoc />
    public partial class Fix_HocVienNguoiDung : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MaNguoiDung",
                table: "HocVien",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_HocVien_MaNguoiDung",
                table: "HocVien",
                column: "MaNguoiDung",
                unique: true,
                filter: "[MaNguoiDung] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_HocVien_NguoiDung_MaNguoiDung",
                table: "HocVien",
                column: "MaNguoiDung",
                principalTable: "NguoiDung",
                principalColumn: "MaNguoiDung");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HocVien_NguoiDung_MaNguoiDung",
                table: "HocVien");

            migrationBuilder.DropIndex(
                name: "IX_HocVien_MaNguoiDung",
                table: "HocVien");

            migrationBuilder.DropColumn(
                name: "MaNguoiDung",
                table: "HocVien");
        }
    }
}
