namespace EnlightEnglishCenter.Models
{
    public class DangKyTuVan
    {
        public int Id { get; set; }
        public string? HoTen { get; set; }
        public string? Email { get; set; }
        public string? SoDienThoai { get; set; }
        public string TrangThai { get; set; } = "Chưa liên hệ";
    }
}