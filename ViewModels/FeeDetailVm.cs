namespace EnlightEnglishCenter.ViewModels
{
    public class FeeItemVm
    {
        public string NoiDung { get; set; } = "";
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public decimal ThanhTien => SoLuong * DonGia;
    }

    public class FeeStudentVm
    {
        public string HoTen { get; set; } = "";
        public string? Email { get; set; }
        public string? SoDienThoai { get; set; }
        public string? MaHocVien { get; set; }
    }

    public class FeeClassVm
    {
        public string? TenLop { get; set; }
        public string? TenKhoaHoc { get; set; }
        public string? CaHoc { get; set; }
    }

    public class FeePaymentVm
    {
        public DateTime ThoiGian { get; set; }
        public decimal SoTien { get; set; }
        public string? HinhThuc { get; set; }  // Tiền mặt/Chuyển khoản/Momo...
        public string? GhiChu { get; set; }
    }

    public class FeeDetailVm
    {
        public string MaDon { get; set; } = "";
        public DateTime NgayTao { get; set; }
        public DateTime? NgayThanhToan { get; set; }
        public string TrangThai { get; set; } = "Chờ xác nhận"; // Chờ xác nhận / Đã thanh toán

        public FeeStudentVm HocVien { get; set; } = new();
        public FeeClassVm LopHoc { get; set; } = new();

        public List<FeeItemVm> Items { get; set; } = new();

        // Tổng hợp
        public decimal TienHang => Items.Sum(i => i.ThanhTien);
        public decimal GiamTru { get; set; }  // học bổng/mã giảm
        public decimal TongTien => TienHang - GiamTru;

        // Thu/chi tiết
        public List<FeePaymentVm> Payments { get; set; } = new(); // nếu nhiều lần thu
        public decimal DaThu => Payments.Sum(p => p.SoTien);
        public decimal ConNo => Math.Max(0, TongTien - DaThu);

        public string? GhiChu { get; set; }
        public string? NhanVienLap { get; set; }
        public string? PhuongThucThanhToanMacDinh { get; set; }
    }
}
