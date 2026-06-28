using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Models;

public class Jugador
{
    [Key]
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Avatar { get; set; }

    public int Elo { get; set; } = 1000;

    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

    public ICollection<Participacion> Participaciones { get; set; } = new List<Participacion>();
}

public class Torneo
{
    [Key]
    public int Id { get; set; }

    [Required, MaxLength(200)]
    public string Nombre { get; set; } = string.Empty;

    [MaxLength(50)]
    public string Formato { get; set; } = "EliminacionSimple";

    [MaxLength(20)]
    public string Estado { get; set; } = "Inscripcion";

    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    public ICollection<Participacion> Participaciones { get; set; } = new List<Participacion>();
}

public class Participacion
{
    [Key]
    public int Id { get; set; }

    public int JugadorId { get; set; }

    [ForeignKey(nameof(JugadorId))]
    public Jugador? Jugador { get; set; }

    public int TorneoId { get; set; }

    [ForeignKey(nameof(TorneoId))]
    public Torneo? Torneo { get; set; }

    public int Posicion { get; set; }

    public int PuntosGanados { get; set; }
}

public class Ranking
{
    [Key]
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string Categoria { get; set; } = string.Empty;

    [MaxLength(50)]
    public string Criterio { get; set; } = "Elo";

    public string DatosJson { get; set; } = "[]";

    public DateTime FechaActualizacion { get; set; } = DateTime.UtcNow;
}
