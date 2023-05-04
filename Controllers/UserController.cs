

using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mvc_pruebas.Models;

public class UserController : Controller
{
    private readonly MeethubdbContext _context;

    public UserController(MeethubdbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Login()
    {
        ClaimsPrincipal c = HttpContext.User;
        if (c.Identity != null)
        {
            if (c.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "User");
            }
        }
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(string email, string password)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email && u.Password == password);

        try
        {
            if (user != null)
            {
                // Autenticación exitosa, redirigir a la vista deseada

                List<Claim> claims = new List<Claim>(){
                    new Claim(ClaimTypes.Name, user.Email),
                };
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);
                AuthenticationProperties p = new();
                p.AllowRefresh = true;
                p.IsPersistent = user.MantenerSesion;

                if (!user.MantenerSesion)
                {
                    p.ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(1);
                }
                else
                {
                    p.ExpiresUtc = DateTimeOffset.UtcNow.AddDays(1);
                }

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, p);
                return RedirectToAction("Index", "Home");
            }
            Console.WriteLine("Credenciales inválidas");
            return View();
        }
        catch (System.Exception e)
        {
            return View();
        }


    }
}