using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using meethub.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<MeethubdbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("Connection"), Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.33-mysql")));

builder.Services.AddSession(options =>
{
    options.Cookie.Name = ".AspNetCore.User";
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = ".AspNetCore.Session";
        options.LoginPath = "/meethub/User/Login";
        options.LogoutPath = "/meethub/Home/Logout";
        options.SlidingExpiration = true;
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/meethub/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseSession();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Redirigir al usuario a home en caso de tener sesion iniciada
app.Use(async (context, next) =>
{
    if (context.Request.Path == "/")
    {
        var isAuthenticated = context.User.Identity.IsAuthenticated;
        if (isAuthenticated)
        {
            context.Response.Redirect("/meethub/Home/Index");
            return;
        }
    }

    await next();
});

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=User}/{action=Login}/{id?}");
    
    endpoints.MapControllerRoute(
        name: "event",
        pattern: "/meethub/Home/Event/{id?}",
        defaults: new { controller = "Home", action = "Event" }
    );
});
app.MapControllers();

app.Run();