using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnlightEnglishCenter.Models
{
    [Table("HocVien")]
    public class HocVien
    {
        [Key]
        public int MaHocVien { get; set; }

        [StringLength(100)]
        public string? HoTen { get; set; }

        [StringLength(100)]
        public string? Email { get; set; }

        [StringLength(20)]
        public string? SoDienThoai { get; set; }

        public DateTime? NgayDangKy { get; set; }

        [StringLength(30)]
        public string? TrangThai { get; set; }

        public virtual ICollection<TestDauVao>? TestDauVaos { get; set; }
    }
}
