using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EnlightEnglishCenter.Models;

[PrimaryKey("MaHocVien", "MaLop")]
[Table("DK_HocVien_LopHoc")]
public partial class DK_HocVien_LopHoc
{
    [Key]
    public int MaHocVien { get; set; }

    [Key]
    public int MaLop { get; set; }

    public DateOnly? NgayDangKy { get; set; }

    [StringLength(30)]
    public string? TrangThai { get; set; }

    [ForeignKey("MaHocVien")]
    [InverseProperty("DK_HocVien_LopHocs")]
    public virtual NguoiDung MaHocVienNavigation { get; set; } = null!;

    [ForeignKey("MaLop")]
    [InverseProperty("DK_HocVien_LopHocs")]
    public virtual LopHoc MaLopNavigation { get; set; } = null!;
}
