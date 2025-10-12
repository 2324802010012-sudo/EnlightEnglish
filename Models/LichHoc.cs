using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EnlightEnglishCenter.Models;

[Table("LichHoc")]
public partial class LichHoc
{
    [Key]
    public int MaLich { get; set; }

    public int? MaLop { get; set; }

    public DateOnly? NgayHoc { get; set; }

    public TimeOnly? GioBatDau { get; set; }

    public TimeOnly? GioKetThuc { get; set; }

    [StringLength(255)]
    public string? NoiDung { get; set; }

    [InverseProperty("MaLichNavigation")]
    public virtual ICollection<DiemDanh> DiemDanhs { get; set; } = new List<DiemDanh>();

    [ForeignKey("MaLop")]
    [InverseProperty("LichHocs")]
    public virtual LopHoc? MaLopNavigation { get; set; }
}
