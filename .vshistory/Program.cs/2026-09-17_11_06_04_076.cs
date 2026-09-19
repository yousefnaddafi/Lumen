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
builder.Services.AddScoped<AuthenticationStateProvider, RevalidatingServerAuthenticationStateProvider>();
builder.Services.AddDependency();
//builder.Services.AddSettings(builder.Configuration);
// Add services to the container.
builder.Services.AddHttpClient();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
