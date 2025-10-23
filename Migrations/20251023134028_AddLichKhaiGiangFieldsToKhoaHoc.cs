using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnlightEnglishCenter.Migrations
{
    /// <inheritdoc />
    public partial class AddLichKhaiGiangFieldsToKhoaHoc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GiangVien",
                table: "KhoaHoc",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HinhThuc",
                table: "KhoaHoc",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NgayKhaiGiang",
                table: "KhoaHoc",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ThoiGianHoc",
                table: "KhoaHoc",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TrinhDo",
                table: "KhoaHoc",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GiangVien",
                table: "KhoaHoc");

            migrationBuilder.DropColumn(
                name: "HinhThuc",
                table: "KhoaHoc");

            migrationBuilder.DropColumn(
                name: "NgayKhaiGiang",
                table: "KhoaHoc");

            migrationBuilder.DropColumn(
                name: "ThoiGianHoc",
                table: "KhoaHoc");

            migrationBuilder.DropColumn(
                name: "TrinhDo",
                table: "KhoaHoc");
        }
    }
}
