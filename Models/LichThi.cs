using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EnlightEnglishCenter.Models;

[Table("LichThi")]
[Index("MaLop", Name = "IX_LichThi_MaLop")]
public partial class LichThi
{
    [Key]
    public int MaThi { get; set; }

    public int? MaLop { get; set; }

    public DateOnly? NgayThi { get; set; }

    public TimeOnly? GioThi { get; set; }

    [StringLength(50)]
    public string? LoaiThi { get; set; }

    [StringLength(100)]
    public string? DiaDiem { get; set; }

    [ForeignKey("MaLop")]
    [InverseProperty("LichThis")]
    public virtual LopHoc? MaLopNavigation { get; set; }
}
