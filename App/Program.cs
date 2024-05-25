﻿using App.Data;
using App.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using App.Areas.Database.Controllers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Đăng ký dịch vụ database
builder.Services.AddControllersWithViews();
builder.Environment.IsDevelopment();
builder.Services.AddRazorPages();
IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
var connectionstring = configuration.GetConnectionString("AppMvcConnectionString");
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(connectionstring);
});

// đăng ký cho dịch vụ giỏ hàng
builder.Services.AddTransient<CartService>();
builder.Services.AddDistributedMemoryCache();           // Đăng ký dịch vụ lưu cache trong bộ nhớ (Session sẽ sử dụng nó)
builder.Services.AddSession(cfg => {                    // Đăng ký dịch vụ Session
    cfg.Cookie.Name = "appmvc";             // Đặt tên Session - tên này sử dụng ở Browser (Cookie)
    cfg.IdleTimeout = new TimeSpan(0, 30, 0);    // Thời gian tồn tại của Session
});

//builder.Services.AddHttpContextAccessor();

// Đăng ký Identity: đăng nhập, quản lý user
builder.Services.AddIdentity<AppUser, IdentityRole>()
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();

// Truy cập IdentityOptions
builder.Services.Configure<IdentityOptions>(options => {
    // Thiết lập về Password
    options.Password.RequireDigit = false; // Không bắt phải có số
    options.Password.RequireLowercase = false; // Không bắt phải có chữ thường
    options.Password.RequireNonAlphanumeric = false; // Không bắt ký tự đặc biệt
    options.Password.RequireUppercase = false; // Không bắt buộc chữ in
    options.Password.RequiredLength = 3; // Số ký tự tối thiểu của password
    options.Password.RequiredUniqueChars = 1; // Số ký tự riêng biệt

    // Cấu hình Lockout - khóa user
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); // Khóa 5 phút
    options.Lockout.MaxFailedAccessAttempts = 5; // Thất bại 5 lầ thì khóa
    options.Lockout.AllowedForNewUsers = false;

    // Cấu hình về User.
    options.User.AllowedUserNameCharacters = // các ký tự đặt tên user
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true;  // Email là duy nhất

    // Cấu hình đăng nhập.
    options.SignIn.RequireConfirmedEmail = true;            // Cấu hình xác thực địa chỉ email (email phải tồn tại)
    options.SignIn.RequireConfirmedPhoneNumber = false;     // Xác thực số điện thoại

});

builder.Services.ConfigureApplicationCookie(options => {
    options.LoginPath = "/Login/";
    options.LogoutPath = "/Logout/";
    options.AccessDeniedPath = "/khongduoctruycap.html";
});

// Kiểm tra quyển để hiện thông tin manager
builder.Services.AddAuthorization(options => {
    options.AddPolicy("ViewManageMenu", builders => {
        builders.RequireAuthenticatedUser();
        builders.RequireRole(RoleName.Administrator);
    });
});





var app = builder.Build();

//using (var scope = app.Services.CreateScope())
//{
//    var services = scope.ServiceProvider;
//    var context = services.GetRequiredService<AppDbContext>();

//    if (await IsDatabaseEmptyAsync(context))
//    {
//        var httpContext = services.GetRequiredService<IHttpContextAccessor>().HttpContext;
//        var routeData = new RouteData();
//        routeData.Values["controller"] = "Database"; // Tên controller
//        routeData.Values["action"] = "SeedData"; // Tên action
//        routeData.Values["area"] = "Database"; // Tên area nếu có

//        var actionContext = new ActionContext(httpContext, routeData, new ControllerActionDescriptor());
//        var controller = services.GetRequiredService<DbManage>(); // Tên controller
//        await controller.SeedDataAsync(); // Gọi phương thức seed data của bạn
//    }
//}



// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseStaticFiles(new StaticFileOptions()
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "Uploads")
        ),
    RequestPath = "/contents"
});
app.UseStaticFiles(new StaticFileOptions()
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")
        ),
});

app.UseSession();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();


// Đã làm xong create - done
// Edit - Done
// Delete Done
// Details Done
// Tích họp Summernote - Done
// Quản lý thư mục, ảnh... 21/4/2024 - Done
// Thêm Thumbnail trong post 22/4/2024 - 23/4/2024 - Done
/*
    Create - Done
    Detail - Done
    Edit - Done
    Show - Done but not edit yet
    -----
    --> delete all photo not need
 */
// Trang ViewPost
/*
    - Trang View Index 23/4/2024 - Done
    - Trang detail: mới truy cập link liên kết, chưa hiển thị nội dung - Done All
    - Hiển thị catolog 6/5/2024 

 */


//static async Task<bool> IsDatabaseEmptyAsync(AppDbContext context)
//{
//    return !await context.Users.AnyAsync();
//}
