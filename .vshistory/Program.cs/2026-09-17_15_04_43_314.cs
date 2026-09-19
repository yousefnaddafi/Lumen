using Lumen.Components;
using Lumen.Extensions;
using Lumen.InfraStructure;
using Lumen.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSignalR();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddDbContext<ProjectDBContext>(o =>
{ o.UseSqlServer(builder.Configuration.GetConnectionString("Lumen")); });
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddSession();
builder.Services.AddMudServices();
// --- Identity core ---
builder.Services
    .AddIdentity<AppUser, AppRole>(options =>
    {
        options.Password.RequiredLength = 6;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireDigit = false;
        options.User.RequireUniqueEmail = false;   // you may not have emails for staff
        options.SignIn.RequireConfirmedAccount = false;
    })
    .AddEntityFrameworkStores<ProjectDBContext>()
    .AddDefaultTokenProviders();

// --- Cookie auth for the Blazor circuit ---
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/admin/login";
    options.AccessDeniedPath = "/admin/login";
    options.ExpireTimeSpan = TimeSpan.FromDays(7);
    options.SlidingExpiration = true;
    options.Cookie.Name = "Lumen.Auth";
});

// --- Blazor needs these two, in this order ---
builder.Services.AddCascadingAuthenticationState();
//builder.Services.AddScoped<AuthenticationStateProvider, RevalidatingServerAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider, ServerAuthenticationStateProvider>();
builder.Services.AddDependency();
//builder.Services.AddSettings(builder.Configuration);
// Add services to the container.
builder.Services.AddHttpClient();
var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ProjectDBContext>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();

    await db.Database.MigrateAsync();

    // Ensure roles exist (in case migrations haven't seeded them yet)
    foreach (var role in new[] { "Owner", "BranchManager", "Cashier", "Barista", "Accountant", "MenuAdmin" })
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new AppRole(role));
    }

    // Create admin if missing
    const string adminUserName = "admin";
    const string adminPassword = "LumenCafe2026";

    var admin = await userManager.FindByNameAsync(adminUserName);
    if (admin == null)
    {
        admin = new AppUser
        {
            UserName = adminUserName,
            FullName = "مدیر سیستم",
            IsActive = true,
            CreateDate = DateTime.UtcNow
        };

        var createResult = await userManager.CreateAsync(admin, adminPassword);
        if (!createResult.Succeeded)
        {
            foreach (var err in createResult.Errors)
                Console.WriteLine($"Admin seed error: {err.Code} - {err.Description}");
        }
        else
        {
            await userManager.AddToRoleAsync(admin, "Owner");
        }
    }
}
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();      // static files first (no auth needed)
app.UseAuthentication();   // who are you?
app.UseAuthorization();    // are you allowed?
app.UseAntiforgery();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapPost("/admin/login-handler", async (
    HttpContext context,
    SignInManager<AppUser> signInManager) =>
{
    var form = await context.Request.ReadFormAsync();
    var username = form["username"].ToString();
    var password = form["password"].ToString();

    var result = await signInManager.PasswordSignInAsync(
        username, password, isPersistent: true, lockoutOnFailure: true);

    if (result.Succeeded)
    {
        context.Response.Redirect("/admin/products");
    }
    else
    {
        context.Response.Redirect("/admin/login?error=invalid");
    }
});
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();
using (var scope = app.Services.CreateScope())
{
    var sp = scope.ServiceProvider;
    var db = sp.GetRequiredService<ProjectDBContext>();
    var userManager = sp.GetRequiredService<UserManager<AppUser>>();
    var roleManager = sp.GetRequiredService<RoleManager<AppRole>>();

    await db.Database.MigrateAsync();

    foreach (var role in new[] { "Owner", "BranchManager", "Cashier", "Barista", "Accountant", "MenuAdmin" })
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new AppRole(role));

    if (await userManager.FindByNameAsync("admin") == null)
    {
        var admin = new AppUser
        {
            UserName = "admin",
            FullName = "مدیر سیستم",
            IsActive = true,
            CreateDate = DateTime.UtcNow
        };
        var result = await userManager.CreateAsync(admin, "LumenCafe2026");
        if (result.Succeeded)
            await userManager.AddToRoleAsync(admin, "Owner");
    }
}
app.Run();
