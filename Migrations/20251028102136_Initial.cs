using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnlightEnglishCenter.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DangKyTuVan",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HoTen = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SoDienThoai = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DangKyTuVan", x => x.Id);
                });

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
                name: "KhoaHoc",
                columns: table => new
                {
                    MaKhoaHoc = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenKhoaHoc = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    HocPhi = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ThoiLuong = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NgayBatDau = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NgayKetThuc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ChuanDauRa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true, defaultValue: "Đang mở")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KhoaHoc", x => x.MaKhoaHoc);
                });

            migrationBuilder.CreateTable(
                name: "LienHeKhachHang",
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
                    table.PrimaryKey("PK_LienHeKhachHang", x => x.MaLienHe);
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

            migrationBuilder.CreateTable(
                name: "VaiTro",
                columns: table => new
                {
                    MaVaiTro = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenVaiTro = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VaiTro", x => x.MaVaiTro);
                });

            migrationBuilder.CreateTable(
                name: "NguoiDung",
                columns: table => new
                {
                    MaNguoiDung = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HoTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    GioiTinh = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    NgaySinh = table.Column<DateOnly>(type: "date", nullable: true),
                    DiaChi = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    SoDienThoai = table.Column<string>(type: "varchar(15)", unicode: false, maxLength: 15, nullable: true),
                    Email = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    TenDangNhap = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MatKhau = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    MaVaiTro = table.Column<int>(type: "int", nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    KhoaDenNgay = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ResetToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResetTokenExpiry = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SoLanSaiMatKhau = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NguoiDung", x => x.MaNguoiDung);
                    table.ForeignKey(
                        name: "FK_NguoiDung_VaiTro_MaVaiTro",
                        column: x => x.MaVaiTro,
                        principalTable: "VaiTro",
                        principalColumn: "MaVaiTro",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BaoCao",
                columns: table => new
                {
                    MaBaoCao = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoaiBaoCao = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    NoiDung = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NguoiLap = table.Column<int>(type: "int", nullable: true),
                    NgayLap = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaoCao", x => x.MaBaoCao);
                    table.ForeignKey(
                        name: "FK_BaoCao_NguoiDung_NguoiLap",
                        column: x => x.NguoiLap,
                        principalTable: "NguoiDung",
                        principalColumn: "MaNguoiDung",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GiaoVien",
                columns: table => new
                {
                    MaGiaoVien = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaNguoiDung = table.Column<int>(type: "int", nullable: true),
                    TrinhDo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    KinhNghiem = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ChuyenMon = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GiaoVien", x => x.MaGiaoVien);
                    table.ForeignKey(
                        name: "FK_GiaoVien_NguoiDung_MaNguoiDung",
                        column: x => x.MaNguoiDung,
                        principalTable: "NguoiDung",
                        principalColumn: "MaNguoiDung");
                });

            migrationBuilder.CreateTable(
                name: "LichSuTruyCap",
                columns: table => new
                {
                    MaLog = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaNguoiDung = table.Column<int>(type: "int", nullable: false),
                    HanhDong = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ThoiGian = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DiaChiIP = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DiaChiIP1 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LichSuTruyCap", x => x.MaLog);
                    table.ForeignKey(
                        name: "FK_LichSuTruyCap_NguoiDung_MaNguoiDung",
                        column: x => x.MaNguoiDung,
                        principalTable: "NguoiDung",
                        principalColumn: "MaNguoiDung",
                        onDelete: ReferentialAction.Cascade);
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
                name: "TestDauVao",
                columns: table => new
                {
                    MaTest = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaHocVien = table.Column<int>(type: "int", nullable: true),
                    NgayTest = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DiemNghe = table.Column<decimal>(type: "decimal(4,1)", nullable: true),
                    DiemDoc = table.Column<decimal>(type: "decimal(4,1)", nullable: true),
                    DiemViet = table.Column<decimal>(type: "decimal(4,1)", nullable: true),
                    DiemNguPhap = table.Column<decimal>(type: "decimal(4,1)", nullable: true),
                    TongDiem = table.Column<decimal>(type: "decimal(4,1)", nullable: true),
                    KhoaHocDeXuat = table.Column<int>(type: "int", nullable: true),
                    LoTrinhHoc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true, defaultValue: "Chờ xác nhận"),
                    HoTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    HocVienMaHocVien = table.Column<int>(type: "int", nullable: true),
                    NguoiDungMaNguoiDung = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestDauVao", x => x.MaTest);
                    table.ForeignKey(
                        name: "FK_TestDauVao_HocVien_HocVienMaHocVien",
                        column: x => x.HocVienMaHocVien,
                        principalTable: "HocVien",
                        principalColumn: "MaHocVien");
                    table.ForeignKey(
                        name: "FK_TestDauVao_KhoaHoc_KhoaHocDeXuat",
                        column: x => x.KhoaHocDeXuat,
                        principalTable: "KhoaHoc",
                        principalColumn: "MaKhoaHoc",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestDauVao_NguoiDung_MaHocVien",
                        column: x => x.MaHocVien,
                        principalTable: "NguoiDung",
                        principalColumn: "MaNguoiDung",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestDauVao_NguoiDung_NguoiDungMaNguoiDung",
                        column: x => x.NguoiDungMaNguoiDung,
                        principalTable: "NguoiDung",
                        principalColumn: "MaNguoiDung");
                });

            migrationBuilder.CreateTable(
                name: "LopHoc",
                columns: table => new
                {
                    MaLop = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenLop = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true, defaultValue: "Đang học"),
                    MaGiaoVien = table.Column<int>(type: "int", nullable: true),
                    MaKhoaHoc = table.Column<int>(type: "int", nullable: true),
                    SiSoHienTai = table.Column<int>(type: "int", nullable: true, defaultValue: 0),
                    SiSoToiDa = table.Column<int>(type: "int", nullable: true, defaultValue: 20)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LopHoc", x => x.MaLop);
                    table.ForeignKey(
                        name: "FK_LopHoc_GiaoVien_MaGiaoVien",
                        column: x => x.MaGiaoVien,
                        principalTable: "GiaoVien",
                        principalColumn: "MaGiaoVien");
                    table.ForeignKey(
                        name: "FK_LopHoc_KhoaHoc_MaKhoaHoc",
                        column: x => x.MaKhoaHoc,
                        principalTable: "KhoaHoc",
                        principalColumn: "MaKhoaHoc",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LuongGiaoVien",
                columns: table => new
                {
                    MaLuong = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaGiaoVien = table.Column<int>(type: "int", nullable: true),
                    Thang = table.Column<int>(type: "int", nullable: true),
                    Nam = table.Column<int>(type: "int", nullable: true),
                    SoBuoiDay = table.Column<int>(type: "int", nullable: true),
                    LuongMoiBuoi = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    TongLuong = table.Column<decimal>(type: "decimal(21,2)", nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LuongGiaoVien", x => x.MaLuong);
                    table.ForeignKey(
                        name: "FK_LuongGiaoVien_GiaoVien_MaGiaoVien",
                        column: x => x.MaGiaoVien,
                        principalTable: "GiaoVien",
                        principalColumn: "MaGiaoVien");
                });

            migrationBuilder.CreateTable(
                name: "DiemSo",
                columns: table => new
                {
                    MaDiem = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaHocVien = table.Column<int>(type: "int", nullable: false),
                    MaLop = table.Column<int>(type: "int", nullable: false),
                    DiemGiuaKy = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DiemCuoiKy = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    NhanXet = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiemSo", x => x.MaDiem);
                    table.ForeignKey(
                        name: "FK_DiemSo_LopHoc_MaLop",
                        column: x => x.MaLop,
                        principalTable: "LopHoc",
                        principalColumn: "MaLop",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DiemSo_NguoiDung_MaHocVien",
                        column: x => x.MaHocVien,
                        principalTable: "NguoiDung",
                        principalColumn: "MaNguoiDung",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DK_HocVien_LopHoc",
                columns: table => new
                {
                    MaHocVien = table.Column<int>(type: "int", nullable: false),
                    MaLop = table.Column<int>(type: "int", nullable: false),
                    NgayDangKy = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TrangThaiHoc = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
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

            migrationBuilder.CreateTable(
                name: "HocPhi",
                columns: table => new
                {
                    MaHocPhi = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaHocVien = table.Column<int>(type: "int", nullable: false),
                    MaLop = table.Column<int>(type: "int", nullable: false),
                    SoTienPhaiDong = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SoTienDaDong = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayDongCuoi = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HocPhi", x => x.MaHocPhi);
                    table.ForeignKey(
                        name: "FK_HocPhi_LopHoc_MaLop",
                        column: x => x.MaLop,
                        principalTable: "LopHoc",
                        principalColumn: "MaLop",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HocPhi_NguoiDung_MaHocVien",
                        column: x => x.MaHocVien,
                        principalTable: "NguoiDung",
                        principalColumn: "MaNguoiDung",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LichHoc",
                columns: table => new
                {
                    MaLich = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaLop = table.Column<int>(type: "int", nullable: true),
                    NgayHoc = table.Column<DateTime>(type: "date", nullable: true),
                    GioBatDau = table.Column<TimeSpan>(type: "time", nullable: true),
                    GioKetThuc = table.Column<TimeSpan>(type: "time", nullable: true),
                    NoiDung = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    PhongHoc = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LichHoc", x => x.MaLich);
                    table.ForeignKey(
                        name: "FK_LichHoc_LopHoc_MaLop",
                        column: x => x.MaLop,
                        principalTable: "LopHoc",
                        principalColumn: "MaLop");
                });

            migrationBuilder.CreateTable(
                name: "LichThi",
                columns: table => new
                {
                    MaThi = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaLop = table.Column<int>(type: "int", nullable: true),
                    NgayThi = table.Column<DateOnly>(type: "date", nullable: true),
                    GioThi = table.Column<TimeOnly>(type: "time", nullable: true),
                    LoaiThi = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DiaDiem = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LichThi", x => x.MaThi);
                    table.ForeignKey(
                        name: "FK_LichThi_LopHoc_MaLop",
                        column: x => x.MaLop,
                        principalTable: "LopHoc",
                        principalColumn: "MaLop");
                });

            migrationBuilder.CreateTable(
                name: "PhanCongGiangDay",
                columns: table => new
                {
                    MaPhanCong = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaGiaoVien = table.Column<int>(type: "int", nullable: true),
                    MaLop = table.Column<int>(type: "int", nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    NgayPhanCong = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhanCongGiangDay", x => x.MaPhanCong);
                    table.ForeignKey(
                        name: "FK_PhanCongGiangDay_GiaoVien_MaGiaoVien",
                        column: x => x.MaGiaoVien,
                        principalTable: "GiaoVien",
                        principalColumn: "MaGiaoVien");
                    table.ForeignKey(
                        name: "FK_PhanCongGiangDay_LopHoc_MaLop",
                        column: x => x.MaLop,
                        principalTable: "LopHoc",
                        principalColumn: "MaLop");
                });

            migrationBuilder.CreateTable(
                name: "TaiLieu",
                columns: table => new
                {
                    MaTaiLieu = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaLop = table.Column<int>(type: "int", nullable: true),
                    MaGiaoVien = table.Column<int>(type: "int", nullable: true),
                    TenTaiLieu = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    MoTa = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    DuongDan = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    NgayTaiLen = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaiLieu", x => x.MaTaiLieu);
                    table.ForeignKey(
                        name: "FK_TaiLieu_GiaoVien_MaGiaoVien",
                        column: x => x.MaGiaoVien,
                        principalTable: "GiaoVien",
                        principalColumn: "MaGiaoVien",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TaiLieu_LopHoc_MaLop",
                        column: x => x.MaLop,
                        principalTable: "LopHoc",
                        principalColumn: "MaLop");
                });

            migrationBuilder.CreateTable(
                name: "DiemDanh",
                columns: table => new
                {
                    MaHocVien = table.Column<int>(type: "int", nullable: false),
                    MaLich = table.Column<int>(type: "int", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MaHocVienNavigationMaNguoiDung = table.Column<int>(type: "int", nullable: true),
                    MaLichNavigationMaLich = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiemDanh", x => new { x.MaHocVien, x.MaLich });
                    table.ForeignKey(
                        name: "FK_DiemDanh_LichHoc_MaLichNavigationMaLich",
                        column: x => x.MaLichNavigationMaLich,
                        principalTable: "LichHoc",
                        principalColumn: "MaLich");
                    table.ForeignKey(
                        name: "FK_DiemDanh_NguoiDung_MaHocVienNavigationMaNguoiDung",
                        column: x => x.MaHocVienNavigationMaNguoiDung,
                        principalTable: "NguoiDung",
                        principalColumn: "MaNguoiDung");
                });

            migrationBuilder.CreateIndex(
                name: "IX_BaoCao_NguoiLap",
                table: "BaoCao",
                column: "NguoiLap");

            migrationBuilder.CreateIndex(
                name: "IX_DiemDanh_MaHocVienNavigationMaNguoiDung",
                table: "DiemDanh",
                column: "MaHocVienNavigationMaNguoiDung");

            migrationBuilder.CreateIndex(
                name: "IX_DiemDanh_MaLichNavigationMaLich",
                table: "DiemDanh",
                column: "MaLichNavigationMaLich");

            migrationBuilder.CreateIndex(
                name: "IX_DiemSo_MaHocVien",
                table: "DiemSo",
                column: "MaHocVien");

            migrationBuilder.CreateIndex(
                name: "IX_DiemSo_MaLop",
                table: "DiemSo",
                column: "MaLop");

            migrationBuilder.CreateIndex(
                name: "IX_DK_HocVien_LopHoc_MaLop",
                table: "DK_HocVien_LopHoc",
                column: "MaLop");

            migrationBuilder.CreateIndex(
                name: "IX_GiaoVien_MaNguoiDung",
                table: "GiaoVien",
                column: "MaNguoiDung");

            migrationBuilder.CreateIndex(
                name: "IX_HocPhi_MaHocVien",
                table: "HocPhi",
                column: "MaHocVien");

            migrationBuilder.CreateIndex(
                name: "IX_HocPhi_MaLop",
                table: "HocPhi",
                column: "MaLop");

            migrationBuilder.CreateIndex(
                name: "IX_LichHoc_MaLop",
                table: "LichHoc",
                column: "MaLop");

            migrationBuilder.CreateIndex(
                name: "IX_LichSuTruyCap_MaNguoiDung",
                table: "LichSuTruyCap",
                column: "MaNguoiDung");

            migrationBuilder.CreateIndex(
                name: "IX_LichThi_MaLop",
                table: "LichThi",
                column: "MaLop");

            migrationBuilder.CreateIndex(
                name: "IX_LopHoc_MaGiaoVien",
                table: "LopHoc",
                column: "MaGiaoVien");

            migrationBuilder.CreateIndex(
                name: "IX_LopHoc_MaKhoaHoc",
                table: "LopHoc",
                column: "MaKhoaHoc");

            migrationBuilder.CreateIndex(
                name: "IX_LuongGiaoVien_MaGiaoVien",
                table: "LuongGiaoVien",
                column: "MaGiaoVien");

            migrationBuilder.CreateIndex(
                name: "IX_NguoiDung_MaVaiTro",
                table: "NguoiDung",
                column: "MaVaiTro");

            migrationBuilder.CreateIndex(
                name: "IX_NguoiDung_TenDangNhap",
                table: "NguoiDung",
                column: "TenDangNhap",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NhanVienLeTan_MaNguoiDung",
                table: "NhanVienLeTan",
                column: "MaNguoiDung");

            migrationBuilder.CreateIndex(
                name: "IX_PhanCongGiangDay_MaGiaoVien",
                table: "PhanCongGiangDay",
                column: "MaGiaoVien");

            migrationBuilder.CreateIndex(
                name: "IX_PhanCongGiangDay_MaLop",
                table: "PhanCongGiangDay",
                column: "MaLop");

            migrationBuilder.CreateIndex(
                name: "IX_PhongDaoTao_MaNguoiDung",
                table: "PhongDaoTao",
                column: "MaNguoiDung");

            migrationBuilder.CreateIndex(
                name: "IX_TaiLieu_MaGiaoVien",
                table: "TaiLieu",
                column: "MaGiaoVien");

            migrationBuilder.CreateIndex(
                name: "IX_TaiLieu_MaLop",
                table: "TaiLieu",
                column: "MaLop");

            migrationBuilder.CreateIndex(
                name: "IX_TestDauVao_HocVienMaHocVien",
                table: "TestDauVao",
                column: "HocVienMaHocVien");

            migrationBuilder.CreateIndex(
                name: "IX_TestDauVao_KhoaHocDeXuat",
                table: "TestDauVao",
                column: "KhoaHocDeXuat");

            migrationBuilder.CreateIndex(
                name: "IX_TestDauVao_MaHocVien",
                table: "TestDauVao",
                column: "MaHocVien");

            migrationBuilder.CreateIndex(
                name: "IX_TestDauVao_NguoiDungMaNguoiDung",
                table: "TestDauVao",
                column: "NguoiDungMaNguoiDung");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BaoCao");

            migrationBuilder.DropTable(
                name: "DangKyTuVan");

            migrationBuilder.DropTable(
                name: "DiemDanh");

            migrationBuilder.DropTable(
                name: "DiemSo");

            migrationBuilder.DropTable(
                name: "DK_HocVien_LopHoc");

            migrationBuilder.DropTable(
                name: "HocPhi");

            migrationBuilder.DropTable(
                name: "LichSuTruyCap");

            migrationBuilder.DropTable(
                name: "LichThi");

            migrationBuilder.DropTable(
                name: "LienHeKhachHang");

            migrationBuilder.DropTable(
                name: "LuongGiaoVien");

            migrationBuilder.DropTable(
                name: "NhanVienLeTan");

            migrationBuilder.DropTable(
                name: "PhanCongGiangDay");

            migrationBuilder.DropTable(
                name: "PhongDaoTao");

            migrationBuilder.DropTable(
                name: "PhongHoc");

            migrationBuilder.DropTable(
                name: "TaiLieu");

            migrationBuilder.DropTable(
                name: "TestDauVao");

            migrationBuilder.DropTable(
                name: "LichHoc");

            migrationBuilder.DropTable(
                name: "HocVien");

            migrationBuilder.DropTable(
                name: "LopHoc");

            migrationBuilder.DropTable(
                name: "GiaoVien");

            migrationBuilder.DropTable(
                name: "KhoaHoc");

            migrationBuilder.DropTable(
                name: "NguoiDung");

            migrationBuilder.DropTable(
                name: "VaiTro");
        }
    }
}
