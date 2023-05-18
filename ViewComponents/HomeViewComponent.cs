using System.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using meethub.Models;

public class HomeViewComponent : ViewComponent
{
    private readonly MeethubdbContext _context;
    private readonly IWebHostEnvironment _hostEnvironment;

    public HomeViewComponent(MeethubdbContext context, IWebHostEnvironment hostEnvironment)
    {
        _context = context;
        _hostEnvironment = hostEnvironment;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        if (Request.Cookies.TryGetValue("Usuario", out string usuarioJson))
        {
            var usuario = JsonSerializer.Deserialize<User>(usuarioJson);

            Debug.WriteLine("Id de usuario: " + usuario.IdUser);

            var eventos = await _context.Events
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
                .ToListAsync();

            if (eventos == null)
            {
                Debug.WriteLine("Error: el evento es null");
            }

            return View("Home", eventos);
        }
        else
        {
            return null;
        }
    }

    private byte[] GetDefaultImage()
    {
        string path = _hostEnvironment.WebRootPath;

        string imagePath = Path.Combine(path, "images", "fondoprueba.jpg");

        byte[] imageData = File.ReadAllBytes(imagePath);

        return imageData;
    }
}