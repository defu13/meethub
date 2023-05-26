using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
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
                // CREAR AUTENTICACION AL USUARIO
                List<Claim> claims = new List<Claim>(){
                    new Claim(ClaimTypes.Name, usuario.Email),
                };
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);
                var authenticationProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTime.UtcNow.Add(TimeSpan.FromDays(15))
                };
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authenticationProperties);

                // Almacenar el usuario en una cookie
                var cookieOptions = new CookieOptions
                {
                    Path = "/",
                    IsEssential = true,
                    Expires = DateTime.UtcNow.Add(TimeSpan.FromDays(15))
                };
                Response.Cookies.Append(".AspNetCore.User", JsonSerializer.Serialize(usuario), cookieOptions);
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
        TempData["MensajeError"] = "Usuario registrado correctamente.";
        return RedirectToAction("Login");
    }
}