using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnlightEnglishCenter.Models
{
    [Table("LichSuTruyCap")]
    public class LichSuTruyCap
    {
        [Key]
        public int MaLog { get; set; }

        public int MaNguoiDung { get; set; }
        public string? HanhDong { get; set; }
        public DateTime ThoiGian { get; set; } = DateTime.Now;
        public string? DiaChiIP { get; set; }
        public string? MoTa { get; set; }

        [ForeignKey("MaNguoiDung")]
        [InverseProperty("LichSuTruyCaps")]
        public virtual NguoiDung? MaNguoiDungNavigation { get; set; }
    }
}
