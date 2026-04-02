using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OstaFeedbackApp.Data;
using OstaFeedbackApp.Hubs;

var builder = WebApplication.CreateBuilder(args);

// --------------------
// DATABASE
// --------------------
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// --------------------
// IDENTITY
// --------------------
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    // Password Policy
    options.Password.RequireDigit = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;

    // Lockout
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User settings
    options.User.RequireUniqueEmail = true;

})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// --------------------
// COOKIE SETTINGS
// --------------------
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/AdminLogin";
    options.AccessDeniedPath = "/Account/AdminLogin";
    options.Cookie.Name = "OstaAdminAuth";
    options.ExpireTimeSpan = TimeSpan.FromHours(1);
    options.SlidingExpiration = true;
});

// --------------------
// MVC + Razor Pages
// --------------------
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// --------------------
// SIGNALR
// --------------------
builder.Services.AddSignalR();

// --------------------
// BUILD APP
// --------------------
var app = builder.Build();

// --------------------
// APPLY DATABASE MIGRATIONS
// --------------------
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

// --------------------
// ROLE SEEDING (Admin/Manager/Viewer)
// --------------------
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    string[] roles = { "Admin", "Manager", "Viewer" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
}

// --------------------
// MIDDLEWARE
// --------------------
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

// --------------------
// SIGNALR HUB
// --------------------
app.MapHub<FeedbackHub>("/feedbackHub");

// --------------------
// ROUTES
// --------------------
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
