using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnlightEnglishCenter.Models
{
    [Table("LichHoc")]
    public class LichHoc
    {
        [Key]
        [Column("MaLich")]                 // PK trong DB
        public int MaLich { get; set; }

        public int? MaLop { get; set; }

        [Column(TypeName = "date")]        // SQL: DATE
        public DateTime? NgayHoc { get; set; }

        [Column(TypeName = "time")]        // SQL: TIME
        public TimeSpan? GioBatDau { get; set; }

        [Column(TypeName = "time")]        // SQL: TIME
        public TimeSpan? GioKetThuc { get; set; }

        [StringLength(255)]
        public string? NoiDung { get; set; }   // có trong DB

        [StringLength(100)]
        public string? PhongHoc { get; set; }

        // Navigation
        [ForeignKey(nameof(MaLop))]
        public virtual LopHoc? LopHoc { get; set; }

        public virtual ICollection<DiemDanh>? DiemDanhs { get; set; }
    }
}
