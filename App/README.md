# VeSinhMayGiat
 Dự án làm của kinh doanh vệ sinh máy giặt
 # Bước 1: Kết nối làm việc với database
     B1: Add package
        dotnet tool install --global dotnet-ef
        dotnet tool install --global dotnet-aspnet-codegenerator
        dotnet add package Microsoft.VisualStudio.Web.CodeGeneration.Design
        dotnet add package Microsoft.EntityFrameworkCore.Design
        dotnet add package Microsoft.EntityFrameworkCore.SqlServer
        dotnet add package MySql.Data.EntityFramework
    B2: Thêm conection string trong appsetting.json
     "ConnectionStrings": {
        "AppMvcConnectionString": "Server=.;Database=appmvcnew;Trusted_Connection=SSPI;Encrypt=false;TrustServerCertificate=true"
    B3: Thêm dịch vụ AppDbConText
        Thêm lớp App DBcontext
    B4: Đăng ký dịch vụ tại lớp program
            builder.Services.AddControllersWithViews(); 
            builder.Environment.IsDevelopment();
            builder.Services.AddRazorPages();

            IConfigurationRoot configuration = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.json")
                            .Build();
            // lien ket databas
            var connectionString = configuration.GetConnectionString("AppMvcConnectionString");
            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(connectionString); // Cấu hình kết nối đến cơ sở dữ liệu của bạn
            });
    B5: Thực hiện migration
        dotnet ef migrations add init
        dotnet ef database update
    --> Xong rồi
 # SCSS THÀNH CSS
    B1: Tạo thư mục  assets/site.scss
    B2: Thực hiện: 
        npm init
        những thông tin sau tùy ý nhập
    B3: Add package
        npm install --global gulp-cli
        npm install gulp
        npm install node-sass postcss sass
        npm install gulp-sass gulp-less gulp-concat gulp-cssmin gulp-uglify rimraf gulp-postcss gulp-rename
    B4: Tạo Gulpfile.js
    B5: 
        gulp [Ten]
        Nếu lối: Gõ:  Set-ExecutionPolicy -ExecutionPolicy Bypass -Scope CurrentUser

 # Thêm role đăng nhập + quản lý user
    B1: add package
        dotnet add package System.Data.SqlClient
        dotnet add package Microsoft.EntityFrameworkCore
        dotnet add package Microsoft.EntityFrameworkCore.SqlServer
        dotnet add package Microsoft.EntityFrameworkCore.Design
        dotnet add package Microsoft.Extensions.DependencyInjection
        dotnet add package Microsoft.Extensions.Logging.Console

        dotnet add package Microsoft.AspNetCore.Identity
        dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore
        dotnet add package Microsoft.VisualStudio.Web.CodeGeneration.Design
        dotnet add package Microsoft.AspNetCore.Identity.UI
        dotnet add package Microsoft.AspNetCore.Authentication
        dotnet add package Microsoft.AspNetCore.Http.Abstractions
        dotnet add package Microsoft.AspNetCore.Authentication.Cookies
        dotnet add package Microsoft.AspNetCore.Authentication.Facebook
        dotnet add package Microsoft.AspNetCore.Authentication.Google
        dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
        dotnet add package Microsoft.AspNetCore.Authentication.MicrosoftAccount
        dotnet add package Microsoft.AspNetCore.Authentication.oAuth
        dotnet add package Microsoft.AspNetCore.Authentication.OpenIDConnect
        dotnet add package Microsoft.AspNetCore.Authentication.Twitter

        dotnet add package MailKit
        dotnet add package MimeKit
    B2: Tạo model AppUser
    B3: Chỉnh sửa file AppDbContext cho nó kết thừa từ IdendityDbContext(AppUser)
    B4: Lưu All file, thực hiện migration 
    B5: Đăng ký dịch vụ bên Program.cs
        // Thêm chức năng người dùng, user....
        builder.Services.AddIdentity<AppUser, IdentityRole>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

    B6: Đăng lý dịch vụ dăng nhập bằng Facebook or google
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
                options.Lockout.MaxFailedAccessAttempts = 3; // Thất bại 5 lầ thì khóa
                options.Lockout.AllowedForNewUsers = true;

                // Cấu hình về User.
                options.User.AllowedUserNameCharacters = // các ký tự đặt tên user
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = true;  // Email là duy nhất

                // Cấu hình đăng nhập.
                options.SignIn.RequireConfirmedEmail = true;            // Cấu hình xác thực địa chỉ email (email phải tồn tại)
                options.SignIn.RequireConfirmedPhoneNumber = false;     // Xác thực số điện thoại
                options.SignIn.RequireConfirmedAccount = true;

            });
            builder.Services.ConfigureApplicationCookie(options => {
                options.LoginPath = "/Login/";
                options.LogoutPath = "/Logout/";
                options.AccessDeniedPath = "/khongduoctruycap.html";
            });

            // add đăng nhập google

            builder.Services.AddAuthentication()
                .AddGoogle(options => {
                    var gconfig = configuration.GetSection("Authentication:Google");
                    options.ClientId = gconfig["ClientId"];
                    options.ClientSecret = gconfig["ClientSecret"];
                    //http://localhost:5224/dang-nhap-tu-google
                    options.CallbackPath = "/dang-nhap-tu-google";

                })
                .AddFacebook(options => {
                    var fconfig = configuration.GetSection("Authentication:Facebook");
                    #pragma warning disable CS8601 // Possible null reference assignment.
                    options.AppId = fconfig["AppId"];
                    #pragma warning restore CS8601 // Possible null reference assignment.
                    options.AppSecret = fconfig["AppSecret"];
                    //http://localhost:5224/dang-nhap-tu-google
                    options.CallbackPath = "/dang-nhap-tu-facebook";

                });
            // Kiểm tra quyển để hiện thông tin manager
            builder.Services.AddAuthorization(options => {
                options.AddPolicy("ViewManageMenu", builders => {
                    builders.RequireAuthenticatedUser();
                    builders.RequireRole(RoleName.Administrator);
                });
            });
    B7: Thêm vào appsetting.json
          "MailSettings": {
            "Mail": "reviewphimtongtai@gmail.com",
            "DisplayName": "Vương Thanh Tuyền",
            "Password": "Tuyen2002@",
            "Host": "smtp.gmail.com",
            "Port": 587
          },
          "Authentication": {
            "Google": {
              "ClientId": "538518215501-rerfp2bhfm47huti1av8fjgk3a01cbmu.apps.googleusercontent.com",
              "ClientSecret": "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"
            },
            "Facebook": {
              "AppId": "895359945364613",
              "AppSecret": "fe146c7c706c438a0f0f3dab541dff97"
            }

         }
    B8: Xây dụng controler và view : copy here - https://xuanthulab.net/asp-net-core-mvc-tich-hop-entity-framework-va-identity.html
    B9: Nếu có thêm mới libman: gõ: libman restore
    
# Thêm Identity vào dự án
    B1: Thêm package

        dotnet add package System.Data.SqlClient
        dotnet add package Microsoft.EntityFrameworkCore
        dotnet add package Microsoft.EntityFrameworkCore.SqlServer
        dotnet add package Microsoft.EntityFrameworkCore.Design
        dotnet add package Microsoft.Extensions.DependencyInjection
        dotnet add package Microsoft.Extensions.Logging.Console

        dotnet add package Microsoft.AspNetCore.Identity
        dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore
        dotnet add package Microsoft.VisualStudio.Web.CodeGeneration.Design
        dotnet add package Microsoft.AspNetCore.Identity.UI
        dotnet add package Microsoft.AspNetCore.Authentication
        dotnet add package Microsoft.AspNetCore.Http.Abstractions
    B2: 
        - Atthenticatein: Xác định danh tính
        - Authorization: Xác thực quyền truy cập
        
        Đảm bảo tại file program.cs có: 
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

        Tạo một file AppUser.cs 
            
        Tại file AppDbContext, thêm kế thừa như sau: 
            public class AppDbContext : IdentityDbContext<AppUser>{}

        Đăng ký các dịch vụ của Identity User            
            builder.Services.AddIdentity<AppUser, IdentityRole>()
                    .AddEntityFrameworkStores<AppDbContext>()
                    .AddDefaultTokenProviders();
    B3: Gỡ tiền tố ASPNET 
        Tại AppDBContext: 
            foreach(var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var tableName = entityType.GetTableName();
                if (tableName.StartsWith("AspNet"))
                {
                    entityType.SetTableName(tableName.Substring(6));
                }
            }

    B4: Thiết lập cài đặng cho đăng nhập trong program.cs
       // Truy cập IdentityOptions
        services.Configure<IdentityOptions> (options => {
            // Thiết lập về Password
            options.Password.RequireDigit = false; // Không bắt phải có số
            options.Password.RequireLowercase = false; // Không bắt phải có chữ thường
            options.Password.RequireNonAlphanumeric = false; // Không bắt ký tự đặc biệt
            options.Password.RequireUppercase = false; // Không bắt buộc chữ in
            options.Password.RequiredLength = 3; // Số ký tự tối thiểu của password
            options.Password.RequiredUniqueChars = 1; // Số ký tự riêng biệt

            // Cấu hình Lockout - khóa user
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes (5); // Khóa 5 phút
            options.Lockout.MaxFailedAccessAttempts = 5; // Thất bại 5 lầ thì khóa
            options.Lockout.AllowedForNewUsers = true;

            // Cấu hình về User.
            options.User.AllowedUserNameCharacters = // các ký tự đặt tên user
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
            options.User.RequireUniqueEmail = true;  // Email là duy nhất

            // Cấu hình đăng nhập.
            options.SignIn.RequireConfirmedEmail = true;            // Cấu hình xác thực địa chỉ email (email phải tồn tại)
            options.SignIn.RequireConfirmedPhoneNumber = false;     // Xác thực số điện thoại

        });
    B5: tạo giao diện (copy từ file của xuanthulab)

    B6: 
        thêm cái này vào program.cs
                    // Kiểm tra quyển để hiện thông tin manager
                builder.Services.AddAuthorization(options => {
                    options.AddPolicy("ViewManageMenu", builders => {
                        builders.RequireAuthenticatedUser();
                        builders.RequireRole(RoleName.Administrator);
                    });
-
    # Seed data
        B1: Tại controller Data: 
            private readonly AppDbContext _dbContext;
            private readonly UserManager<AppUser> _userManager;
            private readonly RoleManager<IdentityRole> _roleManager;

            public DbManage(AppDbContext dbContext, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
            {
                _dbContext = dbContext;
                _userManager = userManager;
                _roleManager = roleManager;
            }

         ....
         ---
 # Tích hợp thư viện edit - summernote vào dự án
        B1: Thêm vào file libman.json
             {
                "library": "summernote@0.8.20",
                "destination": "wwwroot/lib/summernote"
              }

              --> Gõ libman restore tại terminal
        B2: 
            Nạp Script > Nạp Summernote> code edit
            - Nạp script lên đầu
            - Thêm file _summernote, thêm model Summernote
            _ Sử dụng: 
                <textarea id="summernoteEditor" name="editordata"></textarea>

                @{
                    var summenote = new App.Models.Summernote("#summernoteEditor", true);
                }

                <partial name="_Summenote" model="summenote" />
        B3: Xong rồi




