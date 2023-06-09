using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using meethub.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

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

    // Función para verificar la contraseña hasheada
    private bool VerifyPasswordHash(string password, string hashedPassword)
    {
        var hashedPasswordBytes = Convert.FromBase64String(hashedPassword);
        return KeyDerivation.Pbkdf2(
            password: password,
            salt: new byte[0],
            prf: KeyDerivationPrf.HMACSHA1,
            iterationCount: 10000,
            numBytesRequested: 256 / 8).SequenceEqual(hashedPasswordBytes);
    }

    // METODO LOGIN
    [HttpPost]
    public async Task<IActionResult> Login(User user)
    {
        var usuario = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email);

        try
        {
            if (usuario != null && VerifyPasswordHash(user.Password, usuario.Password))
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

    // METODO DE REGISTRO DE USUARIOS
    [HttpPost]
    public async Task<IActionResult> Register(User model)
    {
        // Verificar si el usuario ya existe en la base de datos por el email
        var existingUser = _context.Users.FirstOrDefault(u => u.Email == model.Email);
        if (existingUser != null)
        {
            // El usuario ya existe
            TempData["MensajeError"] = "El usuario ya existe.";
            return RedirectToAction("Login");
        }

        var user = new User
        {
            Nombre = model.Nombre,
            Apellidos = model.Apellidos,
            Email = model.Email,
            Password = model.Password
        };

        // Hashear la contraseña antes de guardarla en la base de datos
        var hashedPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: model.Password,
            salt: new byte[0],
            prf: KeyDerivationPrf.HMACSHA1,
            iterationCount: 10000,
            numBytesRequested: 256 / 8));
        user.Password = hashedPassword;

        // Guardar el usuario en la base de datos
        _context.Users.Add(user);
        _context.SaveChanges();

        // Redireccionar al usuario al login de nuevo
        TempData["MensajeError"] = "Usuario registrado correctamente.";
        return RedirectToAction("Login");
    }
}