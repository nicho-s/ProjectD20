using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Lab4_5.ViewModels;

namespace GameForum.Models
{
    public class ForumDBContext : IdentityDbContext<ApplicationUser>
    {
        public ForumDBContext(DbContextOptions<ForumDBContext> options) : base(options) 
        {
            Database.EnsureCreated();
        }
        
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Topic> Topics { get; set; }
        public DbSet<ApplicationUser> Users {  get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Review>(rw =>
            {
                rw.HasOne(m => m.Topic)
                .WithMany(t => t.Reviews)
                .OnDelete(DeleteBehavior.Cascade);

                rw.HasOne(m => m.User)
                .WithMany()
                .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<Topic>(t =>
            {
                t.HasMany(t => t.Reviews)
                .WithOne(rw => rw.Topic);

                t.HasOne(t => t.User)
                .WithMany()
                .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<ApplicationUser>(u =>
            {
                u.HasMany(u => u.Reviews)
                .WithOne(rw => rw.User)
                .OnDelete(DeleteBehavior.NoAction);

                u.HasMany(u => u.Topics)
                .WithOne(t => t.User)
                .OnDelete(DeleteBehavior.NoAction);
            });
        }

        public enum Roles
        {
            Admin,
            User
        }

        public static async Task SeedRolesAndAdminAsync(IServiceProvider service)
        {
            //Seed Roles
            var userManager = service.GetService<UserManager<ApplicationUser>>();
            var roleManager = service.GetService<RoleManager<IdentityRole>>();
            await roleManager.CreateAsync(new IdentityRole(Roles.Admin.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.User.ToString()));

            // creating admin

            var user = new ApplicationUser
            {
                UserName = "admin@gmail.com",
                Email = "admin@gmail.com",
                Description = "Official admin account",
                BirthDay = DateTime.UtcNow,
                Name = "TrueAdmin",
                Sex = "Knight",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true
            };

            var userInDb = await userManager.FindByEmailAsync(user.Email);
            if (userInDb == null)
            {
                await userManager.CreateAsync(user, "Admin100@@");
                await userManager.AddToRoleAsync(user, Roles.Admin.ToString());
            }
        }

        public DbSet<Lab4_5.ViewModels.UserViewModel> UserViewModel { get; set; } = default!;

        public DbSet<Lab4_5.ViewModels.UpdUserViewModel> UpdUserViewModel { get; set; } = default!;
    }
}
