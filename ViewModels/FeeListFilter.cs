namespace EnlightEnglishCenter.ViewModels
{
    public class FeeListFilter
    {
        public string? Keyword { get; set; }          // mã đơn / tên HV / lớp / khóa học
        public string? TrangThai { get; set; }        // "ChoXacNhan" | "DaThanhToan" | null = tất cả
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
