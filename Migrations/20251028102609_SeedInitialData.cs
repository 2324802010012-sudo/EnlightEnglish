using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnlightEnglishCenter.Migrations
{
    /// <inheritdoc />
    public partial class SeedInitialData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ======================
            // Seed bảng VaiTro
            // ======================
            migrationBuilder.InsertData(
                table: "VaiTro",
                columns: new[] { "TenVaiTro" },
                values: new object[,]
                {
        { "Admin" },
        { "KeToan" },
        { "LeTan" },
        { "HocVien" },
        { "GiaoVien" },
        { "PhongDaoTao" }
                }
            );

            // ======================
            // Seed bảng NguoiDung
            // ======================
            migrationBuilder.InsertData(
                table: "NguoiDung",
                columns: new[] { "HoTen", "GioiTinh", "NgaySinh", "DiaChi", "SoDienThoai", "Email", "TenDangNhap", "MatKhau", "MaVaiTro", "SoLanSaiMatKhau", "TrangThai" },
                values: new object[,]
                {
        { "Trần Thị Diệu", "Nữ", new DateTime(1990,5,10), "TDM, Bình Dương", "0909123456", "admin@enlight.edu.vn", "admin", "123", 1, 0, "Hoạt động" },
        { "Phạm Thị Hân", "Nữ", new DateTime(1993,3,25), "TDM, Bình Dương", "0911222333", "ketoan@enlight.edu.vn", "ketoan", "123", 2, 0, "Hoạt động" },
        { "Nguyễn Thị Hồng Hân", "Nữ", new DateTime(1996,9,21), "TDM, Bình Dương", "0912555777", "letan@enlight.edu.vn", "letan", "123", 3, 0, "Hoạt động" },
        { "Phạm Trung Tín", "Nam", new DateTime(1988,11,2), "TDM, Bình Dương", "0909555111", "giaovien@enlight.edu.vn", "giaovien", "123", 5, 0, "Hoạt động" },
        { "Trần Hoài Tín", "Nam", new DateTime(1991,6,16), "TDM, Bình Dương", "0912888999", "phongdt@enlight.edu.vn", "phongdt", "123", 6, 0, "Hoạt động" }
                }
            );

            // ======================
            // Seed bảng KhoaHoc
            // ======================
            migrationBuilder.InsertData(
                table: "KhoaHoc",
                columns: new[] { "TenKhoaHoc", "MoTa", "CapDo", "ChuanDauRa", "TrangThai", "HocPhi", "ThoiLuong", "NgayBatDau", "NgayKetThuc" },
                values: new object[,]
                {
        { "IELTS", "Luyện 4 kỹ năng chuẩn quốc tế", "Cơ bản", "Đạt 4.0 - 5.0+", "Đang mở", 0m, "3 tháng", DateTime.Now.Date, DateTime.Now.AddMonths(3).Date },
        { "TOEIC", "Giao tiếp - thương mại", "Trung bình", "Đạt 600-900", "Đang mở", 0m, "3 tháng", DateTime.Now.Date, DateTime.Now.AddMonths(3).Date },
        { "CAMBRIDGE", "Thiếu nhi - giao tiếp cơ bản", "Cơ bản", "Starters - Movers - Flyers", "Đang mở", 0m, "3 tháng", DateTime.Now.Date, DateTime.Now.AddMonths(3).Date }
                }
            );

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM NguoiDung");
            migrationBuilder.Sql("DELETE FROM VaiTro");
            migrationBuilder.Sql("DELETE FROM KhoaHoc");

        }
    }
}
