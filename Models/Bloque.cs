namespace TattooStudioBooking;

public class Bloque
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public int HoraDesde { get; set; }
    public int HoraHasta { get; set; }
    public bool ActivoFindesem { get; set; }

    public string HoraDesdeStr { get { return this.HoraDesde.ToString().PadLeft(2, '0'); } }
    public string HoraHastaStr { get { return this.HoraHasta.ToString().PadLeft(2, '0'); } }
}
