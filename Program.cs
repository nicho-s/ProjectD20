using GameForum.Models;
using GameForum.Repositories.Abstract;
using GameForum.Repositories.Implementation;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Serilog;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ForumDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ForumDBContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options => options.LoginPath = "/UserAuth/Login");

builder.Services.AddScoped<IUserAuthentificationService, UserAuthenticationService>();

builder.Services
    .AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services
    .AddControllersWithViews()
    .AddViewLocalization()
    .AddDataAnnotationsLocalization();

Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).Enrich
    .FromLogContext().WriteTo
    .File("Logs/log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    const string defaultCulture = "uk";
    var supportedCultures = new[]
    {
        new CultureInfo(defaultCulture),
        new CultureInfo("en"),
    };

    options.DefaultRequestCulture = new RequestCulture(defaultCulture);
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.Use((context, next) =>
{
    Log.Information($"{context.Request.Scheme}://{context.Request.Host}{context.Request.Path}{context.Request.QueryString} [:] " +
                    $"T:{DateTime.Now}, " +
                    $"IP:{context.Connection.RemoteIpAddress}");
    return next();
});

app.UseRequestLocalization(app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Forum}/{action=Main}/{id?}");

// Seed the roles and admin user
using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    ForumDBContext.SeedRolesAndAdminAsync(serviceProvider).Wait();
}

app.Run();