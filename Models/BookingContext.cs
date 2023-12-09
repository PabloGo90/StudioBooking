using Microsoft.EntityFrameworkCore;
using TattooStudioBooking.Helpers;

namespace TattooStudioBooking;

public class BookingContext : DbContext
{
    public DbSet<Agenda> Agendas { get; set; }
    public DbSet<Artista> Artistas { get; set; }
    public DbSet<Admin> Admins { get; set; }
    public DbSet<PuestoTrabajo> PuestosTrabajo { get; set; }
    public DbSet<Bloque> Bloques { get; set; }
    //private string DbPath { get; set; } 

    public BookingContext()
    {
        ////guarda en userFolder
        //var folder = Environment.SpecialFolder.LocalApplicationData;
        //var path = Environment.GetFolderPath(folder);
        //DbPath = System.IO.Path.Join(path, "booking.db");
    }
    public BookingContext(DbContextOptions<BookingContext> options) : base(options)
    {
        //////guarda en userFolder
        //var folder = Environment.SpecialFolder.LocalApplicationData;
        //var path = Environment.GetFolderPath(folder);
        //DbPath = System.IO.Path.Join(path, "booking.db");
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        Security security = new Security();
        
        modelBuilder.Entity("TattooStudioBooking.Bloque").HasData(new Bloque { Id = 1, Nombre = "Temprano", HoraDesde = 8, HoraHasta = 10, ActivoFindesem = true });
        modelBuilder.Entity("TattooStudioBooking.Bloque").HasData(new Bloque { Id = 2, Nombre = "Temprano", HoraDesde = 10, HoraHasta = 12, ActivoFindesem = true });
        modelBuilder.Entity("TattooStudioBooking.Bloque").HasData(new Bloque { Id = 3, Nombre = "Tarde", HoraDesde = 12, HoraHasta = 14, ActivoFindesem = true });
        modelBuilder.Entity("TattooStudioBooking.Bloque").HasData(new Bloque { Id = 4, Nombre = "Tarde", HoraDesde = 14, HoraHasta = 16, ActivoFindesem = true });
        modelBuilder.Entity("TattooStudioBooking.Bloque").HasData(new Bloque { Id = 5, Nombre = "Tarde", HoraDesde = 16, HoraHasta = 18, ActivoFindesem = true });
        modelBuilder.Entity("TattooStudioBooking.Bloque").HasData(new Bloque { Id = 6, Nombre = "Tarde", HoraDesde = 18, HoraHasta = 20, ActivoFindesem = true });
        modelBuilder.Entity("TattooStudioBooking.Bloque").HasData(new Bloque { Id = 7, Nombre = "Noche", HoraDesde = 20, HoraHasta = 22, ActivoFindesem = true });
        modelBuilder.Entity("TattooStudioBooking.Bloque").HasData(new Bloque { Id = 8, Nombre = "Noche", HoraDesde = 22, HoraHasta = 24, ActivoFindesem = true });


        modelBuilder.Entity("TattooStudioBooking.Artista").HasData(new { Id = 1, Instagram = "", IsAdmin = false, Nombre = "nombre", UserName = "Danu", PasswordStored =  security.Encrypt("pwd", "1234") });
        modelBuilder.Entity("TattooStudioBooking.Artista").HasData(new { Id = 2, Instagram = "", IsAdmin = false, Nombre = "nombre", UserName = "Joshua", PasswordStored =  security.Encrypt("pwd", "1234") });
        modelBuilder.Entity("TattooStudioBooking.Artista").HasData(new { Id = 3, Instagram = "", IsAdmin = false, Nombre = "nombre", UserName = "Tere", PasswordStored =  security.Encrypt("pwd", "1234") });
        modelBuilder.Entity("TattooStudioBooking.Artista").HasData(new { Id = 4, Instagram = "", IsAdmin = false, Nombre = "nombre", UserName = "Aless", PasswordStored =  security.Encrypt("pwd", "1234") });
        modelBuilder.Entity("TattooStudioBooking.Artista").HasData(new { Id = 5, Instagram = "", IsAdmin = false, Nombre = "nombre", UserName = "Gerardo", PasswordStored =  security.Encrypt("pwd", "1234") });

        modelBuilder.Entity("TattooStudioBooking.Admin").HasData(new { Id = 1, UserName = "Admin", PasswordStored =  security.Encrypt("pwd", "1234"), IsAdmin = true, Email = "p.guerraogalde@outlook.cl" });

        modelBuilder.Entity("TattooStudioBooking.PuestoTrabajo").HasData(new { Id = 1, Nombre = "Puesto Danu" });
        modelBuilder.Entity("TattooStudioBooking.PuestoTrabajo").HasData(new { Id = 2, Nombre = "Puesto Rulos" });
        modelBuilder.Entity("TattooStudioBooking.PuestoTrabajo").HasData(new { Id = 3, Nombre = "Puesto Tere" });
        modelBuilder.Entity("TattooStudioBooking.PuestoTrabajo").HasData(new { Id = 4, Nombre = "Puesto Aless" });

    }

    // The following configures EF to create a Sqlite database file in the
    // special "local" folder for your platform.
    //protected override void OnConfiguring(DbContextOptionsBuilder options)
    //    => options.UseSqlite($"Data Source={DbPath}");
}
