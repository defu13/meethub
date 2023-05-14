

using System.Security.Claims;
using System.Text.Json;
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
        // ClaimsPrincipal c = HttpContext.User;
        // if (c.Identity != null)
        // {
        //     if (c.Identity.IsAuthenticated)
        //     {
        //         return RedirectToAction("Login", "User");
        //     }
        // }
        return View();
    }

    [HttpPost]
    public IActionResult DeleteTempData(string key)
    {
        TempData.Remove(key);
        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> Login(User user)
    {
        var usuario = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email && u.Password == user.Password);

        try
        {
            if (usuario != null)
            {
                // Autenticación exitosa, redirigir a la vista deseada

                List<Claim> claims = new List<Claim>(){
                    new Claim(ClaimTypes.Name, usuario.Email),
                };
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);
                AuthenticationProperties p = new();
                p.AllowRefresh = true;
                p.IsPersistent = usuario.MantenerSesion;

                if (!usuario.MantenerSesion)
                {
                    p.ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(1);
                }
                else
                {
                    p.ExpiresUtc = DateTimeOffset.UtcNow.AddDays(1);
                }

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, p);

                // Almacena el usuario en una cookie
                Response.Cookies.Append("Usuario", JsonSerializer.Serialize(usuario));

                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["MensajeError"] = "Credenciales inválidas";
                return View();
            }
        }
        catch (System.Exception e)
        {
            return View();
        }
    }
}