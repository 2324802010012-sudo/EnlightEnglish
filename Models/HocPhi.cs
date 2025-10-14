using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnlightEnglishCenter.Models
{
    [Table("HocPhi")]
    public class HocPhi
    {
        [Key]
        public int MaHocPhi { get; set; }

        public int MaHocVien { get; set; }
        public int MaLop { get; set; }

        public decimal SoTienPhaiDong { get; set; }
        public decimal SoTienDaDong { get; set; }
        public string? TrangThai { get; set; }
        public DateTime? NgayDongCuoi { get; set; }

        [ForeignKey("MaHocVien")]
        [InverseProperty("HocPhis")]
        public virtual NguoiDung? MaHocVienNavigation { get; set; }

        [ForeignKey("MaLop")]
        [InverseProperty("HocPhis")]
        public virtual LopHoc? MaLopNavigation { get; set; }
    }
}
