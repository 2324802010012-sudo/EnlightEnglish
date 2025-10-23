using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EnlightEnglishCenter.Models;

[PrimaryKey("MaHocVien", "MaLich")]
[Table("DiemDanh")]
[Index("MaHocVienNavigationMaNguoiDung", Name = "IX_DiemDanh_MaHocVienNavigationMaNguoiDung")]
[Index("MaLichNavigationMaLich", Name = "IX_DiemDanh_MaLichNavigationMaLich")]
public partial class DiemDanh
{
    [Key]
    public int MaHocVien { get; set; }

    [Key]
    public int MaLich { get; set; }

    [StringLength(20)]
    public string TrangThai { get; set; } = null!;

    public int? MaHocVienNavigationMaNguoiDung { get; set; }

    public int? MaLichNavigationMaLich { get; set; }

    [ForeignKey("MaHocVienNavigationMaNguoiDung")]
    [InverseProperty("DiemDanhs")]
    public virtual NguoiDung? MaHocVienNavigationMaNguoiDungNavigation { get; set; }

    [ForeignKey("MaLichNavigationMaLich")]
    [InverseProperty("DiemDanhs")]
    public virtual LichHoc? MaLichNavigationMaLichNavigation { get; set; }
}
