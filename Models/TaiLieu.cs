using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnlightEnglishCenter.Models
{
    [Table("TaiLieu")]
    public class TaiLieu
    {
        [Key]
        public int MaTaiLieu { get; set; }

        public string? TenTaiLieu { get; set; }
        public string? MoTa { get; set; }
        public string? DuongDan { get; set; }

        public int MaGiaoVien { get; set; }
        public int MaLop { get; set; }

        [ForeignKey("MaGiaoVien")]
        [InverseProperty("TaiLieus")]
        public virtual NguoiDung? MaGiaoVienNavigation { get; set; }

        [ForeignKey("MaLop")]
        [InverseProperty("TaiLieus")]
        public virtual LopHoc? MaLopNavigation { get; set; }
    }
}
