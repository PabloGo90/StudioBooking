using System.ComponentModel.DataAnnotations.Schema;
using SQLitePCL;

namespace TattooStudioBooking;

public class Agenda
{
    public int Id { get; set; }
    public Artista Artista { get; set; }
    public PuestoTrabajo PuestoTrabajo { get; set; }
    public DateTime FechaReserva { get; set; }
    public int HoraDesde { get; set; }
    public int HoraHasta { get; set; }

    public Agenda()
    {
        this.Artista = new Artista();
        this.PuestoTrabajo = new PuestoTrabajo();
    }


}
