using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EnlightEnglishCenter.Models;

[Table("LopHoc")]
public partial class LopHoc
{
    [Key]
    public int MaLop { get; set; }

    [StringLength(100)]
    public string? TenLop { get; set; }

    public int? MaKhoaHoc { get; set; }

    public int? MaGiaoVien { get; set; }

    public int? SiSoToiDa { get; set; }

    public int? SiSoHienTai { get; set; }

    [StringLength(20)]
    public string? TrangThai { get; set; }

    [InverseProperty("MaLopNavigation")]

   
    public virtual ICollection<DiemSo> DiemSos { get; set; } = new List<DiemSo>();

    [InverseProperty("MaLopNavigation")]
    public virtual ICollection<HocPhi> HocPhis { get; set; } = new List<HocPhi>();

    [InverseProperty("MaLopNavigation")]
    public virtual ICollection<LichHoc> LichHocs { get; set; } = new List<LichHoc>();

    [InverseProperty("MaLopNavigation")]
    public virtual ICollection<LichThi> LichThis { get; set; } = new List<LichThi>();

    [ForeignKey("MaGiaoVien")]
    [InverseProperty("LopHocs")]
    public virtual NguoiDung? MaGiaoVienNavigation { get; set; }

    [ForeignKey("MaKhoaHoc")]
    [InverseProperty("LopHocs")]
    public virtual KhoaHoc? MaKhoaHocNavigation { get; set; }

    [InverseProperty("MaLopNavigation")]
    public virtual ICollection<TaiLieu> TaiLieus { get; set; } = new List<TaiLieu>();
}
