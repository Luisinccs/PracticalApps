// Section 1 - Impotrs namespaces
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Northwind.Mvc.Data;
using Packt.Shared;
using System.Net.Http.Headers;

// Section 2 - Configures the host web server including services
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Registers an appliction database context using SQLite. The database connection string is loaded from the appsettings.json file.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
// adds ASP.NET Core Identifynfor authentication and configures it to use the application database
builder.Services.AddDefaultIdentity<IdentityUser>(
    options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
// Adds support for MVC controllers with views
builder.Services.AddControllersWithViews();
builder.Services.AddNorthwindContext();
builder.Services.AddOutputCache(options => {
    options.DefaultExpirationTimeSpan = TimeSpan.FromSeconds(10);
});
builder.Services.AddHttpClient(name: "Norhtwind.WebApi", 
configureClient: options => {
    options.BaseAddress = new Uri("http://localhost:5002");
    options.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(
        mediaType: "application/json", quality:1.0));
});
var app = builder.Build();

// Section 3 Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.UseMigrationsEndPoint();
} else {
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseOutputCache();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.MapGet("/notcached", () => DateTime.Now.ToString());
app.MapGet("/cached", () => DateTime.Now.ToString()).CacheOutput();

// Section 4 - start the host web server listening for HTTP requests
app.Run();

