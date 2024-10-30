using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AnnounceHub.Data
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {   }
        public DbSet<Announcement> Announcements { get; set; }
        public new DbSet<User>  Users  { get; set; } 
    }

    public class Announcement
    {
        public int Id { get; set; }
        public string? Message { get; set; }
        public DateTime CreatedAt { get; set; }
    }
    public  class User : IdentityUser
    {
        //override public string Id { get; set; }
        //override public string? UserName { get; set; }
        public DateTime Joined { get; set; }
    }
    public class LoginViewModel
    {
        public string? UserName { get; set; }
        //public string Email { get; set; }
        public string? Password { get; set; }
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? ConfirmPassword { get; set; }
    }


    internal class DbInitializer
    {
        internal static async Task Initialize(AppDbContext context, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            await context.Database.EnsureCreatedAsync();
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }
            var adminUser = await userManager.FindByNameAsync("Intitech");
            if (adminUser == null)
            {
                // Create the admin user
                var user = new User()
                {
                    UserName = "Intitech",
                    Email = "admin@example.com",
                    Joined = DateTime.Now,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, "Admin@123"); // Sets password securely
                //if (result.IsCompletedSuccessfully)
                //{
                    await userManager.AddToRoleAsync(user, "Admin");
                //}
                await context.SaveChangesAsync();
            }
        }
    }
}
