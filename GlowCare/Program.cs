using GlowCare.Entities;
using GlowCare.Entities.Contracts.Interfaces;
using GlowCare.Entities.Models;
using GlowCare.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GlowCare;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        builder.Services.AddDbContext<GlowCareDbContext>(options =>
            options.UseSqlServer(connectionString));
        builder.Services.AddDatabaseDeveloperPageExceptionFilter();

        builder.Services.AddDefaultIdentity<GlowUser>(options =>
        {
            options.SignIn.RequireConfirmedAccount = false;
        })
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<GlowCareDbContext>()
            .AddDefaultTokenProviders();

        builder.Services
                .RegisterRepositories()
                .RegisterUserDefinedServices();


        builder.Services.AddRazorPages();
        builder.Services
            .AddControllersWithViews()
            .AddRazorRuntimeCompilation();


        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            var dbContext = services.GetRequiredService<GlowCareDbContext>();

            try
            {
                dbContext.Database.Migrate();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while migrating the database: {ex.Message}");
            }

            var roleManager = services.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
            var userManager = services.GetRequiredService<UserManager<GlowUser>>();
            var userRepository = services.GetRequiredService<IRepository<GlowUser, Guid>>();
            await AssignRoles(userManager, roleManager, userRepository);
        }

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseMigrationsEndPoint();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller=AdminPanel}/{action=Index}/{id?}");

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");
        app.MapRazorPages();

        app.Run();
    }

    public static async Task AssignRoles(UserManager<GlowUser> userManager, RoleManager<IdentityRole<Guid>> roleManager, IRepository<GlowUser, Guid> userRepository)
    {
        var users = await userRepository.GetAllAsync();

        foreach (var user in users)
        {
            if (user.Id.ToString() == "c9f4e7b1-2d33-4a11-8f56-abcdef123456" ||
                user.Id.ToString() == "e5c2a9b3-4a67-4f89-8d23-556677889900")
            {
                await userManager.AddToRoleAsync(user, "User");
            }

            if (user.Id.ToString() == "a7d3c5e2-9b41-4f12-8f34-123456789abc" ||
                user.Id.ToString() == "ac31b0bb-d05a-438d-be06-9bfe3323cf08" ||
                user.Id.ToString() == "29965aaa-46cf-4829-93b8-e38401be7547" ||
                user.Id.ToString() == "c9f4e7b1-2d33-4a11-8f56-abcdef123456")
            {
                await userManager.AddToRoleAsync(user, "Specialist");
            }

            else if (user.Id.ToString() == "fc95b3fa-f342-4172-ac8b-5b35951ad760")
            {
                await userManager.AddToRoleAsync(user, "Admin");
            }

        }
    }
}

