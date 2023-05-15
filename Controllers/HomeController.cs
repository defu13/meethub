using System.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mvc_pruebas.Models;

namespace mvc_pruebas.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly MeethubdbContext _context;

    public HomeController(MeethubdbContext context)
    {
        _context = context;
    }

    public IActionResult Stats()
    {
        return View();
    }

    public IActionResult Profile()
    {
        return View();
    }

    public async Task<List<Event>> obtenerEventos(User usuario)
    {
        if (usuario == null)
        {
            // Manejar el caso en el que el usuario no existe
            return null;
        }
        Debug.WriteLine(usuario.IdUser);

        // Realiza la consulta a la base de datos
        var eventos = _context.Events
            .Where(e => e.IdUser == usuario.IdUser)
            .Select(e => new Event
            {
                IdEvent = e.IdEvent,
                IdUser = e.IdUser,
                Image = e.Image,
                Titulo = e.Titulo,
                Descripcion = e.Descripcion,
                Aforo = e.Aforo,
                Direccion = e.Direccion,
                FechaInicio = e.FechaInicio,
                FechaFin = e.FechaFin,
                Tipo = e.Tipo,
                Enlace = e.Enlace
            })
            .ToList();
        Debug.WriteLine(eventos);
        return eventos;
    }

    // public IActionResult Home()
    // {
    //     return View();
    // }

    public IActionResult Index()
    {
        // if (Request.Cookies.TryGetValue("Usuario", out string usuarioJson))
        // {
        //     var usuario = JsonSerializer.Deserialize<User>(usuarioJson);

        //     Debug.WriteLine("Id de usuario: " + usuario.IdUser);

        //     var eventos = obtenerEventos(usuario);

        //     if (eventos == null)
        //     {
        //         Debug.WriteLine("Error: el evento es null");
        //     }

        //     return View(eventos);
        // }
        // return RedirectToAction("Login", "User");
        return View();
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login", "User");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
