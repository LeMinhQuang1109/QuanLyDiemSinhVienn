using Microsoft.EntityFrameworkCore;
using QuanLiDiem.Data;
using QuanLiDiem.Models;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Cấu hình DbContext và kết nối với cơ sở dữ liệu
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Thêm dịch vụ MVC
builder.Services.AddControllersWithViews();

// Cấu hình dịch vụ Session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Thời gian hết hạn session
    options.Cookie.HttpOnly = true; // Đảm bảo session chỉ được truy cập từ phía server
    options.Cookie.IsEssential = true; // Đảm bảo session được sử dụng ngay cả khi cookie không được người dùng chấp thuận
});

var app = builder.Build();

// Cấu hình pipeline xử lý yêu cầu HTTP
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Cấu hình các middleware
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Kích hoạt Session
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
