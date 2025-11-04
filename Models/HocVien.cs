using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnlightEnglishCenter.Models
{
    [Table("HocVien")]
    public class HocVien
    {
        [Key]
        [Display(Name = "Mã học viên")]
        public int MaHocVien { get; set; }

        [StringLength(100)]
        public string? HoTen { get; set; }

        [StringLength(100)]
        public string? Email { get; set; }

        [StringLength(20)]
        public string? SoDienThoai { get; set; }

        public int? MaNguoiDung { get; set; }


        [ForeignKey("MaNguoiDung")]
        [InverseProperty("HocVien")] // 👈 phải trùng với property ở NguoiDung.cs
        public virtual NguoiDung? NguoiDung { get; set; }

        public DateTime? NgayDangKy { get; set; }

        [StringLength(30)]
        public string? TrangThai { get; set; }

        public virtual ICollection<TestDauVao>? TestDauVaos { get; set; }
   [InverseProperty("MaHocVienNavigation")]
public virtual ICollection<DiemSo> DiemSos { get; set; } = new List<DiemSo>();



    }
}
