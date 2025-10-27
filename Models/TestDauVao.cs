using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnlightEnglishCenter.Models
{
    [Table("TestDauVao")]
    public class TestDauVao
    {
        [Key] public int MaTest { get; set; }

        public int? MaHocVien { get; set; }         // FK -> NguoiDung.MaNguoiDung
        public DateTime? NgayTest { get; set; }     // default GETDATE() trong DB cũng OK

        [Column(TypeName = "decimal(4,1)")] public decimal? DiemNghe { get; set; }
        [Column(TypeName = "decimal(4,1)")] public decimal? DiemDoc { get; set; }
        [Column(TypeName = "decimal(4,1)")] public decimal? DiemViet { get; set; }
        [Column(TypeName = "decimal(4,1)")] public decimal? DiemNguPhap { get; set; }
        [Column(TypeName = "decimal(4,1)")] public decimal? TongDiem { get; set; }

        public int? KhoaHocDeXuat { get; set; }     // FK -> KhoaHoc.MaKhoaHoc
        public string? LoTrinhHoc { get; set; }     // nvarchar(max) theo script
        [StringLength(30)] public string? TrangThai { get; set; }  // default N'Chờ xác nhận'
        [StringLength(100)] public string? HoTen { get; set; }
        [StringLength(100)] public string? Email { get; set; }

        // Navigation (KHÔNG dùng attribute ForeignKey/InverseProperty ở đây)
        public virtual NguoiDung? HocVien { get; set; }
        public virtual KhoaHoc? KhoaHocDeXuatNavigation { get; set; }
    }
}
