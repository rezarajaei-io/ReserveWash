using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ReserveWash.Middlewares;
using ReserveWash.Repository.Services;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddControllersWithViews();


builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
    });

builder.Services.AddDbContext<AppDbContext>(options =>
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    });

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();


#region Dependency Injection ( DI )
//builder.Services.AddScoped<IUserService, UserService>(); // Scoped: یک نمونه برای هر درخواست
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>)); // Scoped: یک نمونه برای هر درخواست
builder.Services.AddTransient<ReserveTimeService>();
builder.Services.AddScoped<CarwashService>();
builder.Services.AddScoped<CarService>();
builder.Services.AddScoped<ReservationService>();
builder.Services.AddScoped<ServiceRepository>();
#endregion
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.UseMiddleware<PersianCultureMiddleware>();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.Run();
