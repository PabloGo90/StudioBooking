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
    public IActionResult Index(int ptoId, int semanaIdx = 0)
    {
        return View(ObtenerReservas(ptoId, semanaIdx));
    }
    [HttpPost]
    [Authorize(Roles = "Artista")]
    public async Task<IActionResult> RegistrarAgenda(int ptoId, int semanaIdx, CrearReserva reserva)// int HoraDesde, int HoraHasta, string Usuario)
    {
        if (ModelState.IsValid)
        {
            if (User.Identity == null || User.Identity.Name != reserva.Usuario)
            {
                ModelState.AddModelError("", "No se encontro user logeado");
                return View("Index", ObtenerReservas(ptoId, semanaIdx));
            }

            Artista? artista = _context.Artistas.FirstOrDefault(x => x.UserName == reserva.Usuario);
            PuestoTrabajo? puestoTrabajo = _context.PuestosTrabajo.FirstOrDefault(x => x.Id == ptoId);

            if (artista == null)
            {
                ModelState.AddModelError("", $"No se encontro user {reserva.Usuario}");
                return View("Index", ObtenerReservas(ptoId, semanaIdx));
            }
            if (puestoTrabajo == null)
            {
                ModelState.AddModelError("", $"No se encontro puesto de trabajo {ptoId}");
                return View("Index", ObtenerReservas(ptoId, semanaIdx));
            }
            _context.Agendas.Add(new Agenda()
            {
                PuestoTrabajo = puestoTrabajo,
                HoraDesde = reserva.HoraDesde,
                HoraHasta = reserva.HoraHasta,
                FechaReserva = reserva.fecha,
                Artista = artista
            });
            await _context.SaveChangesAsync();
            ViewBag.ShowSuccessMsg = "Y";
        }
        return View("Index", ObtenerReservas(ptoId, semanaIdx));
    }
    [Authorize]
    public IActionResult Actualizar(int id, int ptoId)
    {
        try
        {
            Agenda? agenda = _context.Agendas.Include(art => art.Artista)
                                            .AsNoTracking()
                                            .FirstOrDefault(x => x.Id == id);
            if (agenda == null)
            {
                ModelState.AddModelError("", "Agenda no encontrada");
                return View(new Agenda());
            }
            if (User.Identity == null || User.Identity.Name != agenda.Artista.UserName)
            {
                ModelState.AddModelError("", "Usuario no logeado o distinto a usuario agenda"); ;
                return View(new Agenda());
            }
            return View(agenda);
        }
        finally
        {
            ViewBag.ptoId = ptoId;
            ViewBag.User = User.Identity == null ? "" : User.Identity.Name;
        }
    }
    [HttpGet]
    [Authorize(Roles = "Artista")]
    public async Task<IActionResult> Eliminar(int id, int ptoId)
    {
        try
        {
            if (ModelState.IsValid)
            {
                Agenda? agenda = _context.Agendas.Include(art => art.Artista)
                                        .FirstOrDefault(x => x.Id == id);
                if (agenda == null)
                    ModelState.AddModelError("", "Agenda no encontrada");

                else if (User.Identity == null || User.Identity.Name != agenda.Artista.UserName)
                    ModelState.AddModelError("", "Usuario no logeado o distinto a usuario agenda");
                else
                {
                    _context.Remove(agenda);
                    await _context.SaveChangesAsync();
                    ViewBag.ShowSuccessMsg = "Y";
                }
            }
            return View("Actualizar", new Agenda());
        }
        finally
        {
            ViewBag.ptoId = ptoId;
            ViewBag.User = User.Identity == null ? "" : User.Identity.Name;
        }
    }

    private List<Reserva> ObtenerReservas(int ptoId, int semanaIdx)
    {

        List<Reserva> reservas = new List<Reserva>();
        DateTime hoy = semanaIdx == 0 ? DateTime.Now.Date : DateTime.Now.Date.AddDays(semanaIdx * 7);
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
                                             .Where(x => x.FechaReserva == diaInicio && x.PuestoTrabajo.Id == ptoId &&
                                                         bloque.HoraDesde >= x.HoraDesde && bloque.HoraDesde <= x.HoraHasta).FirstOrDefault();


                if (agenda != null)
                    reservas.Add(new Reserva
                    {
                        Bloque = bloque,
                        FechaReserva = diaInicio,
                        PuestoTrabajoId = ptoId,
                        ArtistaId = agenda.Artista.Id,
                        ArtistaUserName = agenda.Artista.UserName,
                        AgendaId = agenda.Id,
                        isReserva = true,
                        ReservaStr1 = agenda.Artista.Instagram,
                        ReservaStr2 = $"en {agenda.PuestoTrabajo.Nombre}",
                        ReservaStr3 = $"Reserva de {agenda.Artista.Nombre}"
                    });
                else
                    reservas.Add(new Reserva { Bloque = bloque, FechaReserva = diaInicio, PuestoTrabajoId = ptoId });

            }

            diaInicio = diaInicio.AddDays(1);
        }
;
        ViewBag.bloques = bloques;
        ViewBag.ptoId = ptoId;
        ViewBag.semanaIdx = semanaIdx;
        ViewBag.semanaAntIdx = semanaIdx-1;
        ViewBag.semanaSigIdx = semanaIdx+1;
        ViewBag.fechaactual = hoy.ToString("yyyy-MM-dd");
        ViewBag.User = User.Identity == null ? "" : User.Identity.Name;
        return reservas;
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
