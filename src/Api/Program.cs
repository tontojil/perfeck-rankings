using Microsoft.EntityFrameworkCore;
using Api.Data;
using Api.Models;

var builder = WebApplication.CreateBuilder(args);

var connString = builder.Configuration.GetConnectionString("DefaultConnection");
if (!string.IsNullOrEmpty(connString))
{
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(connString));
}
else
{
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseInMemoryDatabase("PerfeckRankingsDB"));
}

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

app.UseCors();
app.UseSwagger();
app.UseSwaggerUI();

// Health check para Azure Load Balancer
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }));

// Rankings
app.MapGet("/api/rankings", async (AppDbContext db) =>
{
    var jugadores = await db.Jugadores
        .OrderByDescending(j => j.Elo)
        .Select(j => new {
            j.Id, j.Nombre, j.Avatar, j.Elo,
            TotalTorneos = j.Participaciones.Count,
            Victorias = j.Participaciones.Count(p => p.Posicion == 1)
        })
        .ToListAsync();

    return Results.Ok(jugadores);
});

app.MapGet("/api/rankings/{id:int}", async (int id, AppDbContext db) =>
{
    var ranking = await db.Rankings.FindAsync(id);
    return ranking is null ? Results.NotFound() : Results.Ok(ranking);
});

// Jugadores
app.MapGet("/api/jugadores", async (AppDbContext db) =>
{
    var data = await db.Jugadores
        .OrderByDescending(j => j.Elo)
        .Select(j => new { j.Id, j.Nombre, j.Avatar, j.Elo, j.FechaRegistro })
        .ToListAsync();
    return Results.Ok(data);
});

app.MapGet("/api/jugadores/{id:int}", async (int id, AppDbContext db) =>
{
    var j = await db.Jugadores
        .Include(x => x.Participaciones)
        .ThenInclude(x => x.Torneo)
        .FirstOrDefaultAsync(x => x.Id == id);

    if (j is null) return Results.NotFound();

    return Results.Ok(new
    {
        j.Id, j.Nombre, j.Avatar, j.Elo, j.FechaRegistro,
        Torneos = j.Participaciones.Select(p => new {
            TorneoId = p.Torneo!.Id,
            TorneoNombre = p.Torneo.Nombre,
            p.Posicion,
            p.PuntosGanados
        })
    });
});

app.MapPost("/api/jugadores", async (Jugador jugador, AppDbContext db) =>
{
    jugador.FechaRegistro = DateTime.UtcNow;
    db.Jugadores.Add(jugador);
    await db.SaveChangesAsync();
    return Results.Created($"/api/jugadores/{jugador.Id}", jugador);
});

// Torneos
app.MapGet("/api/torneos", async (AppDbContext db) =>
{
    var data = await db.Torneos
        .OrderByDescending(t => t.FechaCreacion)
        .Select(t => new {
            t.Id, t.Nombre, t.Formato, t.Estado, t.FechaCreacion,
            Participantes = t.Participaciones.Count
        })
        .ToListAsync();
    return Results.Ok(data);
});

app.MapGet("/api/torneos/{id:int}", async (int id, AppDbContext db) =>
{
    var t = await db.Torneos
        .Include(x => x.Participaciones)
        .ThenInclude(x => x.Jugador)
        .FirstOrDefaultAsync(x => x.Id == id);

    if (t is null) return Results.NotFound();

    return Results.Ok(new
    {
        t.Id, t.Nombre, t.Formato, t.Estado, t.FechaCreacion,
        Clasificacion = t.Participaciones
            .OrderBy(p => p.Posicion)
            .Select(p => new {
                Posicion = p.Posicion,
                JugadorId = p.Jugador!.Id,
                JugadorNombre = p.Jugador.Nombre,
                p.PuntosGanados
            })
    });
});

app.MapPost("/api/torneos", async (Torneo torneo, AppDbContext db) =>
{
    db.Torneos.Add(torneo);
    await db.SaveChangesAsync();
    return Results.Created($"/api/torneos/{torneo.Id}", torneo);
});

app.MapPost("/api/torneos/{id:int}/resultado", async (int id, Participacion resultado, AppDbContext db) =>
{
    var torneo = await db.Torneos.FindAsync(id);
    if (torneo is null) return Results.NotFound();

    resultado.TorneoId = id;
    db.Participaciones.Add(resultado);
    await db.SaveChangesAsync();
    return Results.Created($"/api/torneos/{id}", resultado);
});

// Estadisticas
app.MapGet("/api/stats", async (AppDbContext db) =>
{
    return Results.Ok(new
    {
        TotalJugadores = await db.Jugadores.CountAsync(),
        TotalTorneos = await db.Torneos.CountAsync(),
        TorneosActivos = await db.Torneos.CountAsync(t => t.Estado == "EnCurso"),
        MejorElo = await db.Jugadores.MaxAsync(j => j.Elo),
        PromedioElo = await db.Jugadores.AverageAsync(j => (double)j.Elo)
    });
});

app.Run();
