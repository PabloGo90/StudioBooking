using Microsoft.AspNetCore.Identity;

namespace TattooStudioBooking;

public class Artista : Usuario
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Instagram { get; set; } = string.Empty;
}
