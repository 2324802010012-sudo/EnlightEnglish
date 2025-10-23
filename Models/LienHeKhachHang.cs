using System;
using System.ComponentModel.DataAnnotations;

namespace EnlightEnglishCenter.Models
{
    public class LienHeKhachHang
    {
        [Key]
        public int MaLienHe { get; set; }

        [Required, MaxLength(100)]
        public string? HoTen { get; set; }

        [MaxLength(15)]
        public string? DienThoai { get; set; }

        [MaxLength(100)]
        public string? Email { get; set; }

        [MaxLength(50)]
        public string? TrangThai { get; set; } = "Tiềm năng"; // Mặc định là khách tiềm năng

        [MaxLength(100)]
        public string? KhoaHoc { get; set; }

        public DateTime NgayLienHe { get; set; } = DateTime.Now;

        [MaxLength(255)]
        public string? GhiChu { get; set; }
    }
}
