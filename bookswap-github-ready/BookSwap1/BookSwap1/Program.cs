using BookSwap.ContextDBConfig;
using BookSwap.Models;
using BookSwap.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using static BookSwap.ContextDBConfig.BookSwapDBContext;
 namespace BookSwap
{
    public class Program
    {

        public static async Task Main(string[] args)  
        {

            var builder = WebApplication.CreateBuilder(args);

            builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));

            builder.Services.AddTransient<EmailSender>();
            


            var dbConnection = builder.Configuration.GetConnectionString("dbConnection");

             builder.Services.AddControllersWithViews();


             builder.Services.AddDbContext<BookSwapDBContext>(options => options.UseSqlServer(dbConnection));

             builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<BookSwapDBContext>()
                .AddDefaultTokenProviders();

            var app = builder.Build();

             if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();  
            app.UseAuthorization();  

             using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                     await SeedData.Initialize(services);
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "ÕœÀ Œÿ√ √À‰«¡  ÂÌ∆… «·»Ì«‰«  «·√Ê·Ì….");
                }
            }

             app.MapControllerRoute(
                name: "profile",
                pattern: "Account/Profile/{userId?}",
                defaults: new { controller = "Account", action = "Profile" });

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{userId?}");

            app.Run();  
        }

        public static async Task Initialize(IServiceProvider serviceProvider, UserManager<ApplicationUser> userManager)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            string[] roleNames = { "Admin", "User" };
            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }

    }
}
