using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnlightEnglishCenter.Models
{
    [Table("PhanCongGiangDay")]
    public class PhanCongGiangDay
    {
        [Key]
        public int MaPhanCong { get; set; }

        public int? MaGiaoVien { get; set; }
        public int? MaLop { get; set; }

        [StringLength(100)]
        public string? GhiChu { get; set; }
        [DataType(DataType.Date)]
        public DateTime? NgayPhanCong { get; set; }

        // Navigation
        [ForeignKey("MaGiaoVien")]
        public virtual GiaoVien? GiaoVien { get; set; }

        [ForeignKey("MaLop")]
        public virtual LopHoc? LopHoc { get; set; }

    }
}
