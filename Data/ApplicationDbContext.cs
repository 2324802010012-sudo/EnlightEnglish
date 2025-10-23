using EnlightEnglishCenter.Models;
using Microsoft.EntityFrameworkCore;

namespace EnlightEnglishCenter.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // ==========================
        // 🔹 Các DbSet ánh xạ bảng
        // ==========================
        public DbSet<NguoiDung> NguoiDungs { get; set; }
        public DbSet<VaiTro> VaiTros { get; set; }
        public DbSet<KhoaHoc> KhoaHocs { get; set; }
        public DbSet<LopHoc> LopHocs { get; set; }
        public DbSet<TestDauVao> TestDauVaos { get; set; }
        public DbSet<HocPhi> HocPhis { get; set; }
        public DbSet<LuongGiaoVien> LuongGiaoViens { get; set; }
        public DbSet<LichHoc> LichHocs { get; set; }
        public DbSet<LichThi> LichThis { get; set; }
        public DbSet<DiemSo> DiemSos { get; set; }
        public DbSet<TaiLieu> TaiLieus { get; set; }
        public DbSet<DiemDanh> DiemDanhs { get; set; }
        public DbSet<LichSuTruyCap> LichSuTruyCaps { get; set; }
        public DbSet<BaoCao> BaoCaos { get; set; }
        public DbSet<DkHocVienLopHoc> DkHocVienLopHocs { get; set; }

        public DbSet<DangKyTuVan> DangKyTuVan { get; set; }

        public DbSet<GiaoVien> GiaoViens { get; set; }
        public DbSet<PhanCongGiangDay> PhanCongGiangDays { get; set; }
        public DbSet<PhongDaoTao> PhongDaoTaos { get; set; }
        public DbSet<NhanVienLeTan> NhanVienLeTans { get; set; }
        public DbSet<HocVien> HocViens { get; set; }
        public DbSet<PhongHoc> PhongHocs { get; set; }




        // ==========================
        // ⚙️ Cấu hình chi tiết quan hệ
        // ==========================
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 🔹 Cấu hình bảng NguoiDung
            modelBuilder.Entity<NguoiDung>()
                .HasOne(u => u.MaVaiTroNavigation)
                .WithMany(v => v.NguoiDungs)
                .HasForeignKey(u => u.MaVaiTro)
                .OnDelete(DeleteBehavior.Restrict);

           

            // 🔹 Cấu hình DiemDanh (khóa chính kép)
            modelBuilder.Entity<DiemDanh>()
                .HasKey(d => new { d.MaHocVien, d.MaLich });

            // 🔹 TestDauVao – liên kết học viên
            modelBuilder.Entity<TestDauVao>()
                .HasOne(t => t.HocVien)
                .WithMany(u => u.TestDauVaos)
                .HasForeignKey(t => t.MaHocVien)
                .OnDelete(DeleteBehavior.Restrict);

            // 🔹 TaiLieu – liên kết lớp và giáo viên
            modelBuilder.Entity<TaiLieu>()
                .HasOne(t => t.MaGiaoVienNavigation)
                .WithMany(u => u.TaiLieus)
                .HasForeignKey(t => t.MaGiaoVien)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TaiLieu>()
                .HasOne(t => t.MaLopNavigation)
                .WithMany(l => l.TaiLieus)
                .HasForeignKey(t => t.MaLop)
                .OnDelete(DeleteBehavior.Restrict);

            // 🔹 BaoCao – liên kết NguoiDung
            modelBuilder.Entity<BaoCao>()
                .HasOne(b => b.NguoiLapNavigation)
                .WithMany(u => u.BaoCaos)
                .HasForeignKey(b => b.NguoiLap)
                .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<DkHocVienLopHoc>()
                .HasKey(d => new { d.MaHocVien, d.MaLop });
        }
    }
}
