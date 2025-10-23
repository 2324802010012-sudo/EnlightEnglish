using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnlightEnglishCenter.Migrations
{
    /// <inheritdoc />
    public partial class RecreateAll : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LopHoc_NguoiDung_MaGiaoVien",
                table: "LopHoc");

            migrationBuilder.DropForeignKey(
                name: "FK_LuongGiaoVien_NguoiDung_MaGiaoVien",
                table: "LuongGiaoVien");

            migrationBuilder.DropForeignKey(
                name: "FK_PhanCongGiangDays_GiaoViens_MaGiaoVien",
                table: "PhanCongGiangDays");

            migrationBuilder.DropForeignKey(
                name: "FK_PhanCongGiangDays_LopHoc_MaLop",
                table: "PhanCongGiangDays");

            migrationBuilder.DropForeignKey(
                name: "FK_TaiLieu_NguoiDung_MaGiaoVien",
                table: "TaiLieu");

            migrationBuilder.DropIndex(
                name: "IX_NguoiDung_Email",
                table: "NguoiDung");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PhanCongGiangDays",
                table: "PhanCongGiangDays");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GiaoViens",
                table: "GiaoViens");

            migrationBuilder.DropColumn(
                name: "LoTrinhHoc",
                table: "TestDauVao");

            migrationBuilder.DropColumn(
                name: "MoTa",
                table: "TaiLieu");

            migrationBuilder.DropColumn(
                name: "SiSoHienTai",
                table: "LopHoc");

            migrationBuilder.DropColumn(
                name: "SiSoToiDa",
                table: "LopHoc");

            migrationBuilder.DropColumn(
                name: "NoiDung",
                table: "LichHoc");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "GiaoViens");

            migrationBuilder.DropColumn(
                name: "GioiTinh",
                table: "GiaoViens");

            migrationBuilder.DropColumn(
                name: "HoTen",
                table: "GiaoViens");

            migrationBuilder.DropColumn(
                name: "NgaySinh",
                table: "GiaoViens");

            migrationBuilder.DropColumn(
                name: "SoDienThoai",
                table: "GiaoViens");

            migrationBuilder.RenameTable(
                name: "PhanCongGiangDays",
                newName: "PhanCongGiangDay");

            migrationBuilder.RenameTable(
                name: "GiaoViens",
                newName: "GiaoVien");

            migrationBuilder.RenameColumn(
                name: "MaLich",
                table: "LichHoc",
                newName: "MaLichHoc");

            migrationBuilder.RenameIndex(
                name: "IX_PhanCongGiangDays_MaLop",
                table: "PhanCongGiangDay",
                newName: "IX_PhanCongGiangDay_MaLop");

            migrationBuilder.RenameIndex(
                name: "IX_PhanCongGiangDays_MaGiaoVien",
                table: "PhanCongGiangDay",
                newName: "IX_PhanCongGiangDay_MaGiaoVien");

            migrationBuilder.AlterColumn<string>(
                name: "TrangThai",
                table: "TestDauVao",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<decimal>(
                name: "TongDiem",
                table: "TestDauVao",
                type: "decimal(5,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "MaHocVien",
                table: "TestDauVao",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "KhoaHocDeXuat",
                table: "TestDauVao",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "DiemViet",
                table: "TestDauVao",
                type: "decimal(5,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "DiemNguPhap",
                table: "TestDauVao",
                type: "decimal(5,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "DiemNghe",
                table: "TestDauVao",
                type: "decimal(5,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "DiemDoc",
                table: "TestDauVao",
                type: "decimal(5,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HocVienMaHocVien",
                table: "TestDauVao",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "KetQua",
                table: "TestDauVao",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TenTaiLieu",
                table: "TaiLieu",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "MaLop",
                table: "TaiLieu",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "MaGiaoVien",
                table: "TaiLieu",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "DuongDan",
                table: "TaiLieu",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LoaiTaiLieu",
                table: "TaiLieu",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NgayDang",
                table: "TaiLieu",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TrangThai",
                table: "LuongGiaoVien",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TrangThai",
                table: "LopHoc",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TrinhDo",
                table: "LopHoc",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DiaChiIP1",
                table: "LichSuTruyCap",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "GioKetThuc",
                table: "LichHoc",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(TimeOnly),
                oldType: "time",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "GioBatDau",
                table: "LichHoc",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(TimeOnly),
                oldType: "time",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaGiaoVien",
                table: "LichHoc",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhongHoc",
                table: "LichHoc",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "NgayPhanCong",
                table: "PhanCongGiangDay",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<int>(
                name: "MaLop",
                table: "PhanCongGiangDay",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "MaGiaoVien",
                table: "PhanCongGiangDay",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "GhiChu",
                table: "PhanCongGiangDay",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaNguoiDung",
                table: "GiaoVien",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PhanCongGiangDay",
                table: "PhanCongGiangDay",
                column: "MaPhanCong");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GiaoVien",
                table: "GiaoVien",
                column: "MaGiaoVien");

            migrationBuilder.CreateTable(
                name: "HocVien",
                columns: table => new
                {
                    MaHocVien = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HoTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SoDienThoai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    NgayDangKy = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HocVien", x => x.MaHocVien);
                });

            migrationBuilder.CreateTable(
                name: "NhanVienLeTan",
                columns: table => new
                {
                    MaLeTan = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaNguoiDung = table.Column<int>(type: "int", nullable: false),
                    CaLam = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    KinhNghiem = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NhanVienLeTan", x => x.MaLeTan);
                    table.ForeignKey(
                        name: "FK_NhanVienLeTan_NguoiDung_MaNguoiDung",
                        column: x => x.MaNguoiDung,
                        principalTable: "NguoiDung",
                        principalColumn: "MaNguoiDung",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PhongDaoTao",
                columns: table => new
                {
                    MaPhongDaoTao = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaNguoiDung = table.Column<int>(type: "int", nullable: false),
                    ChucVu = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhongDaoTao", x => x.MaPhongDaoTao);
                    table.ForeignKey(
                        name: "FK_PhongDaoTao_NguoiDung_MaNguoiDung",
                        column: x => x.MaNguoiDung,
                        principalTable: "NguoiDung",
                        principalColumn: "MaNguoiDung",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PhongHoc",
                columns: table => new
                {
                    MaPhongHoc = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenPhong = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ViTri = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhongHoc", x => x.MaPhongHoc);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TestDauVao_HocVienMaHocVien",
                table: "TestDauVao",
                column: "HocVienMaHocVien");

            migrationBuilder.CreateIndex(
                name: "IX_TestDauVao_KhoaHocDeXuat",
                table: "TestDauVao",
                column: "KhoaHocDeXuat");

            migrationBuilder.CreateIndex(
                name: "IX_LichHoc_MaGiaoVien",
                table: "LichHoc",
                column: "MaGiaoVien");

            migrationBuilder.CreateIndex(
                name: "IX_GiaoVien_MaNguoiDung",
                table: "GiaoVien",
                column: "MaNguoiDung");

            migrationBuilder.CreateIndex(
                name: "IX_NhanVienLeTan_MaNguoiDung",
                table: "NhanVienLeTan",
                column: "MaNguoiDung");

            migrationBuilder.CreateIndex(
                name: "IX_PhongDaoTao_MaNguoiDung",
                table: "PhongDaoTao",
                column: "MaNguoiDung");

            migrationBuilder.AddForeignKey(
                name: "FK_GiaoVien_NguoiDung_MaNguoiDung",
                table: "GiaoVien",
                column: "MaNguoiDung",
                principalTable: "NguoiDung",
                principalColumn: "MaNguoiDung");

            migrationBuilder.AddForeignKey(
                name: "FK_LichHoc_GiaoVien_MaGiaoVien",
                table: "LichHoc",
                column: "MaGiaoVien",
                principalTable: "GiaoVien",
                principalColumn: "MaGiaoVien");

            migrationBuilder.AddForeignKey(
                name: "FK_LopHoc_GiaoVien_MaGiaoVien",
                table: "LopHoc",
                column: "MaGiaoVien",
                principalTable: "GiaoVien",
                principalColumn: "MaGiaoVien");

            migrationBuilder.AddForeignKey(
                name: "FK_LuongGiaoVien_GiaoVien_MaGiaoVien",
                table: "LuongGiaoVien",
                column: "MaGiaoVien",
                principalTable: "GiaoVien",
                principalColumn: "MaGiaoVien");

            migrationBuilder.AddForeignKey(
                name: "FK_PhanCongGiangDay_GiaoVien_MaGiaoVien",
                table: "PhanCongGiangDay",
                column: "MaGiaoVien",
                principalTable: "GiaoVien",
                principalColumn: "MaGiaoVien");

            migrationBuilder.AddForeignKey(
                name: "FK_PhanCongGiangDay_LopHoc_MaLop",
                table: "PhanCongGiangDay",
                column: "MaLop",
                principalTable: "LopHoc",
                principalColumn: "MaLop");

            migrationBuilder.AddForeignKey(
                name: "FK_TaiLieu_GiaoVien_MaGiaoVien",
                table: "TaiLieu",
                column: "MaGiaoVien",
                principalTable: "GiaoVien",
                principalColumn: "MaGiaoVien",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TestDauVao_HocVien_HocVienMaHocVien",
                table: "TestDauVao",
                column: "HocVienMaHocVien",
                principalTable: "HocVien",
                principalColumn: "MaHocVien");

            migrationBuilder.AddForeignKey(
                name: "FK_TestDauVao_KhoaHoc_KhoaHocDeXuat",
                table: "TestDauVao",
                column: "KhoaHocDeXuat",
                principalTable: "KhoaHoc",
                principalColumn: "MaKhoaHoc");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GiaoVien_NguoiDung_MaNguoiDung",
                table: "GiaoVien");

            migrationBuilder.DropForeignKey(
                name: "FK_LichHoc_GiaoVien_MaGiaoVien",
                table: "LichHoc");

            migrationBuilder.DropForeignKey(
                name: "FK_LopHoc_GiaoVien_MaGiaoVien",
                table: "LopHoc");

            migrationBuilder.DropForeignKey(
                name: "FK_LuongGiaoVien_GiaoVien_MaGiaoVien",
                table: "LuongGiaoVien");

            migrationBuilder.DropForeignKey(
                name: "FK_PhanCongGiangDay_GiaoVien_MaGiaoVien",
                table: "PhanCongGiangDay");

            migrationBuilder.DropForeignKey(
                name: "FK_PhanCongGiangDay_LopHoc_MaLop",
                table: "PhanCongGiangDay");

            migrationBuilder.DropForeignKey(
                name: "FK_TaiLieu_GiaoVien_MaGiaoVien",
                table: "TaiLieu");

            migrationBuilder.DropForeignKey(
                name: "FK_TestDauVao_HocVien_HocVienMaHocVien",
                table: "TestDauVao");

            migrationBuilder.DropForeignKey(
                name: "FK_TestDauVao_KhoaHoc_KhoaHocDeXuat",
                table: "TestDauVao");

            migrationBuilder.DropTable(
                name: "HocVien");

            migrationBuilder.DropTable(
                name: "NhanVienLeTan");

            migrationBuilder.DropTable(
                name: "PhongDaoTao");

            migrationBuilder.DropTable(
                name: "PhongHoc");

            migrationBuilder.DropIndex(
                name: "IX_TestDauVao_HocVienMaHocVien",
                table: "TestDauVao");

            migrationBuilder.DropIndex(
                name: "IX_TestDauVao_KhoaHocDeXuat",
                table: "TestDauVao");

            migrationBuilder.DropIndex(
                name: "IX_LichHoc_MaGiaoVien",
                table: "LichHoc");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PhanCongGiangDay",
                table: "PhanCongGiangDay");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GiaoVien",
                table: "GiaoVien");

            migrationBuilder.DropIndex(
                name: "IX_GiaoVien_MaNguoiDung",
                table: "GiaoVien");

            migrationBuilder.DropColumn(
                name: "HocVienMaHocVien",
                table: "TestDauVao");

            migrationBuilder.DropColumn(
                name: "KetQua",
                table: "TestDauVao");

            migrationBuilder.DropColumn(
                name: "LoaiTaiLieu",
                table: "TaiLieu");

            migrationBuilder.DropColumn(
                name: "NgayDang",
                table: "TaiLieu");

            migrationBuilder.DropColumn(
                name: "TrangThai",
                table: "LuongGiaoVien");

            migrationBuilder.DropColumn(
                name: "TrinhDo",
                table: "LopHoc");

            migrationBuilder.DropColumn(
                name: "DiaChiIP1",
                table: "LichSuTruyCap");

            migrationBuilder.DropColumn(
                name: "MaGiaoVien",
                table: "LichHoc");

            migrationBuilder.DropColumn(
                name: "PhongHoc",
                table: "LichHoc");

            migrationBuilder.DropColumn(
                name: "MaNguoiDung",
                table: "GiaoVien");

            migrationBuilder.RenameTable(
                name: "PhanCongGiangDay",
                newName: "PhanCongGiangDays");

            migrationBuilder.RenameTable(
                name: "GiaoVien",
                newName: "GiaoViens");

            migrationBuilder.RenameColumn(
                name: "MaLichHoc",
                table: "LichHoc",
                newName: "MaLich");

            migrationBuilder.RenameIndex(
                name: "IX_PhanCongGiangDay_MaLop",
                table: "PhanCongGiangDays",
                newName: "IX_PhanCongGiangDays_MaLop");

            migrationBuilder.RenameIndex(
                name: "IX_PhanCongGiangDay_MaGiaoVien",
                table: "PhanCongGiangDays",
                newName: "IX_PhanCongGiangDays_MaGiaoVien");

            migrationBuilder.AlterColumn<string>(
                name: "TrangThai",
                table: "TestDauVao",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "TongDiem",
                table: "TestDauVao",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(5,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "MaHocVien",
                table: "TestDauVao",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "KhoaHocDeXuat",
                table: "TestDauVao",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "DiemViet",
                table: "TestDauVao",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(5,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "DiemNguPhap",
                table: "TestDauVao",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(5,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "DiemNghe",
                table: "TestDauVao",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(5,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "DiemDoc",
                table: "TestDauVao",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(5,2)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LoTrinhHoc",
                table: "TestDauVao",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TenTaiLieu",
                table: "TaiLieu",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "MaLop",
                table: "TaiLieu",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "MaGiaoVien",
                table: "TaiLieu",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DuongDan",
                table: "TaiLieu",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MoTa",
                table: "TaiLieu",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TrangThai",
                table: "LopHoc",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SiSoHienTai",
                table: "LopHoc",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SiSoToiDa",
                table: "LopHoc",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<TimeOnly>(
                name: "GioKetThuc",
                table: "LichHoc",
                type: "time",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.AlterColumn<TimeOnly>(
                name: "GioBatDau",
                table: "LichHoc",
                type: "time",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NoiDung",
                table: "LichHoc",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "NgayPhanCong",
                table: "PhanCongGiangDays",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "MaLop",
                table: "PhanCongGiangDays",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "MaGiaoVien",
                table: "PhanCongGiangDays",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "GhiChu",
                table: "PhanCongGiangDays",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "GiaoViens",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GioiTinh",
                table: "GiaoViens",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HoTen",
                table: "GiaoViens",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "NgaySinh",
                table: "GiaoViens",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SoDienThoai",
                table: "GiaoViens",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PhanCongGiangDays",
                table: "PhanCongGiangDays",
                column: "MaPhanCong");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GiaoViens",
                table: "GiaoViens",
                column: "MaGiaoVien");

            migrationBuilder.CreateIndex(
                name: "IX_NguoiDung_Email",
                table: "NguoiDung",
                column: "Email",
                unique: true,
                filter: "[Email] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_LopHoc_NguoiDung_MaGiaoVien",
                table: "LopHoc",
                column: "MaGiaoVien",
                principalTable: "NguoiDung",
                principalColumn: "MaNguoiDung");

            migrationBuilder.AddForeignKey(
                name: "FK_LuongGiaoVien_NguoiDung_MaGiaoVien",
                table: "LuongGiaoVien",
                column: "MaGiaoVien",
                principalTable: "NguoiDung",
                principalColumn: "MaNguoiDung");

            migrationBuilder.AddForeignKey(
                name: "FK_PhanCongGiangDays_GiaoViens_MaGiaoVien",
                table: "PhanCongGiangDays",
                column: "MaGiaoVien",
                principalTable: "GiaoViens",
                principalColumn: "MaGiaoVien",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PhanCongGiangDays_LopHoc_MaLop",
                table: "PhanCongGiangDays",
                column: "MaLop",
                principalTable: "LopHoc",
                principalColumn: "MaLop",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TaiLieu_NguoiDung_MaGiaoVien",
                table: "TaiLieu",
                column: "MaGiaoVien",
                principalTable: "NguoiDung",
                principalColumn: "MaNguoiDung",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
