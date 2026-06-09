using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BookSwap.Models;
using Microsoft.AspNetCore.Identity;

namespace BookSwap.ContextDBConfig
{
    public class BookSwapDBContext : IdentityDbContext<ApplicationUser>
    {
        public BookSwapDBContext(DbContextOptions<BookSwapDBContext> options) : base(options)
        {
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<LendHistory> LendHistories { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<BookComment> BookComments { get; set; }
        public DbSet<BookRating> BookRatings { get; set; } 

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

             modelBuilder.Entity<Book>()
                .HasOne(b => b.User)
                .WithMany(u => u.Books)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Cascade);

             modelBuilder.Entity<Book>()
                .HasMany(b => b.Comments)
                .WithOne(c => c.Book)
                .HasForeignKey(c => c.BookId)
                .OnDelete(DeleteBehavior.Cascade);

             modelBuilder.Entity<Chat>(entity =>
            {
                entity.HasOne(d => d.Sender)
                    .WithMany()
                    .HasForeignKey(d => d.SenderId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(d => d.Receiver)
                    .WithMany()
                    .HasForeignKey(d => d.ReceiverId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

             modelBuilder.Entity<LendHistory>()
                .HasMany(lh => lh.Ratings)
                .WithOne(r => r.LendHistory)
                .HasForeignKey(r => r.LendHistoryId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BookRating>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.NoAction);
        }

         public static class SeedData
        {
            public static async Task Initialize(IServiceProvider serviceProvider)
            {
                var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                string[] roles = { "Admin", "User" };
                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                    {
                        await roleManager.CreateAsync(new IdentityRole(role));
                    }
                }

                var adminEmail = "admin@example.com";
                var adminUser = await userManager.FindByEmailAsync(adminEmail);
                if (adminUser == null)
                {
                    var user = new ApplicationUser
                    {
                        UserName = adminEmail,
                        Email = adminEmail,
                        Name = "Admin",
                        Address = "Admin Address",
                        ImagePath = "images/carouselimages/p.jpg"
                    };

                    var result = await userManager.CreateAsync(user, "Admin@123");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, "Admin");
                    }
                }

                var userEmail = "user@example.com";
                var normalUser = await userManager.FindByEmailAsync(userEmail);
                if (normalUser == null)
                {
                    var user = new ApplicationUser
                    {
                        UserName = userEmail,
                        Email = userEmail,
                        Name = "User",
                        Address = "User Address"
                    };

                    var result = await userManager.CreateAsync(user, "User@123");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, "User");
                    }
                }
            }
        }
    }
}