using GameForum.Models;
using GameForum.Repositories.Abstract;
using GameForum.Repositories.Implementation;
using Lab4_5;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services.AddControllersWithViews()
    .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
    .AddDataAnnotationsLocalization(options => {
        options.DataAnnotationLocalizerProvider = (type, factory) =>
            factory.Create(typeof(SharedResource));
    });

builder.Services.AddDbContext<ForumDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ForumDBContext>()
    .AddDefaultTokenProviders();
builder.Services.ConfigureApplicationCookie(options => options.LoginPath = "/UserAuth/Login");
builder.Services.AddScoped<IUserAuthentificationService, UserAuthenticationService>();

builder.Services.Configure<RequestLocalizationOptions>(options => 
{ 
    var supportedCultures = new[] 
    { 
        new CultureInfo("en"), 
        new CultureInfo("uk"), 
    }; 
        options.DefaultRequestCulture = new RequestCulture("uk"); 
        options.SupportedCultures = supportedCultures; 
        options.SupportedUICultures = supportedCultures; 
    });

var app = builder.Build();

app.UseRequestLocalization();

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