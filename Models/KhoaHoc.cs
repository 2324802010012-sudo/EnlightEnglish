using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EnlightEnglishCenter.Models
{
    [Table("KhoaHoc")]
    public class KhoaHoc
    {
        [Key]
        public int MaKhoaHoc { get; set; }

        [StringLength(100)]
        public string TenKhoaHoc { get; set; } = null!;

        [StringLength(255)]
        public string? MoTa { get; set; }

        [Column(TypeName = "decimal(12,2)")]
        public decimal? HocPhi { get; set; }

        [StringLength(50)]
        public string? ThoiLuong { get; set; }

        [StringLength(50)]
        public string? CapDo { get; set; }

        public DateOnly? NgayBatDau { get; set; }
        public DateOnly? NgayKetThuc { get; set; }

        public string? ChuanDauRa { get; set; }

        [StringLength(20)]
        public string? TrangThai { get; set; }

        public string? LoTrinhHoc { get; set; }
        public DateTime? NgayKhaiGiang { get; set; }

        [StringLength(100)]
        public string? ThoiGianHoc { get; set; }

        [StringLength(50)]
        public string? TrinhDo { get; set; }

        [StringLength(100)]
        public string? GiangVien { get; set; }

        [StringLength(30)]
        public string? HinhThuc { get; set; }

        // 🔹 Quan hệ 1 - N với LopHoc
        [InverseProperty("MaKhoaHocNavigation")]
        public virtual ICollection<LopHoc> LopHocs { get; set; } = new List<LopHoc>();

        // 🔹 Quan hệ 1 - N với TestDauVao
        [InverseProperty("KhoaHocDeXuatNavigation")]
        public virtual ICollection<TestDauVao> TestDauVaos { get; set; } = new List<TestDauVao>();
    }
}
