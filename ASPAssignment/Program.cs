
using Azure.Identity;
using Business.Interface;
using Business.Services;
using Data.Contexts;
using Data.Entities;
using Data.Interface;
using Data.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<DataContext>(x => x.UseSqlServer(builder.Configuration.GetConnectionString("AlphaDb")));
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IMemberService, MemberService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddControllersWithViews();
builder.Services.AddIdentity<MemberEntity, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.User.RequireUniqueEmail = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;

})
    .AddEntityFrameworkStores<DataContext>();


builder.Services.ConfigureApplicationCookie(options =>  
{
    options.LoginPath = "/Account/Login";
    options.SlidingExpiration = true;
});






var app = builder.Build();
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

app.UseAuthorization();

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    string[] roleNames = { "Admin", "User" };

    foreach (var roleName in roleNames)
    {
        var roleExist = await roleManager.RoleExistsAsync(roleName);
        if (!roleExist)
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<MemberEntity>>();
    var user = new MemberEntity { UserName = "admin@admin.com", Email = "admin@admin.com" };

    var userExist = await userManager.Users.AnyAsync(x => x.Email == user.Email);
    if (!userExist)
    {
        var result = await userManager.CreateAsync(user, "Admin1234!");
        if (result.Succeeded)
            await userManager.AddToRoleAsync(user, "Admin");
    }
}





app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Account}/{action=Login}/{id?}");


app.Run();
