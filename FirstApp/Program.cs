using FirstApp.Interfaces;
using FirstApp.Models;
using FirstApp.Service;
using FirstApp.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebSockets;
using Microsoft.EntityFrameworkCore;
using System.Configuration;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<AppIdentityDbContext>(options => options.UseSqlServer(builder.Configuration["ConnectionStrings:IdentityDb"]));
builder.Services.AddDbContext<CatalogContext>(options => options.UseSqlServer(builder.Configuration["ConnectionStrings:EshopDb"]));
builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<AppIdentityDbContext>().AddDefaultTokenProviders();



builder.Services.Configure<DataProtectionTokenProviderOptions>(opts => opts.TokenLifespan = TimeSpan.FromHours(10));

//Add core service
builder.Services.AddScoped(typeof(IReadRepository<>), typeof(EfRepository<>));
builder.Services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));

//builder.Services.AddScoped<IBasketService, BasketService>();
//builder.Services.AddScoped<IOrderService, OrderService>();
//builder.Services.AddScoped<IBasketQueryService, BasketQueryService>();
//var catalogSettings = configuration.Get<CatalogSettings>() ?? new CatalogSettings();
builder.Services.AddSingleton<IUriComposer>(new UriComposer(new CatalogSettings()));

//builder.Services.AddScoped(typeof(IAppLogger<>), typeof(LoggerAdapter<>));
//builder.Services.AddTransient<IEmailSender, EmailSender>();

//Add web service
builder.Services.AddScoped<ICatalogItemViewModelService, CatalogItemViewModelService>();
builder.Services.AddScoped<ICatalogViewModelService, CatalogViewModelService>();


builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.Name = ".AspNetCore.Identity.Application";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
    options.SlidingExpiration = true;
});

//builder.Services.AddAuthorization(opts =>
//{
//    opts.AddPolicy("AspManager", policy =>
//    {
//        policy.RequireRole("Manager");
//        policy.RequireClaim("Coding-Skill", "ASP.NET Core MVC");
//    });
//});

builder.Services.AddAuthentication("Cookies").AddCookie(options =>
{
    options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
    options.SlidingExpiration = true;
    options.AccessDeniedPath = "/Account/AccessDenied.cshtml";
});

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(10);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();


// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

app.Logger.LogInformation("App created...");

app.Logger.LogInformation("Seeding Database...");

using (var scope = app.Services.CreateScope())
{
    var scopedProvider = scope.ServiceProvider;
    try
    {
        var catalogContext = scopedProvider.GetRequiredService<CatalogContext>();
        await CatalogContextSeed.SeedAsync(catalogContext, app.Logger);
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "An error occurred seeding the DB.");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

//app.UseSession();
app.UseCookiePolicy();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Catalog}/{action=Index}/{id?}");

app.Run();
