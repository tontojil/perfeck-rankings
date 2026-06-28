using Microsoft.EntityFrameworkCore;
using Api.Models;

namespace Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Jugador> Jugadores => Set<Jugador>();
    public DbSet<Torneo> Torneos => Set<Torneo>();
    public DbSet<Participacion> Participaciones => Set<Participacion>();
    public DbSet<Ranking> Rankings => Set<Ranking>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Jugador>(e =>
        {
            e.HasIndex(j => j.Nombre);
        });

        modelBuilder.Entity<Torneo>(e =>
        {
            e.HasIndex(t => t.Estado);
        });

        modelBuilder.Entity<Participacion>(e =>
        {
            e.HasOne(p => p.Jugador)
                .WithMany(j => j.Participaciones)
                .HasForeignKey(p => p.JugadorId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(p => p.Torneo)
                .WithMany(t => t.Participaciones)
                .HasForeignKey(p => p.TorneoId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Datos de prueba
        modelBuilder.Entity<Jugador>().HasData(
            new Jugador { Id = 1, Nombre = "Perfeck", Elo = 1850, Avatar = "https://ui-avatars.com/api/?name=Perfeck&background=000000&color=00ffff&size=30" },
            new Jugador { Id = 2, Nombre = "ShadowX", Elo = 1720, Avatar = "https://ui-avatars.com/api/?name=ShadowX&background=000000&color=ff5050&size=30" },
            new Jugador { Id = 3, Nombre = "DragonFire", Elo = 1680, Avatar = "https://ui-avatars.com/api/?name=DragonFire&background=000000&color=ffc439&size=30" },
            new Jugador { Id = 4, Nombre = "NeonStar", Elo = 1550, Avatar = "https://ui-avatars.com/api/?name=NeonStar&background=000000&color=10b981&size=30" },
            new Jugador { Id = 5, Nombre = "IceWolf", Elo = 1420, Avatar = "https://ui-avatars.com/api/?name=IceWolf&background=000000&color=3b82f6&size=30" }
        );

        modelBuilder.Entity<Torneo>().HasData(
            new Torneo { Id = 1, Nombre = "Torneo de Verano 2026", Formato = "EliminacionSimple", Estado = "EnCurso" },
            new Torneo { Id = 2, Nombre = "Liga Primavera", Formato = "Liga", Estado = "Inscripcion" },
            new Torneo { Id = 3, Nombre = "Copa Nocturna", Formato = "DobleEliminacion", Estado = "Finalizado" }
        );

        modelBuilder.Entity<Participacion>().HasData(
            new Participacion { Id = 1, JugadorId = 1, TorneoId = 1, Posicion = 1, PuntosGanados = 100 },
            new Participacion { Id = 2, JugadorId = 2, TorneoId = 1, Posicion = 2, PuntosGanados = 75 },
            new Participacion { Id = 3, JugadorId = 3, TorneoId = 1, Posicion = 3, PuntosGanados = 50 },
            new Participacion { Id = 4, JugadorId = 1, TorneoId = 3, Posicion = 1, PuntosGanados = 150 },
            new Participacion { Id = 5, JugadorId = 4, TorneoId = 3, Posicion = 2, PuntosGanados = 100 },
            new Participacion { Id = 6, JugadorId = 5, TorneoId = 3, Posicion = 3, PuntosGanados = 75 },
            new Participacion { Id = 7, JugadorId = 2, TorneoId = 2, Posicion = 0, PuntosGanados = 0 },
            new Participacion { Id = 8, JugadorId = 3, TorneoId = 2, Posicion = 0, PuntosGanados = 0 }
        );
    }
}
