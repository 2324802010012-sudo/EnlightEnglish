using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnlightEnglishCenter.Models
{
    [Table("PhongHoc")]
    public class PhongHoc
    {
        [Key]
        public int MaPhongHoc { get; set; }

        [StringLength(100)]
        public string? TenPhong { get; set; }

        [StringLength(50)]
        public string? ViTri { get; set; }

        [StringLength(30)]
        public string? TrangThai { get; set; }
    }
}
