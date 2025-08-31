using System;
using System.Collections.Generic;

namespace LoccarInfra.ORM.model;

public partial class Locatario
{
    public int Idlocatario { get; set; }

    public string Nome { get; set; } = null!;

    public string? Email { get; set; }

    public string? Telefone { get; set; }

    public bool? Locador { get; set; }

    public string Login { get; set; } = null!;

    public string Senha { get; set; } = null!;

    public virtual PessoaFisica? PessoaFisica { get; set; }

    public virtual PessoaJuridica? PessoaJuridica { get; set; }

    public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
}
