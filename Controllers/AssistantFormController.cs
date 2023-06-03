using meethub.Models;
using Microsoft.AspNetCore.Mvc;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;

namespace meethub.Controllers;

public class AssistantFormController : Controller
{
    private readonly MeethubdbContext _context;

    public AssistantFormController(MeethubdbContext context)
    {
        _context = context;
    }

    [Route("AssistantForm/Form/{id?}")]
    public IActionResult Form(int id)
    {
        var evento = _context.Events.FirstOrDefault(e => e.IdEvent == id);
        if (evento == null)
        {
            return RedirectToAction("Login", "User"); // Manejar el caso en el que no se encuentre el evento con el id especificado
        }

        return View(evento);
    }

    public IActionResult Success(int EventId)
    {
        ViewBag.EventId = EventId;
        return View();
    }

    public IActionResult Failed()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> RegisterToEvent(String nombre, String apellidos, String email, String telefono, int id)
    {
        // COMPROBAR TIPO DE EVENTO
        var evento = _context.Events.FirstOrDefault(e => e.IdEvent == id);
        Boolean aprobacion = true;
        if (evento != null)
        {
            if (evento.Tipo == "invitacion")
            {
                aprobacion = false;
            }
        }

        // Por ejemplo, podrías crear un nuevo registro de asistente
        var newAssistant = new Assistant
        {
            IdEvent = id,
            Nombre = nombre,
            Apellidos = apellidos,
            Email = email,
            Telefono = telefono,
            Aprobado = aprobacion,
            Confirmado = false,
            QrCode = ""
        };

        try
        {
            // Guardar el nuevo asistente en la base de datos
            _context.Assistants.Add(newAssistant);
            _context.SaveChanges();

            // CREACION DE QR
            String qrUrl = "https://chart.googleapis.com/chart?cht=qr&chs=200x200&chld=L|3&chl=" + newAssistant.IdAssistant;
            newAssistant.QrCode = qrUrl;
            _context.Assistants.Update(newAssistant);
            _context.SaveChanges();

            return RedirectToAction("Success", new { EventId = id });
        }
        catch (Exception e)
        {
            return RedirectToAction("Failed");
        }
    }

    public IActionResult GeneratePDF(int id)
    {
        // Crear un nuevo documento PDF
        var document = new PdfDocument();

        // Agregar una página al documento
        var page = document.AddPage();

        // Obtener el objeto XGraphics para dibujar en la página
        var gfx = XGraphics.FromPdfPage(page);

        // Dibujar contenido en la página
        gfx.DrawString("Hola, mundo!", new XFont("Arial", 12), XBrushes.Black,
            new XRect(10, 10, page.Width - 20, page.Height - 20),
            XStringFormats.Center);

        // Guardar el documento en un MemoryStream
        var memoryStream = new MemoryStream();
        document.Save(memoryStream);
        memoryStream.Seek(0, SeekOrigin.Begin);

        // Devolver el PDF para mostrarlo en una ventana nueva del navegador
        return File(memoryStream, "application/pdf", "entrada.pdf", true);
    }
}