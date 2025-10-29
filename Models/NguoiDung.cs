using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EnlightEnglishCenter.Models
{
    [Table("NguoiDung")]
    [Index("MaVaiTro", Name = "IX_NguoiDung_MaVaiTro")]
    [Index("TenDangNhap", Name = "IX_NguoiDung_TenDangNhap", IsUnique = true)]
    public partial class NguoiDung
    {
        [Key]
        public int MaNguoiDung { get; set; }

        [StringLength(100)]
        public string HoTen { get; set; } = null!;

        [StringLength(10)]
        public string? GioiTinh { get; set; }

        public DateOnly? NgaySinh { get; set; }

        [StringLength(200)]
        public string? DiaChi { get; set; }

        [StringLength(15)]
        [Unicode(false)]
        public string? SoDienThoai { get; set; }

        [StringLength(100)]
        [Unicode(false)]
        public string? Email { get; set; }

        [StringLength(50)]
        [Unicode(false)]
        public string TenDangNhap { get; set; } = null!;

        [StringLength(100)]
        [Unicode(false)]
        public string MatKhau { get; set; } = null!;

        public int? MaVaiTro { get; set; }

        [StringLength(20)]
        public string? TrangThai { get; set; }

        public DateTime? KhoaDenNgay { get; set; }

        public string? ResetToken { get; set; }

        public DateTime? ResetTokenExpiry { get; set; }

        public int SoLanSaiMatKhau { get; set; }

        // ================== QUAN HỆ ===================
        [InverseProperty("NguoiDung")]
        public virtual HocVien? HocVien { get; set; }

        [InverseProperty("NguoiLapNavigation")]
        public virtual ICollection<BaoCao> BaoCaos { get; set; } = new List<BaoCao>();

        [InverseProperty("MaHocVienNavigationMaNguoiDungNavigation")]
        public virtual ICollection<DiemDanh> DiemDanhs { get; set; } = new List<DiemDanh>();

        [InverseProperty("MaHocVienNavigation")]
        public virtual ICollection<DiemSo> DiemSos { get; set; } = new List<DiemSo>();

        [InverseProperty("MaHocVienNavigation")]
        public virtual ICollection<DkHocVienLopHoc> DkHocVienLopHocs { get; set; } = new List<DkHocVienLopHoc>();

        // ✅ Sửa lại đúng tên để khớp với GiaoVien.NguoiDung
        [InverseProperty("NguoiDung")]
        public virtual ICollection<GiaoVien> GiaoViens { get; set; } = new List<GiaoVien>();

        [InverseProperty("MaHocVienNavigation")]
        public virtual ICollection<HocPhi> HocPhis { get; set; } = new List<HocPhi>();

        [InverseProperty("MaNguoiDungNavigation")]
        public virtual ICollection<LichSuTruyCap> LichSuTruyCaps { get; set; } = new List<LichSuTruyCap>();

        [ForeignKey("MaVaiTro")]
        [InverseProperty("NguoiDungs")]
        public virtual VaiTro? MaVaiTroNavigation { get; set; }
        [InverseProperty("HocVien")]
        public virtual ICollection<TestDauVao> TestDauVaos { get; set; } = new HashSet<TestDauVao>();
    }
}


