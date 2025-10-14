using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EnlightEnglishCenter.Models
{
    [Table("NguoiDung")]
    [Index("TenDangNhap", IsUnique = true)]
    [Index("Email", IsUnique = true)]
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

        // ================== LIÊN KẾT 1-N ==================

        [InverseProperty("NguoiLapNavigation")]
        public virtual ICollection<BaoCao> BaoCaos { get; set; } = new List<BaoCao>();

        [InverseProperty("MaHocVienNavigation")]
     
        public virtual ICollection<DiemDanh> DiemDanhs { get; set; } = new List<DiemDanh>();

        [InverseProperty("MaHocVienNavigation")]
        public virtual ICollection<DiemSo> DiemSos { get; set; } = new List<DiemSo>();

        [InverseProperty("MaHocVienNavigation")]
        public virtual ICollection<HocPhi> HocPhis { get; set; } = new List<HocPhi>();

        [InverseProperty("MaNguoiDungNavigation")]
        public virtual ICollection<LichSuTruyCap> LichSuTruyCaps { get; set; } = new List<LichSuTruyCap>();

        [InverseProperty("MaGiaoVienNavigation")]
        public virtual ICollection<LopHoc> LopHocs { get; set; } = new List<LopHoc>();

        [InverseProperty("MaGiaoVienNavigation")]
        public virtual ICollection<LuongGiaoVien> LuongGiaoViens { get; set; } = new List<LuongGiaoVien>();

        [InverseProperty("MaGiaoVienNavigation")]
        public virtual ICollection<TaiLieu> TaiLieus { get; set; } = new List<TaiLieu>();

        [InverseProperty("HocVien")]
        public virtual ICollection<TestDauVao> TestDauVaos { get; set; } = new List<TestDauVao>();

        // ================== LIÊN KẾT NHIỀU-1 ==================
        public int SoLanSaiMatKhau { get; set; } = 0;  // Số lần nhập sai liên tiếp
        public DateTime? KhoaDenNgay { get; set; }      // Thời điểm khóa tạm
        public string? ResetToken { get; set; }
        public DateTime? ResetTokenExpiry { get; set; }
        [ForeignKey("MaVaiTro")]
        [InverseProperty("NguoiDungs")]
        public virtual VaiTro? MaVaiTroNavigation { get; set; }
    }
}
