using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EnlightEnglishCenter.Models
{
    [Table("LuongGiaoVien")]
    [Index("MaGiaoVien", Name = "IX_LuongGiaoVien_MaGiaoVien")]
    public partial class LuongGiaoVien
    {
        [Key]
        public int MaLuong { get; set; }

        public int? MaGiaoVien { get; set; }

        public int? Thang { get; set; }

        public int? Nam { get; set; }

        public int? SoBuoiDay { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal? LuongMoiBuoi { get; set; }

        [Column(TypeName = "decimal(21, 2)")]
        public decimal? TongLuong { get; set; }

        [StringLength(50)]
        public string? TrangThai { get; set; } = "Đã tính";

        // ✅ Sửa lại: liên kết đúng sang bảng GiaoVien
        [ForeignKey("MaGiaoVien")]
        [InverseProperty("LuongGiaoViens")]
        public virtual GiaoVien? MaGiaoVienNavigation { get; set; }
    }
}
