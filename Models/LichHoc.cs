using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnlightEnglishCenter.Models
{
    [Table("LichHoc")]
    public class LichHoc
    {
        [Key]
        public int MaLichHoc { get; set; }

        public int? MaLop { get; set; }
        public int? MaGiaoVien { get; set; }

        public DateOnly? NgayHoc { get; set; }

        [StringLength(10)]
        public string? GioBatDau { get; set; }

        [StringLength(10)]
        public string? GioKetThuc { get; set; }

        [StringLength(50)]
        public string? PhongHoc { get; set; }

        // Navigation
        [ForeignKey("MaLop")]
        public virtual LopHoc? LopHoc { get; set; }

        [ForeignKey("MaGiaoVien")]
        public virtual GiaoVien? GiaoVien { get; set; }
        public virtual ICollection<DiemDanh>? DiemDanhs { get; set; }

    }
}
