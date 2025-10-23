using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EnlightEnglishCenter.Models
{
    [Table("DiemSo")]
    [Index("MaHocVien", Name = "IX_DiemSo_MaHocVien")]
    [Index("MaLop", Name = "IX_DiemSo_MaLop")]
    public partial class DiemSo
    {
        [Key]
        public int MaDiem { get; set; }

        public int MaHocVien { get; set; }

        public int MaLop { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal? DiemGiuaKy { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal? DiemCuoiKy { get; set; }

        public string? NhanXet { get; set; }

        [ForeignKey("MaHocVien")]
        [InverseProperty("DiemSos")]
        public virtual NguoiDung MaHocVienNavigation { get; set; } = null!;

        [ForeignKey("MaLop")]
        [InverseProperty("DiemSos")]
        public virtual LopHoc MaLopNavigation { get; set; } = null!;

        // ✅ Thuộc tính tạm để hiển thị tên học viên trong View
        [NotMapped]
        public string? HoTen { get; set; }
    }
}
