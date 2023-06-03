using System.Net;
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

    public IActionResult Success(int AssistantId)
    {
        ViewBag.AssistantId = AssistantId;
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

            return RedirectToAction("Success", new { EventId = newAssistant.IdAssistant });
        }
        catch (Exception e)
        {
            return RedirectToAction("Failed");
        }
    }

    public IActionResult GeneratePDF(int id)
    {
        var asistente = _context.Assistants.FirstOrDefault(a => a.IdAssistant == id);
        var evento = _context.Events.FirstOrDefault(e => e.IdEvent == asistente.IdEvent);

        // Crear un nuevo documento PDF
        var document = new PdfDocument();

        // Agregar una página al documento
        var page = document.AddPage();

        // Obtener el objeto XGraphics para dibujar en la página
        var gfx = XGraphics.FromPdfPage(page);

        // Definir las fuentes y estilos
        var fontTitle = new XFont("Arial", 14, XFontStyle.Bold);
        var fontContent = new XFont("Arial", 12);

        // Escribir los datos del evento
        gfx.DrawString("Datos del Evento:", fontTitle, XBrushes.Black, new XPoint(10, 10));
        gfx.DrawString("Evento: " + evento.Titulo, fontContent, XBrushes.Black, new XPoint(10, 30));
        gfx.DrawString("Fecha: " + evento.FechaInicio.ToString() + " - " + evento.FechaFin.ToString(), fontContent, XBrushes.Black, new XPoint(10, 50));
        // Agregar más datos del evento según sea necesario

        // Escribir los datos del asistente
        gfx.DrawString("Datos del Asistente:", fontTitle, XBrushes.Black, new XPoint(10, 80));
        gfx.DrawString("Nombre: " + asistente.Nombre, fontContent, XBrushes.Black, new XPoint(10, 100));
        gfx.DrawString("Apellidos: " + asistente.Apellidos, fontContent, XBrushes.Black, new XPoint(10, 120));
        // Agregar más datos del asistente según sea necesario

        // Obtener el código QR del asistente
        var qrCodeUrl = asistente.QrCode;

        // Cargar la imagen del código QR desde la URL
        using (var webClient = new WebClient())
        {
            var qrImageData = webClient.DownloadData(qrCodeUrl);
            using (var qrImageStream = new MemoryStream(qrImageData))
            {
                var qrImage = XImage.FromStream(() => qrImageStream);
                gfx.DrawImage(qrImage, new XPoint(10, 150));
            }
        }

        // Guardar el documento en un MemoryStream
        var memoryStream = new MemoryStream();
        document.Save(memoryStream);
        memoryStream.Seek(0, SeekOrigin.Begin);

        // Devolver el PDF para mostrarlo en una ventana nueva del navegador
        return File(memoryStream, "application/pdf", "entrada.pdf", true);
    }
}