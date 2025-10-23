using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnlightEnglishCenter.Migrations
{
    /// <inheritdoc />
    public partial class Update_TestDauVao_FullModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "DaLamBaiTest",
                table: "TestDauVao",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<double>(
                name: "DiemSo",
                table: "TestDauVao",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LopDeXuat",
                table: "TestDauVao",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NgayDangKy",
                table: "TestDauVao",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DaLamBaiTest",
                table: "TestDauVao");

            migrationBuilder.DropColumn(
                name: "DiemSo",
                table: "TestDauVao");

            migrationBuilder.DropColumn(
                name: "LopDeXuat",
                table: "TestDauVao");

            migrationBuilder.DropColumn(
                name: "NgayDangKy",
                table: "TestDauVao");
        }
    }
}
