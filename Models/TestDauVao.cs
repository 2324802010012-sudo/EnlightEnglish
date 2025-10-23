using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EnlightEnglishCenter.Models
{
    [Table("TestDauVao")]
    [Index("MaHocVien", Name = "IX_TestDauVao_MaHocVien")]
    [Index("KhoaHocDeXuat", Name = "IX_TestDauVao_KhoaHocDeXuat")]
    public class TestDauVao
    {
        [Key]
        public int MaTest { get; set; }

        // 🔹 Học viên làm bài test (liên kết với bảng NguoiDung)
        public int? MaHocVien { get; set; }

        [ForeignKey("MaHocVien")]
        [InverseProperty("TestDauVaos")]
        public virtual NguoiDung? HocVien { get; set; }   // Navigation tới học viên

        // =======================
        // 🎯 Chi tiết điểm từng kỹ năng
        // =======================
        [Column(TypeName = "decimal(5,2)")]
        public decimal? DiemNghe { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal? DiemDoc { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal? DiemViet { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal? DiemNguPhap { get; set; }

        // 🔹 Tổng điểm tính trung bình hoặc quy đổi
        [Column(TypeName = "decimal(5,2)")]
        public decimal? TongDiem { get; set; }

        // 🔹 Ngày test thực hiện
        [DataType(DataType.Date)]
        public DateTime? NgayTest { get; set; } = DateTime.Now;

        // 🔹 Kết quả đánh giá tổng quát (Đạt / Không đạt)
        [StringLength(50)]
        public string? KetQua { get; set; }

        // 🔹 Trạng thái bài test
        [StringLength(50)]
        public string? TrangThai { get; set; } = "Chờ duyệt";
        // (Chờ duyệt / Được phép test / Hoàn thành / Từ chối)

        // =======================
        // 🧠 Thông tin bổ sung
        // =======================
        public double? DiemSo { get; set; }   // Tổng điểm quy đổi (0–100)

        [StringLength(100)]
        public string? LopDeXuat { get; set; }  // Gợi ý lớp phù hợp (IELTS cơ bản / nâng cao)

        [StringLength(255)]
        public string? LoTrinhHoc { get; set; } // Gợi ý lộ trình học theo kết quả

        // =======================
        // 📘 Khóa học được đề xuất (FK đến KhoaHoc)
        // =======================
        public int? KhoaHocDeXuat { get; set; }

        [ForeignKey("KhoaHocDeXuat")]
        [InverseProperty("TestDauVaos")]
        public virtual KhoaHoc? KhoaHocDeXuatNavigation { get; set; }

        // =======================
        // 🕓 Ngày đăng ký test đầu vào
        // =======================
        public DateTime NgayDangKy { get; set; } = DateTime.Now;

        // 🔹 Học viên đã làm bài test chưa
        public bool DaLamBaiTest { get; set; } = false;
        [NotMapped]
        public KhoaHoc? KhoaHoc => KhoaHocDeXuatNavigation;
        [NotMapped]
        public int? MaKhoaHoc => KhoaHocDeXuat;

    }
}
