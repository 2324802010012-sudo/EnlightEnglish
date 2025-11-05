using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnlightEnglishCenter.Models
{
    [Table("KhoaHoc")]
    public class KhoaHoc
    {
        [Key]
        public int MaKhoaHoc { get; set; }

        [Required, StringLength(100)]
        public string TenKhoaHoc { get; set; } = string.Empty;

        [StringLength(255)]
        public string? MoTa { get; set; }
        public int ThoiLuongTuan { get; set; }
        public decimal? HocPhi { get; set; }

        [StringLength(50)]
        public string? ThoiLuong { get; set; }

        [StringLength(50)]
        public string? CapDo { get; set; }

        public DateTime? NgayBatDau { get; set; }
        public DateTime? NgayKetThuc { get; set; }

        public string? ChuanDauRa { get; set; }
        [StringLength(10)]
        public string? LichHoc { get; set; }

        [StringLength(20)]
        public string? LoaiKhoaHoc { get; set; }  // Test / ChinhThuc

        [StringLength(20)]
        public string? TrangThai { get; set; } = "Đang mở";

        // Navigation (nếu cần)
        public virtual ICollection<LopHoc>? LopHocs { get; set; }
    }
}
