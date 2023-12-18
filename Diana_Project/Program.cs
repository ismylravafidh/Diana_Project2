var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddIdentity<AppUser,IdentityRole>(opt =>
{
	opt.Password.RequiredLength = 8;
	opt.Password.RequireNonAlphanumeric = true;
	opt.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789._";
	opt.Lockout.MaxFailedAccessAttempts = 5;
	opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(2);
}).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();


builder.Services.AddDbContext<AppDbContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
});


var app = builder.Build();
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
    );
app.MapControllerRoute(
    name: "Default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
    );

app.UseStaticFiles();
app.Run();