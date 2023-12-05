namespace TattooStudioBooking.Models
{
    public class CrearReserva
    {
        public DateTime fecha { get; set; }
        public int HoraDesde { get; set; }
        public int HoraHasta { get; set; }
        public string Usuario { get; set; } = string.Empty;
    }
}
