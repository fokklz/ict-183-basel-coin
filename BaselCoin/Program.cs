using BaselCoin.Middlewares;
using BaselCoin2.Data;
using BaselCoin2.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace BaselCoin2
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Setup Custom logging using Serilog
            // will rotate the logfile
            var LoggerFromSettings = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
            .Enrich.FromLogContext()
                .CreateLogger();
            builder.Logging.ClearProviders();
            builder.Logging.AddSerilog(LoggerFromSettings);

            /*builder.Services.Configure<RouteOptions>(options =>
            {
                options.LowercaseUrls = true;
                options.LowercaseQueryStrings = false;
                options.AppendTrailingSlash = true;
            });*/

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<ApplicationDBContext>(options =>
                options.UseSqlServer(connectionString).UseLazyLoadingProxies());
                

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.SlidingExpiration = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
                options.Cookie.HttpOnly = true;
            });

            builder.Services.AddAutoMapper(typeof(Program).Assembly);

            // Add Identity and specify options if necessary
            builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
            {
                // Password settings
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = false;
                options.Password.RequiredUniqueChars = 3;

                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // User settings
                options.User.RequireUniqueEmail = false;

                // Sign-in settings
                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedPhoneNumber = false;
            }).AddRoles<IdentityRole>() // Add roles to the identity
                .AddEntityFrameworkStores<ApplicationDBContext>()
                .AddDefaultUI();

            // Add services to the container.
            builder.Services.AddRazorPages();

            var app = builder.Build();
            app.UseAntiforgery();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseMiddleware<SerilogEnrichingMiddleware>();

            app.UseWhen(context => context.Request.Path.StartsWithSegments("/admin"), appBuilder =>
            {
                appBuilder.UseMiddleware<IpSecurityMiddleware>();
            });

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapRazorPages();
            app.MapDefaultControllerRoute();

            SeedDatabase(app).Wait();

            app.Run();
        }

        public static async Task SeedDatabase(WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;

                var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var context = serviceProvider.GetRequiredService<ApplicationDBContext>();

                // Seed roles
                string[] roleNames = { "Admin", "User" };
                foreach (var roleName in roleNames)
                {
                    var roleExist = await roleManager.RoleExistsAsync(roleName);
                    if (!roleExist)
                    {
                        await roleManager.CreateAsync(new IdentityRole(roleName));
                    }
                }


                // Seed default user
                var defaultUser = new ApplicationUser { UserName = "admin@example.com", Email = "admin@example.com" };
                if (userManager.Users.All(u => u.UserName != defaultUser.UserName))
                {
                    await userManager.CreateAsync(defaultUser, "YourSecureP@ssw0rd!");
                    await userManager.AddToRoleAsync(defaultUser, roleNames[0]);
                    await userManager.ConfirmEmailAsync(defaultUser, await userManager.GenerateEmailConfirmationTokenAsync(defaultUser));

                    await context.AddAsync(new Balance { UserId = defaultUser.Id, Amount = 1000 });
                    await context.SaveChangesAsync();
                }
            }
        }

    }
}
