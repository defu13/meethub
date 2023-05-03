

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mvc_pruebas.Models;

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
    public async Task<IActionResult> Login(string email, string password)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email && u.Password == password);

        if (user == null)
        {
            Console.WriteLine("Credenciales inválidas");
            return View();
        }

        // Autenticación exitosa, redirigir a la vista deseada
        return RedirectToAction("Index", "Home");
    }
}