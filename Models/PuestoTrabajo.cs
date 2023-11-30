namespace TattooStudioBooking;

public class PuestoTrabajo
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    
    public PuestoTrabajo()
    {
        this.Nombre = string.Empty;
    }

    // public PuestoTrabajo(int id)
    // {
    //     this.Id = id;
    //     this.Nombre = string.Empty;
    // }

}
