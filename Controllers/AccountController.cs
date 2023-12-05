
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TattooStudioBooking.Models;
using Microsoft.EntityFrameworkCore;
namespace TattooStudioBooking;

public class AccountController : Controller
{
    private readonly BookingContext _context;

    public AccountController(BookingContext context)
    {
        this._context = context;
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {

        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    private bool ValidateLogin(string userName, string password, out Usuario usuario)
    {
        //verifica bd
        _context.Database.EnsureCreated();

        usuario = (Usuario)(_context.Artistas.FirstOrDefault(x => x.UserName == userName) ?? new Usuario());

        if (string.IsNullOrEmpty(usuario.UserName))
            usuario = (Usuario)(_context.Admins.FirstOrDefault(x => x.UserName == userName) ?? new Usuario());

        if (string.IsNullOrEmpty(usuario.UserName))
            return false;

        return password == usuario.Password;
    }

    [HttpPost]
    public async Task<IActionResult> Login(string userName, string password, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        Usuario usr = new Usuario();

        // Normally Identity handles sign in, but you can do it directly
        if (ValidateLogin(userName, password, out usr))
        {
            var claims = new List<Claim>
                {
                    new Claim("user", userName),
                    usr.IsAdmin ? new Claim("role", "Admin") :
                                  new Claim("role", "Artista")
                };

            await HttpContext.SignInAsync(new ClaimsPrincipal(new ClaimsIdentity(claims, "Cookies", "user", "role")));

            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return Redirect("/");
            }
        }

        return View();
    }

    [HttpGet]
    [Authorize(Roles = "Artista")]
    public IActionResult MiCuenta()
    {
        if (User.Identity == null)
        {
            ModelState.AddModelError("", "No se encontro user logeado");
            return View(new Artista());
        }
        else
            return View(_context.Artistas.FirstOrDefault(x => x.UserName == User.Identity.Name));
    }

    [HttpPost]
    [Authorize(Roles = "Artista")]
    public async Task<IActionResult> MiCuenta(Artista artista)
    {
        string pwd = string.Empty;
        try
        {
            //Valida contraseña
            pwd = (_context.Artistas.AsNoTracking().FirstOrDefault(x => x.Id == artista.Id) ?? new Artista()).Password;
            if (string.IsNullOrEmpty(pwd) || artista.Password != pwd)
            {
                ModelState.AddModelError("", "Contraseña incorrecta");
                return View(artista);
            }

            //Cambio de contraseña
            if (!string.IsNullOrEmpty(artista.Password2))
            {
                if (artista.Password2 != artista.Password3)
                {
                    ModelState.AddModelError("", "Contraseña nueva no coincide con confirmación de contraseña");
                    return View(artista);
                }
                else
                    artista.Password = artista.Password2;
            }

            //update
            _context.Entry(artista).State = EntityState.Modified;

            await _context.SaveChangesAsync();
            ViewBag.ShowSuccessMsg = "Y";
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
        }

        return View(artista);
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public IActionResult MiCuentaAdmin()
    {
        if (User.Identity == null)
        {
            ModelState.AddModelError("", "No se encontro user logeado");
            return View(new Admin());
        }
        else
            return View(_context.Admins.FirstOrDefault(x => x.UserName == User.Identity.Name));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> MiCuentaAdmin(Admin admin)
    {
        string pwd = string.Empty;
        try
        {
            //Valida contraseña
            pwd = (_context.Admins.AsNoTracking().FirstOrDefault(x => x.Id == admin.Id) ?? new Admin()).Password;
            if (string.IsNullOrEmpty(pwd) || admin.Password != pwd)
            {
                ModelState.AddModelError("", "Contraseña incorrecta");
                return View(admin);
            }

            //Cambio de contraseña
            if (!string.IsNullOrEmpty(admin.Password2))
            {
                if (admin.Password2 != admin.Password3)
                {
                    ModelState.AddModelError("", "Contraseña nueva no coincide con confirmación de contraseña");
                    return View(admin);
                }
                else
                    admin.Password = admin.Password2;
            }

            //update
            _context.Entry(admin).State = EntityState.Modified;

            await _context.SaveChangesAsync();
            ViewBag.ShowSuccessMsg = "Y";
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
        }

        return View(admin);
    }
    public IActionResult AccessDenied(string? returnUrl = null)
    {
        return View();
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();
        return Redirect("/");
    }

}
