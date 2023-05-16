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
    private readonly IWebHostEnvironment _hostEnvironment;

    public HomeController(MeethubdbContext context, IWebHostEnvironment hostEnvironment)
    {
        _context = context;
        _hostEnvironment = hostEnvironment;
    }

    public IActionResult Stats()
    {
        return PartialView("_Stats");
    }

    public IActionResult Profile()
    {
        return PartialView("_Profile");
    }

    private byte[] GetDefaultImage()
    {
        string path = Path.Combine(_hostEnvironment.WebRootPath, "images", "fondoprueba.jpg");

        using (var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
        {
            using (var memoryStream = new MemoryStream())
            {
                fileStream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }

    public List<Event> obtenerEventos(User usuario)
    {
        if (usuario == null)
        {
            // Manejar el caso en el que el usuario no existe
            return null;
        }
        Debug.WriteLine("Id usuario: "+usuario.IdUser);

        // Realiza la consulta a la base de datos
        var eventos = _context.Events
            .Where(e => e.IdUser == usuario.IdUser)
            .Select(e => new Event
            {
                IdEvent = e.IdEvent,
                IdUser = e.IdUser,
                Image = e.Image ?? GetDefaultImage(),
                Titulo = e.Titulo,
                Descripcion = e.Descripcion,
                Aforo = e.Aforo ?? 0,
                Direccion = e.Direccion,
                FechaInicio = e.FechaInicio,
                FechaFin = e.FechaFin,
                Tipo = e.Tipo,
                Enlace = e.Enlace ?? "EnlaceNull"
            })
            .ToList();
        return eventos;
    }

    public IActionResult Home()
    {
        
        if (Request.Cookies.TryGetValue("Usuario", out string usuarioJson))
        {
            var usuario = JsonSerializer.Deserialize<User>(usuarioJson);
            // var eventos = obtenerEventos(usuario);
            var viewModel = new HomeViewModel
            {
                NewEvent = new Event(),
                EventList = obtenerEventos(usuario)
            };

            return PartialView("_Home", viewModel);
        }
        else
        {
            return RedirectToAction("Login", "User");
        }
    }

    public IActionResult Index()
    {
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
