using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnlightEnglishCenter.Models
{
    [Table("DK_HocVien_LopHoc")]
    public class DkHocVienLopHoc
    {
        public int MaHocVien { get; set; }
        public int MaLop { get; set; }

        public DateTime? NgayDangKy { get; set; }

        [StringLength(50)]
        public string? TrangThai { get; set; }

        [ForeignKey("MaHocVien")]
        public virtual NguoiDung? HocVien { get; set; }

        [ForeignKey("MaLop")]
        public virtual LopHoc? LopHoc { get; set; }
    }
}
