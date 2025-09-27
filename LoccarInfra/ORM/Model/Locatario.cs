using System;
using System.Collections.Generic;

namespace LoccarInfra.ORM.model;

public partial class Locatario
{
    public int Idlocatario { get; set; }

    public string? Nome { get; set; }

    public string Email { get; set; } = null!;

    public string? Telefone { get; set; }

    public string Cnh { get; set; } = null!;

    public DateTime? Created { get; set; }

    public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
}
