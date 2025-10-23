using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EnlightEnglishCenter.Models
{
    [Table("TaiLieu")]
    [Index("MaLop", Name = "IX_TaiLieu_MaLop")]
    [Index("MaGiaoVien", Name = "IX_TaiLieu_MaGiaoVien")]
    public class TaiLieu
    {
        [Key]
        public int MaTaiLieu { get; set; }

        public int? MaLop { get; set; } // Khóa ngoại đến LopHoc
        public int? MaGiaoVien { get; set; } // Khóa ngoại đến GiaoVien

        [StringLength(200)]
        public string? TenTaiLieu { get; set; }

        [StringLength(255)]
        public string? DuongDan { get; set; } // đường dẫn file

        [StringLength(100)]
        public string? LoaiTaiLieu { get; set; }

        public DateTime? NgayDang { get; set; }

        // 🔹 Navigation: Tài liệu thuộc về 1 lớp
        [ForeignKey("MaLop")]
        [InverseProperty("TaiLieus")]
        public virtual LopHoc? MaLopNavigation { get; set; }

        // 🔹 Navigation: Tài liệu thuộc về 1 giáo viên
        [ForeignKey("MaGiaoVien")]
        [InverseProperty("TaiLieus")]
        public virtual GiaoVien? MaGiaoVienNavigation { get; set; }
    }
}
