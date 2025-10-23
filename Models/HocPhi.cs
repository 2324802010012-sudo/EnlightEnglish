using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EnlightEnglishCenter.Models;

[Table("HocPhi")]
[Index("MaHocVien", Name = "IX_HocPhi_MaHocVien")]
[Index("MaLop", Name = "IX_HocPhi_MaLop")]
public partial class HocPhi
{
    [Key]
    public int MaHocPhi { get; set; }

    public int MaHocVien { get; set; }

    public int MaLop { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal SoTienPhaiDong { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal SoTienDaDong { get; set; }

    public string? TrangThai { get; set; }

    public DateTime? NgayDongCuoi { get; set; }

    [ForeignKey("MaHocVien")]
    [InverseProperty("HocPhis")]
    public virtual NguoiDung MaHocVienNavigation { get; set; } = null!;

    [ForeignKey("MaLop")]
    [InverseProperty("HocPhis")]
    public virtual LopHoc MaLopNavigation { get; set; } = null!;
}
