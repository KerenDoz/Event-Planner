using EventPlanner.Common;
using EventPlanner.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Connection string (from appsettings.json)
var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// EF Core + SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString)
);

// Identity (cookie auth)
// builder
//     .Services.AddDefaultIdentity<ApplicationUser>(options =>
//     {
//         options.SignIn.RequireConfirmedAccount = false;

//         // Password rules
//         options.Password.RequireDigit = false;
//         options.Password.RequireLowercase = false;
//         options.Password.RequireUppercase = false;
//         options.Password.RequireNonAlphanumeric = false;
//         options.Password.RequiredLength = 6;
//     })
//     .AddRoles<IdentityRole>()
//     .AddEntityFrameworkStores<ApplicationDbContext>();

// MVC + Identity UI pages
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Lockout.AllowedForNewUsers = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var dbContext = services.GetRequiredService<ApplicationDbContext>();
    await dbContext.Database.MigrateAsync();

    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

    string[] roles = { RoleConstants.User, RoleConstants.Administrator, RoleConstants.Developer };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    string adminEmail = "admin@eventplanner.com";
    string adminUsername = "admin";
    string adminPassword = "Admin123!";

    var adminUser = await userManager.FindByEmailAsync(adminEmail);

    if (adminUser == null)
    {
        adminUser = new ApplicationUser
        {
            UserName = adminUsername,
            Email = adminEmail,
            EmailConfirmed = true,
        };

        var result = await userManager.CreateAsync(adminUser, adminPassword);

        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, RoleConstants.Administrator);
        }
    }
    else
    {
        if (!await userManager.IsInRoleAsync(adminUser, RoleConstants.Administrator))
        {
            await userManager.AddToRoleAsync(adminUser, RoleConstants.Administrator);
        }
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error/500");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseStatusCodePagesWithReExecute("/Error/{0}");

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages(); // enables /Identity/Account/Login, Register, etc.

await DbSeeder.SeedAsync(app.Services);

app.Run();
