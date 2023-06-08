using System.Net;
using meethub.Models;
using Microsoft.AspNetCore.Mvc;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;

namespace meethub.Controllers;

public class AssistantFormController : Controller
{
    private readonly MeethubdbContext _context;
    private readonly IWebHostEnvironment _hostEnvironment;

    public AssistantFormController(MeethubdbContext context, IWebHostEnvironment hostEnvironment)
    {
        _context = context;
        _hostEnvironment = hostEnvironment;
    }

    // VISTA FORMULARIO
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

    // VISTA REGISTRO EXITOSO
    public IActionResult Success(int AssistantId)
    {
        ViewBag.AssistantId = AssistantId;
        return View();
    }

    // VISTA REGISTRO FALLIDO
    public IActionResult Failed()
    {
        return View();
    }

    [HttpPost]
    public IActionResult DeleteTempData(string key)
    {
        TempData.Remove(key);
        return Ok();
    }

    // METODO REGISTRAR ASISTENTE
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

        // crear modelo del nuevo asistente
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

        // Comprobar si el asistente ya se ha registrado al evento
        var asistenteRepetido = _context.Assistants.FirstOrDefault(a => a.Email == newAssistant.Email && a.IdEvent == id);

        if (asistenteRepetido == null)
        {
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

                return RedirectToAction("Success", new { AssistantId = newAssistant.IdAssistant });
            }
            catch (Exception e)
            {
                return RedirectToAction("Failed");
            }
        }else{
            TempData["InvalidEmail"] = "Este email ya ha sido registrado a este evento.";
            return RedirectToAction("Form", new {id = id});
        }
    }

    // METODO GENERAR ENTRADA PDF
    public IActionResult GeneratePDF(int id)
    {
        var asistente = _context.Assistants.FirstOrDefault(a => a.IdAssistant == id);

        if (asistente != null)
        {
            var evento = _context.Events.FirstOrDefault(e => e.IdEvent == asistente.IdEvent);
            if (evento != null)
            {
                try
                {
                    // Crear un nuevo documento PDF
                    var document = new PdfDocument();

                    // Agregar una página al documento
                    var page = document.AddPage();

                    // Obtener el objeto XGraphics para dibujar en la página
                    var gfx = XGraphics.FromPdfPage(page);

                    // Fuente
                    int fontSize = 12;
                    var fontTitle = new XFont("Tahoma", fontSize, XFontStyle.Bold);
                    var fontContent = new XFont("Tahoma", fontSize);

                    gfx.TranslateTransform(0, 20);

                    // Logo
                    string logoPath = Path.Combine(_hostEnvironment.WebRootPath, "images", "logo.png");
                    XImage logoImage = XImage.FromFile(logoPath);
                    double logoWidth = 200;
                    double logoHeight = logoWidth * (logoImage.PixelHeight / (double)logoImage.PixelWidth);
                    gfx.DrawImage(logoImage, new XRect(10, 10, logoWidth, logoHeight));

                    // Datos del evento
                    gfx.TranslateTransform(0, 80);
                    gfx.DrawString("Datos del Evento:", fontTitle, XBrushes.Black, new XPoint(10, 10));
                    gfx.DrawString("Evento: " + evento.Titulo, fontContent, XBrushes.Black, new XPoint(10, 30));
                    gfx.DrawString("Dirección: " + evento.Direccion, fontContent, XBrushes.Black, new XPoint(10, 50));
                    gfx.DrawString("Fecha: " + evento.FechaInicio.ToString("dd/MM/yyyy HH:mm") + " - " + evento.FechaFin.ToString("dd/MM/yyyy HH:mm"), fontContent, XBrushes.Black, new XPoint(10, 70));

                    // Datos del asistente
                    gfx.DrawString("Datos del Asistente:", fontTitle, XBrushes.Black, new XPoint(10, 100));
                    gfx.DrawString("Nombre: " + asistente.Nombre, fontContent, XBrushes.Black, new XPoint(10, 120));
                    gfx.DrawString("Apellidos: " + asistente.Apellidos, fontContent, XBrushes.Black, new XPoint(10, 140));

                    // Obtener el código QR del asistente
                    var qrCodeUrl = asistente.QrCode;

                    // Cargar la imagen del código QR desde la URL
                    using (var webClient = new WebClient())
                    {
                        var qrImageData = webClient.DownloadData(qrCodeUrl);
                        using (var qrImageStream = new MemoryStream(qrImageData))
                        {
                            var qrImage = XImage.FromStream(() => qrImageStream);

                            // Especificar el tamaño de la página (A4 estándar)
                            var pageWidth = XUnit.FromCentimeter(21);
                            var pageHeight = XUnit.FromCentimeter(29.7);

                            // Calcular el tamaño y la posición de la imagen centrada
                            var maxWidth = pageWidth - XUnit.FromCentimeter(2);  // Ancho máximo de la página con un margen de 1 cm en cada lado
                            var maxHeight = pageHeight - XUnit.FromCentimeter(10);  // Alto máximo de la página con un margen de 5 cm en la parte superior

                            var aspectRatio = (double)qrImage.PixelWidth / qrImage.PixelHeight;
                            var width = Math.Min(maxWidth, qrImage.PixelWidth);
                            var height = width / aspectRatio;

                            if (height > maxHeight)
                            {
                                height = maxHeight;
                                width = height * aspectRatio;
                            }

                            var x = (pageWidth - width) / 2;  // Posición x centrada
                            var y = XUnit.FromCentimeter(5) + (maxHeight - height) / 2;  // Posición y centrada

                            gfx.DrawImage(qrImage, x, y, width, height);
                        }
                    }

                    // Guardar el documento en un MemoryStream
                    var memoryStream = new MemoryStream();
                    document.Save(memoryStream);
                    memoryStream.Seek(0, SeekOrigin.Begin);

                    // Devolver el archivo PDF
                    return File(memoryStream, "application/pdf", "entrada.pdf", true);
                }
                catch (Exception e)
                {
                    return RedirectToAction("Success", new { AssistantId = asistente.IdAssistant });
                }
            }
        }
        return RedirectToAction("Index", "home");
    }
}