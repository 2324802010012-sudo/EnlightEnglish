using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EnlightEnglishCenter.Models;

[Table("KhoaHoc")]
public partial class KhoaHoc
{
    [Key]
    public int MaKhoaHoc { get; set; }

    [StringLength(100)]
    public string TenKhoaHoc { get; set; } = null!;

    [StringLength(255)]
    public string? MoTa { get; set; }

    [Column(TypeName = "decimal(12, 2)")]
    public decimal? HocPhi { get; set; }

    [StringLength(50)]
    public string? ThoiLuong { get; set; }

    [StringLength(50)]
    public string? CapDo { get; set; }

    public DateOnly? NgayBatDau { get; set; }

    public DateOnly? NgayKetThuc { get; set; }

    public string? ChuanDauRa { get; set; }

    public string? LoTrinhHoc { get; set; }

    [StringLength(20)]
    public string? TrangThai { get; set; }

    [InverseProperty("MaKhoaHocNavigation")]
    public virtual ICollection<LopHoc> LopHocs { get; set; } = new List<LopHoc>();
}
