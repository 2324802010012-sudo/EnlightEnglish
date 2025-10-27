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

        public int? MaLop { get; set; }
        public int? MaGiaoVien { get; set; }

        [StringLength(200)]
        public string? TenTaiLieu { get; set; }

        [StringLength(1000)]
        public string? MoTa { get; set; }

        [StringLength(255)]
        public string? DuongDan { get; set; }

        // ⬇️ Cột đúng trong DB
        public DateTime? NgayTaiLen { get; set; }

        // (tùy chọn) Navigation
        [ForeignKey("MaLop")] public virtual LopHoc? MaLopNavigation { get; set; }
        [ForeignKey("MaGiaoVien")] public virtual GiaoVien? MaGiaoVienNavigation { get; set; }
    }
}
