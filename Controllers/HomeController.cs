using System;
using System.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using meethub.Models;

namespace meethub.Controllers;

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
        User usuario = null;
        if (Request.Cookies.TryGetValue(".AspNetCore.User", out string usuarioJson))
        {
            usuario = JsonSerializer.Deserialize<User>(usuarioJson);
        }
        return PartialView("_Profile", usuario);
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
        Debug.WriteLine("Id usuario: " + usuario.IdUser);

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
                Tipo = e.Tipo
            })
            .ToList();
        if (eventos != null)
        {
            eventos.Reverse();
        }
        return eventos;
    }

    public IActionResult Home()
    {

        if (Request.Cookies.TryGetValue(".AspNetCore.User", out string usuarioJson))
        {
            var usuario = JsonSerializer.Deserialize<User>(usuarioJson);
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

    [Route("Home/Event/{id?}")]
    public IActionResult Event(int id)
    {
        var evento = _context.Events.FirstOrDefault(e => e.IdEvent == id);
        var asistentes = _context.Assistants.Where(a => a.IdEvent == id).ToList();
        EventViewModel model = new EventViewModel
        {
            eventTarget = evento,
            AssistantList = asistentes
        };
        if (evento != null)
        {
            return View(model);
        }
        else
        {
            TempData["TempMessage"] = "Error al acceder al evento";
            return RedirectToAction("Index");
        }
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        // Eliminar la cookie de usuario
        Response.Cookies.Delete(".AspNetCore.User");
        return RedirectToAction("Login", "User");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    [HttpPost]
    public IActionResult DeleteTempData(string key)
    {
        TempData.Remove(key);
        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> CreateEvent(HomeViewModel model, IFormFile image)
    {
        User usuario = null;

        if (Request.Cookies.TryGetValue(".AspNetCore.User", out string usuarioJson))
        {
            usuario = JsonSerializer.Deserialize<User>(usuarioJson);
        }

        if (usuario == null)
        {
            TempData["TempMessage"] = "Usuario no valido";
            return RedirectToAction("Index");
        }

        // VALIDACION DE LA IMAGEN
        if (image != null && image.Length > 0)
        {
            using (var memoryStream = new MemoryStream())
            {
                image.CopyTo(memoryStream);
                model.NewEvent.Image = memoryStream.ToArray();
            }
        }
        else
        {
            model.NewEvent.Image = null;
        }

        // VALIDACION DE AFORO
        if (!model.NewEvent.Aforo.HasValue || model.NewEvent.Aforo == 0)
        {
            model.NewEvent.Aforo = null;
        }

        // CREACION DEL MODELO
        var newEvent = new Event
        {
            IdUser = usuario.IdUser,
            Image = model.NewEvent.Image,
            Titulo = model.NewEvent.Titulo,
            Descripcion = model.NewEvent.Descripcion,
            Aforo = model.NewEvent.Aforo,
            Direccion = model.NewEvent.Direccion,
            FechaInicio = model.NewEvent.FechaInicio,
            FechaFin = model.NewEvent.FechaFin,
            Tipo = model.NewEvent.Tipo
        };

        // Guardar el nuevo evento en la base de datos
        try
        {
            _context.Events.Add(newEvent);
            _context.SaveChanges();

            // Redirigir a la página de éxito o realizar alguna otra acción
            TempData["TempMessage"] = "Evento creado correctamente";
        }
        catch (Exception e)
        {
            TempData["TempMessage"] = "Error al crear el evento";
        }
        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult EditarUsuario(string nombre, string apellidos)
    {
        User usuario = null;
        if (Request.Cookies.TryGetValue(".AspNetCore.User", out string usuarioJson))
        {
            usuario = JsonSerializer.Deserialize<User>(usuarioJson);
        }

        if (usuario != null)
        {
            try
            {
                usuario.Nombre = nombre;
                usuario.Apellidos = apellidos;
                _context.Users.Update(usuario);
                _context.SaveChanges();

                string usuarioActualizadoJson = JsonSerializer.Serialize(usuario);
                Response.Cookies.Append(".AspNetCore.User", usuarioActualizadoJson);

                TempData["TempMessage"] = "Usuario actualizado";
            }
            catch (Exception e)
            {
                TempData["TempMessage"] = "Error al actualizar usuario";
            }
        }
        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult EditarEvento(EventViewModel eventViewModel)
    {
        // Obtén el evento actual de la base de datos
        var evento = _context.Events.FirstOrDefault(e => e.IdEvent == eventViewModel.eventTarget.IdEvent);

        if (evento != null)
        {
            try
            {
                // Actualiza las propiedades del evento con los valores del modelo
                evento.Titulo = eventViewModel.eventTarget.Titulo;
                evento.Descripcion = eventViewModel.eventTarget.Descripcion;
                evento.Direccion = eventViewModel.eventTarget.Direccion;
                evento.Aforo = eventViewModel.eventTarget.Aforo;
                evento.FechaInicio = eventViewModel.eventTarget.FechaInicio;
                evento.FechaFin = eventViewModel.eventTarget.FechaFin;

                // Guarda los cambios en la base de datos
                _context.Update(evento);
                _context.SaveChanges();

                TempData["TempMessage"] = "Evento actualizado correctamente.";
                return RedirectToAction("Event", new { id = evento.IdEvent });
            }
            catch (Exception e)
            {
                TempData["TempMessage"] = "Error al actualizar el evento.";
                return RedirectToAction("Event", new { id = evento.IdEvent });
            }
        }
        return RedirectToAction("Event", new { id = evento.IdEvent });
    }
}