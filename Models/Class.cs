namespace EnlightEnglishCenter.Models
{
    public class Class
    {
        public int MaLienHe { get; set; }            // Khóa chính (tự tăng)
        public string? HoTen { get; set; }           // Họ tên khách hàng
        public string? DienThoai { get; set; }       // Số điện thoại
        public string? Email { get; set; }           // Email liên hệ
        public string? TrangThai { get; set; }       // Trạng thái (Tiềm năng, Đã xác nhận, Đặt lịch, v.v.)
        public string? KhoaHoc { get; set; }         // Khóa học khách hàng quan tâm
        public DateTime NgayLienHe { get; set; } = DateTime.Now; // Ngày liên hệ
        public string? GhiChu { get; set; }          // Ghi chú thêm
    }
}
