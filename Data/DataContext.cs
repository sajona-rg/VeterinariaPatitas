using Microsoft.EntityFrameworkCore;
using VeterinariaMVC.Models;

namespace VeterinariaMVC.Data;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options) { }

    public DbSet<Propietario> Propietarios => Set<Propietario>();
    public DbSet<Especie> Especies => Set<Especie>();
    public DbSet<Raza> Razas => Set<Raza>();
    public DbSet<Mascota> Mascotas => Set<Mascota>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Propietario>(e =>
        {
            e.ToTable("Propietarios");
            e.Property(p => p.Nombre).HasColumnType("varchar(50)");
            e.Property(p => p.Apellido).HasColumnType("varchar(50)");
            e.Property(p => p.Telefono).HasColumnType("varchar(20)");
            e.Property(p => p.Email).HasColumnType("varchar(100)");
            e.Property(p => p.Direccion).HasColumnType("varchar(150)");
        });

        modelBuilder.Entity<Especie>(e =>
        {
            e.ToTable("Especies");
            e.Property(x => x.Nombre).HasColumnType("varchar(50)");
        });

        modelBuilder.Entity<Raza>(e =>
        {
            e.ToTable("Razas");
            e.Property(x => x.Nombre).HasColumnType("varchar(50)");
            e.HasOne(r => r.Especie)
             .WithMany(es => es.Razas)
             .HasForeignKey(r => r.IdEspecie)
             .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Mascota>(e =>
        {
            e.ToTable("Mascotas");
            e.Property(x => x.Nombre).HasColumnType("varchar(50)");
            e.Property(x => x.RutaFoto).HasColumnType("varchar(255)");

            e.HasOne(m => m.Propietario)
             .WithMany(p => p.Mascotas)
             .HasForeignKey(m => m.IdPropietario)
             .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(m => m.Raza)
             .WithMany(r => r.Mascotas)
             .HasForeignKey(m => m.IdRaza)
             .OnDelete(DeleteBehavior.Restrict);
        });

        // Datos semilla de catálogos
        modelBuilder.Entity<Especie>().HasData(
            new Especie { Id = 1, Nombre = "Canino" },
            new Especie { Id = 2, Nombre = "Felino" },
            new Especie { Id = 3, Nombre = "Ave" });

        modelBuilder.Entity<Raza>().HasData(
            new Raza { Id = 1, IdEspecie = 1, Nombre = "Labrador Retriever" },
            new Raza { Id = 2, IdEspecie = 1, Nombre = "Pastor Alemán" },
            new Raza { Id = 3, IdEspecie = 1, Nombre = "Bulldog Francés" },
            new Raza { Id = 4, IdEspecie = 1, Nombre = "Criollo" },
            new Raza { Id = 5, IdEspecie = 2, Nombre = "Siamés" },
            new Raza { Id = 6, IdEspecie = 2, Nombre = "Persa" },
            new Raza { Id = 7, IdEspecie = 2, Nombre = "Angora" },
            new Raza { Id = 8, IdEspecie = 3, Nombre = "Periquito" },
            new Raza { Id = 9, IdEspecie = 3, Nombre = "Canario" });

    }
}
