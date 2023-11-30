namespace TattooStudioBooking;

public class Reserva
{
    public int AgendaId { get; set; }
    public int ArtistaId { get; set; }
    public string ArtistaUserName { get; set; }
    public int PuestoTrabajoId { get; set; }
    public DateTime FechaReserva { get; set; }
    public Bloque Bloque { get; set; }
    public bool isReserva { get; set; }
    public string ReservaStr1 { get; set; }
    public string ReservaStr2 { get; set; }
    public string ReservaStr3 { get; set; }
    public Enums.ColorReserva ColorReserva
    {
        get
        {
            //Obtiene color segun el id de artista si se sobrepasa el indice de enum elige el primero
            Enums.ColorReserva _color;
            if (Enum.TryParse<Enums.ColorReserva>(ArtistaId.ToString(), out _color))
                return _color;
            else
                return Enums.ColorReserva.bgsky;
        }
    }
    public Reserva()
    {
        this.Bloque = new Bloque();
        this.ReservaStr1 = "";
        this.ReservaStr2 = "";
        this.ReservaStr3 = "";
        this.ArtistaUserName = "";
    }
}
