using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnlightEnglishCenter.Models
{
    [Table("NhanVienLeTan")]
    public class NhanVienLeTan
    {
        [Key]
        public int MaLeTan { get; set; }

        public int MaNguoiDung { get; set; }

        [StringLength(50)]
        public string? CaLam { get; set; }

        [StringLength(100)]
        public string? KinhNghiem { get; set; }

        [StringLength(20)]
        public string? TrangThai { get; set; }

        [ForeignKey("MaNguoiDung")]
        public NguoiDung? NguoiDung { get; set; }
    }
}
