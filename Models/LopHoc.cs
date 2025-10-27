using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnlightEnglishCenter.Models
{
    [Table("LopHoc")]
    public class LopHoc
    {
        [Key]
        public int MaLop { get; set; }

        [StringLength(100)]
        public string? TenLop { get; set; }

        [StringLength(50)]
   

   
        public string? TrangThai { get; set; } = "Đang học";

        // FK
        public int? MaGiaoVien { get; set; }
        [ForeignKey("MaGiaoVien")]
        [InverseProperty("LopHocs")]
        public virtual GiaoVien? MaGiaoVienNavigation { get; set; }

        public int? MaKhoaHoc { get; set; }
        [ForeignKey("MaKhoaHoc")]
        public virtual KhoaHoc? MaKhoaHocNavigation { get; set; }

        // Quan hệ khác
        public virtual ICollection<LichHoc>? LichHocs { get; set; }
        public virtual ICollection<PhanCongGiangDay>? PhanCongGiangDays { get; set; }
        public virtual ICollection<TaiLieu>? TaiLieus { get; set; }
        public virtual ICollection<DiemSo>? DiemSos { get; set; }
        public virtual ICollection<DkHocVienLopHoc>? DkHocVienLopHocs { get; set; }
        public virtual ICollection<HocPhi>? HocPhis { get; set; }
        public virtual ICollection<LichThi>? LichThis { get; set; }

        // Hai cột có thật trong DB
        public int? SiSoHienTai { get; set; }
        public int? SiSoToiDa { get; set; }
    }
}
