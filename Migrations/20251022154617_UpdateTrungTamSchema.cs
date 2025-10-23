using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnlightEnglishCenter.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTrungTamSchema : Migration
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_DangKyTuVan",
                table: "DangKyTuVan");

            migrationBuilder.DropColumn(
                name: "LoTrinhHoc",
                table: "KhoaHoc");

            migrationBuilder.RenameTable(
                name: "DangKyTuVan",
                newName: "DangKyTuVans");

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
