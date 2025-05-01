
using ASPAssignment.Hubs;
using ASPAssignment.Services;
using Business.Interface;
using Business.Services;
using Data.Contexts;
using Data.Entities;
using Data.Helpers;
using Data.Interface;
using Data.Repository;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ===== Databas & DI =====
builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AlphaDb")));

builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IMemberService, MemberService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<ITagService, TagService>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

// ===== MVC + Identity =====
builder.Services.AddControllersWithViews();

builder.Services.AddIdentity<MemberEntity, ApplicationRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.User.RequireUniqueEmail = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
})
.AddEntityFrameworkStores<DataContext>();

// Claims-transformering för IsAppAdmin
builder.Services.AddScoped<IClaimsTransformation, AdminClaimsTransformer>();

// Policyn som kräver admin-claim
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAppAdmin", policy =>
        policy.RequireClaim("IsAppAdmin", "true"));
});

// Konfigurera Identity-cookien EN GÅNG
builder.Services.ConfigureApplicationCookie(options =>
{
    // Standardlogin för vanliga användare
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromDays(14);
    options.SlidingExpiration = true;
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.Name = "MyAppAuth";

    // Särskilj inloggningsväg för /Admin
    options.Events.OnRedirectToLogin = ctx =>
    {
        // Skapa samma typ för båda vägarna
        var returnUrl = ctx.Request.Path + ctx.Request.QueryString;
        PathString targetLogin = ctx.Request.Path.StartsWithSegments("/Admin")
            ? new PathString("/Admin/Login")
            : options.LoginPath;

        // Bygg redirect-URL som string
        var redirectUri = targetLogin.Value
                          + "?returnUrl="
                          + Uri.EscapeDataString(returnUrl);

        ctx.Response.Redirect(redirectUri);
        return Task.CompletedTask;
    };
});

// ===== Externa inloggningsleverantörer =====
builder.Services.AddAuthentication()
    .AddGoogle(opts =>
    {
        opts.ClientId = builder.Configuration["Authentication:Google:ClientId"]!;
        opts.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]!;
        opts.Events.OnRemoteFailure = ctx =>
        {
            ctx.HandleResponse();
            var returnUrl = ctx.Properties.RedirectUri ?? "/";
            var redirect = $"/External/ExternalSignIn?returnUrl={Uri.EscapeDataString(returnUrl)}" +
                           $"&error={Uri.EscapeDataString(ctx.Failure?.Message ?? "External login failed")}";
            ctx.Response.Redirect(redirect);
            return Task.CompletedTask;
        };

    })
    .AddFacebook(opts =>
    {
        opts.AppId = builder.Configuration["Authentication:Facebook:AppId"]!;
        opts.AppSecret = builder.Configuration["Authentication:Facebook:AppSecret"]!;
        opts.Events.OnRemoteFailure = ctx =>
        {
            ctx.HandleResponse();
            var returnUrl = ctx.Properties.RedirectUri ?? "/";
            var redirect = $"/External/ExternalSignIn?returnUrl={Uri.EscapeDataString(returnUrl)}" +
                           $"&error={Uri.EscapeDataString(ctx.Failure?.Message ?? "External login failed")}";
            ctx.Response.Redirect(redirect);
            return Task.CompletedTask;
        };
    })
    .AddGitHub(opts =>
    {
        opts.ClientId = builder.Configuration["Authentication:GitHub:ClientId"]!;
        opts.ClientSecret = builder.Configuration["Authentication:GitHub:ClientSecret"]!;
        opts.Events.OnRemoteFailure = ctx =>
        {
            ctx.HandleResponse();
            var returnUrl = ctx.Properties.RedirectUri ?? "/";
            var redirect = $"/External/ExternalSignIn?returnUrl={Uri.EscapeDataString(returnUrl)}" +
                           $"&error={Uri.EscapeDataString(ctx.Failure?.Message ?? "External login failed")}";
            ctx.Response.Redirect(redirect);
            return Task.CompletedTask;
        };
        opts.Scope.Add("user:email");
    })
    .AddMicrosoftAccount(opts =>
    {
        opts.ClientId = builder.Configuration["Authentication:Microsoft:ClientId"]!;
        opts.ClientSecret = builder.Configuration["Authentication:Microsoft:ClientSecret"]!;
        opts.Events.OnRemoteFailure = ctx =>
        {
            ctx.HandleResponse();
            var returnUrl = ctx.Properties.RedirectUri ?? "/";
            var redirect = $"/External/ExternalSignIn?returnUrl={Uri.EscapeDataString(returnUrl)}" +
                           $"&error={Uri.EscapeDataString(ctx.Failure?.Message ?? "External login failed")}";
            ctx.Response.Redirect(redirect);
            return Task.CompletedTask;
        };
    });

// ===== SignalR & Notifieringar =====
builder.Services.AddSignalR();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddSingleton<IUserIdProvider, EmailBasedUserIdProvider>();

var app = builder.Build();

// ===== Middleware =====
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

// Seed roller vid start
using (var scope = app.Services.CreateScope())
{
    await IdentitySeeder.SeedRoles(scope.ServiceProvider);
}

// ===== Routing =====
// Admin-route (måste deklareras före default)
app.MapControllerRoute(
    name: "admin",
    pattern: "Admin/{action=Login}/{id?}",
    defaults: new { controller = "Admin" }
);

// Default-route (Account/Login som startsida)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);
// SignalR-hubbar
app.MapHub<Chathub>("/chathub");
app.MapHub<NotificationHub>("/notificationHub");

app.Run();
