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
        public string? TrinhDo { get; set; }

     
        [DataType(DataType.Date)]
        public DateTime? NgayBatDau { get; set; }

        [DataType(DataType.Date)]
        public DateTime? NgayKetThuc { get; set; }

        [StringLength(100)]
        public string? TrangThai { get; set; } = "Đang học";
        // 🔹 Khóa ngoại đến bảng GiaoVien
        public int? MaGiaoVien { get; set; }

        [ForeignKey("MaGiaoVien")]
        [InverseProperty("LopHocs")]
        public virtual GiaoVien? MaGiaoVienNavigation { get; set; }

        // 🔹 Khóa ngoại đến bảng KhoaHoc
        public int? MaKhoaHoc { get; set; }

        [ForeignKey("MaKhoaHoc")]
        public virtual KhoaHoc? MaKhoaHocNavigation { get; set; }

        // 🔹 Các quan hệ khác
        public virtual ICollection<LichHoc>? LichHocs { get; set; }
        public virtual ICollection<PhanCongGiangDay>? PhanCongGiangDays { get; set; }
        public virtual ICollection<TaiLieu>? TaiLieus { get; set; }
        public virtual ICollection<DiemSo>? DiemSos { get; set; }
        public virtual ICollection<DkHocVienLopHoc>? DkHocVienLopHocs { get; set; }
        public virtual ICollection<HocPhi>? HocPhis { get; set; }
        public virtual ICollection<LichThi>? LichThis { get; set; }
    }
}
