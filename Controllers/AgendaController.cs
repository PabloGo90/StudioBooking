using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TattooStudioBooking.Models;
namespace TattooStudioBooking;

public class AgendaController : Controller
{
    private readonly ILogger<AgendaController> _logger;
    private readonly BookingContext _context;

    public AgendaController(ILogger<AgendaController> logger, BookingContext context)
    {
        _logger = logger;
        this._context = context;
    }

    [Authorize]
    public IActionResult Index(int id = 0, int semanaIndex = 0)
    {
        List<Reserva> reservas = new List<Reserva>();
        DateTime hoy = semanaIndex == 0 ? DateTime.Now.Date : DateTime.Now.Date.AddDays(semanaIndex * 7);
        List<Bloque> bloques = _context.Bloques.ToList();
        DateTime domingo = hoy.DayOfWeek == DayOfWeek.Sunday ? hoy : hoy.AddDays(7 - (int)hoy.DayOfWeek);//obtiene domingo
        DateTime diaInicio = hoy.AddDays(hoy.DayOfWeek == DayOfWeek.Sunday ? -6 : (1 - (int)hoy.DayOfWeek)); //obtiene lunes

        while ((domingo.Date - diaInicio.Date).Days >= 0)
        {
            //Crea agendas vacias para bloques sin reserva 
            foreach (Bloque bloque in bloques)
            {
                var agenda = _context.Agendas.Include(pto => pto.PuestoTrabajo)
                                             .Include(art => art.Artista)
                                             .Where(x => x.FechaReserva == diaInicio && x.PuestoTrabajo.Id == id &&
                                                         bloque.HoraDesde >= x.HoraDesde && bloque.HoraDesde <= x.HoraHasta).FirstOrDefault();


                if (agenda != null)
                    reservas.Add(new Reserva
                    {
                        Bloque = bloque,
                        FechaReserva = diaInicio,
                        PuestoTrabajoId = id,
                        ArtistaId = agenda.Artista.Id,
                        ArtistaUserName = agenda.Artista.UserName,
                        AgendaId = agenda.Id,
                        isReserva = true,
                        ReservaStr1 = agenda.Artista.Instagram,
                        ReservaStr2 = $"en {agenda.PuestoTrabajo.Nombre}",
                        ReservaStr3 = $"Reserva de {agenda.Artista.Nombre}"
                    });
                else
                    reservas.Add(new Reserva { Bloque = bloque, FechaReserva = diaInicio, PuestoTrabajoId = id });

            }

            diaInicio = diaInicio.AddDays(1);
        }

        ViewBag.bloques = _context.Bloques.ToList();
        ViewBag.PuestoTrabajoId = id;
        ViewBag.User = User.Identity == null ? "" : User.Identity.Name;

        return View(reservas);
    }
    [HttpPost]
    [Authorize(Roles = "Artista")]
    public async Task<IActionResult> RegistrarAgenda(string PuestoTrabajoId, int HoraDesde, int HoraHasta, string Usuario)
    {
        if (User.Identity == null || User.Identity.Name != Usuario)
        {
            ModelState.AddModelError("", "No se encontro user logeado");
            return RedirectToAction("Index", "Agenda", new { id = int.Parse(PuestoTrabajoId) });
        }

        Artista? artista = _context.Artistas.FirstOrDefault(x => x.UserName == Usuario);
        PuestoTrabajo? puestoTrabajo = _context.PuestosTrabajo.FirstOrDefault(x => x.Id == int.Parse(PuestoTrabajoId));

        if (artista == null)
        {
            ModelState.AddModelError("", "No se encontro user logeado");
            return RedirectToAction("Index", "Agenda", new { id = int.Parse(PuestoTrabajoId) });
        }
        _context.Agendas.Add(new Agenda()
        {
            PuestoTrabajo = puestoTrabajo ?? new PuestoTrabajo(),
            // PuestoTrabajo = new PuestoTrabajo(){Id = int.Parse(PuestoTrabajoId)},
            HoraDesde = HoraDesde,
            HoraHasta = HoraHasta,
            FechaReserva = DateTime.Now.Date,
            Artista = artista
        });
        await _context.SaveChangesAsync();

        return RedirectToAction("Index", "Agenda", new { id = int.Parse(PuestoTrabajoId) });
    }
    [Authorize]
    public IActionResult Actualizar(int id, int PuestoTrabajoId)
    {
        Agenda? agenda = _context.Agendas.Include(art => art.Artista)
                                        .AsNoTracking()
                                        .FirstOrDefault(x => x.Id == id);
        if (agenda == null)
        {
            ModelState.AddModelError("", "Agenda no encontrada");
            return RedirectToAction("Index", "Agenda", new { id = PuestoTrabajoId });
        }
        if (User.Identity == null || User.Identity.Name != agenda.Artista.UserName)
        {
            ModelState.AddModelError("", "Usuario no logeado o distinto a usuario agenda");
            return RedirectToAction("Index", "Agenda", new { id = PuestoTrabajoId });
        }

        ViewBag.PuestoTrabajoId = PuestoTrabajoId;
        ViewBag.User = User.Identity == null ? "" : User.Identity.Name;

        return View(agenda);
    }
    [HttpGet]
    [Authorize(Roles = "Artista")]
    public async Task<IActionResult> Eliminar(int id, int PuestoTrabajoId)
    {
        Agenda? agenda = _context.Agendas.Include(art => art.Artista)
                                        .AsNoTracking()
                                        .FirstOrDefault(x => x.Id == id);
        if (agenda == null)
            ModelState.AddModelError("", "Agenda no encontrada");

        else if (User.Identity == null || User.Identity.Name != agenda.Artista.UserName)
            ModelState.AddModelError("", "Usuario no logeado o distinto a usuario agenda");
        else
        {

            _context.Remove(agenda);
            await _context.SaveChangesAsync();

        }
        return RedirectToAction("Index", "Agenda", new { id = PuestoTrabajoId });
    }
    
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public IActionResult Listar()
    {                                    
        return View(_context.Agendas.Include(art => art.Artista)
                                    .Include(pto => pto.PuestoTrabajo)
                                    .Select(x => new Reserva {
                                        AgendaId = x.Id,
                                        ArtistaId = x.Artista.Id,
                                        ArtistaUserName = x.Artista.UserName,
                                        PuestoTrabajoId = x.PuestoTrabajo.Id,
                                        FechaReserva = x.FechaReserva,
                                        Bloque = new Bloque(){HoraDesde = x.HoraDesde, HoraHasta = x.HoraHasta}
                                    }).ToList());

    }
}
