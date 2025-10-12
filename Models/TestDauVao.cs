
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnlightEnglishCenter.Models
{
    [Table("TestDauVao")]
    public class TestDauVao
    {
        [Key]
        public int MaTest { get; set; }

        [ForeignKey("HocVien")]
        public int MaHocVien { get; set; }

        public DateTime? NgayTest { get; set; }
        public decimal? DiemNghe { get; set; }
        public decimal? DiemDoc { get; set; }
        public decimal? DiemViet { get; set; }
        public decimal? DiemNguPhap { get; set; }
        public decimal? TongDiem { get; set; }
        public string? KhoaHocDeXuat { get; set; }
        public string? LoTrinhHoc { get; set; }
        public string TrangThai { get; set; } = "Chờ xác nhận";

        [InverseProperty("TestDauVaos")]
        public virtual NguoiDung? HocVien { get; set; }
    }
}
