using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using meethub.Models;

namespace meethub.Controllers;
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
                TempData["MensajeError"] = "Correo electrónico o contraseña incorrectos.";
                return View();
            }
        }
        catch (System.Exception e)
        {
            return View();
        }
    }

    [HttpPost]
    public async Task<IActionResult> Register(User model)
    {
        // Verificar si el usuario ya existe en la base de datos por el email
        var existingUser = _context.Users.FirstOrDefault(u => u.Email == model.Email);
        if (existingUser != null)
        {
            // El usuario ya existe, mostrar un mensaje de error o redireccionar a una página de error
            TempData["MensajeError"] = "El usuario ya existe.";
            return RedirectToAction("Login");
        }

        var user = new User
        {
            Nombre = model.Nombre,
            Apellidos = model.Apellidos,
            Email = model.Email,
            Password = model.Password
            // Otros campos del usuario si es necesario
        };

        // Guardar el usuario en la base de datos (código según tu implementación)
        _context.Users.Add(user);
        _context.SaveChanges();

        // Redireccionar al usuario a una página de éxito o realizar alguna acción adicional
        return RedirectToAction("Login");
    }
}