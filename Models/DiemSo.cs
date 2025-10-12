using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnlightEnglishCenter.Models
{
    [Table("DiemSo")]
    public class DiemSo
    {
        [Key]
        public int MaDiem { get; set; }

        public int MaHocVien { get; set; }

        public int MaLop { get; set; }

        public decimal? DiemGiuaKy { get; set; }
        public decimal? DiemCuoiKy { get; set; }

        public string? NhanXet { get; set; }

        [NotMapped]
        public decimal DiemTrungBinh => (DiemGiuaKy ?? 0 + DiemCuoiKy ?? 0) / 2;

        [ForeignKey("MaHocVien")]
        [InverseProperty("DiemSos")]
        public virtual NguoiDung? MaHocVienNavigation { get; set; }

        [ForeignKey("MaLop")]
        [InverseProperty("DiemSos")]
        public virtual LopHoc? MaLopNavigation { get; set; }
    }
}
