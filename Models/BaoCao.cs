using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnlightEnglishCenter.Models
{
    [Table("BaoCao")]
    public class BaoCao
    {
        [Key]
        public int MaBaoCao { get; set; }

        [StringLength(100)]
        public string? LoaiBaoCao { get; set; }

        public string? NoiDung { get; set; }

        public int? NguoiLap { get; set; }

        public DateTime NgayLap { get; set; }

        // ✅ Liên kết đúng chiều với NguoiDung
        [ForeignKey("NguoiLap")]
        [InverseProperty("BaoCaos")]
        public virtual NguoiDung? NguoiLapNavigation { get; set; }
    }
}
