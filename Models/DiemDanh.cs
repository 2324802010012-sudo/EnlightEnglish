using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnlightEnglishCenter.Models
{
    [Table("DiemDanh")]
    public class DiemDanh
    {
        [ForeignKey("MaHocVien")]
        public int MaHocVien { get; set; }

        [ForeignKey("MaLich")]
        public int MaLich { get; set; }

        [StringLength(20)]
        public string TrangThai { get; set; } = "Có mặt";

        // Navigation
        [InverseProperty("DiemDanhs")]
        public virtual NguoiDung? MaHocVienNavigation { get; set; }

        [InverseProperty("DiemDanhs")]
        public virtual LichHoc? MaLichNavigation { get; set; }
    }
}
