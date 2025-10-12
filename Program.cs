using EnlightEnglishCenter.Data;
using Microsoft.EntityFrameworkCore;
using EnlightEnglishCenter.Models;

var builder = WebApplication.CreateBuilder(args);

// ==========================
// 🧩 CẤU HÌNH DỊCH VỤ (Services)
// ==========================
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();


// ✅ Kết nối CSDL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ✅ Bật SESSION
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // thời gian lưu session
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// ==========================
// ⚙️ CẤU HÌNH PIPELINE
// ==========================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// ✅ Bật session trước khi map route
app.UseSession();

app.UseAuthorization();

// ==========================
// 🗺️ Định nghĩa route mặc định
// ==========================
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
