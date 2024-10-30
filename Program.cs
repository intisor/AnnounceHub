using AnnounceHub.AppHub;
using AnnounceHub.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


builder.Services.AddDbContext<AppDbContext>(options =>
                                            options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
                                            new MySqlServerVersion(new Version(8, 0, 21))));
builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthorization();
//builder.Services.AddAuthentication(IdentityConstants.ApplicationScheme).AddCookie();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
               .AddCookie(config =>
               {
                   config.LoginPath = "/home/login";
                   config.Cookie.Name = "Announce";
                   config.ExpireTimeSpan = TimeSpan.FromDays(1);
                   config.AccessDeniedPath = "/home/privacy";
                   config.SlidingExpiration = true;
               });
builder.Services.AddSignalR();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContext>();
    var userManager = services.GetRequiredService<UserManager<User>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

    await DbInitializer.Initialize(context, userManager, roleManager);
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapHub<AnnouncementHub>("/announcementhub");
app.Run();
