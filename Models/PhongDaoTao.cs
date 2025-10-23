using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnlightEnglishCenter.Models
{
    [Table("PhongDaoTao")]
    public class PhongDaoTao
    {
        [Key]
        public int MaPhongDaoTao { get; set; }

        public int MaNguoiDung { get; set; }

        [StringLength(100)]
        public string? ChucVu { get; set; }

        [StringLength(255)]
        public string? GhiChu { get; set; }

        [StringLength(20)]
        public string? TrangThai { get; set; }

        [ForeignKey("MaNguoiDung")]
        public NguoiDung? NguoiDung { get; set; }
    }
}
