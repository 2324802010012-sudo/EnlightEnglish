using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EnlightEnglishCenter.Models
{
    [PrimaryKey(nameof(MaHocVien), nameof(MaLop))]
    [Table("DK_HocVien_LopHoc")]
    [Index(nameof(MaLop), Name = "IX_DK_HocVien_LopHoc_MaLop")]
    public class DkHocVienLopHoc
    {
        // 🔹 Khóa chính kép
        [Key, Column(Order = 0)]
        public int MaHocVien { get; set; }

        [Key, Column(Order = 1)]
        public int MaLop { get; set; }

        // 🔹 Thông tin đăng ký
        public DateTime? NgayDangKy { get; set; } = DateTime.Now;

        [StringLength(50)]
        public string? TrangThai { get; set; } = "Chưa thanh toán";
        // hoặc Đã thanh toán

        [StringLength(50)]
        public string? TrangThaiHoc { get; set; } = "Chưa bắt đầu"; // hoặc Đang học / Đã hoàn thành

        // 🔹 Liên kết
        [ForeignKey(nameof(MaHocVien))]
        [InverseProperty(nameof(NguoiDung.DkHocVienLopHocs))]
        public virtual NguoiDung MaHocVienNavigation { get; set; } = null!;

        [ForeignKey(nameof(MaLop))]
        [InverseProperty(nameof(LopHoc.DkHocVienLopHocs))]
        public virtual LopHoc MaLopNavigation { get; set; } = null!;
    }
}
