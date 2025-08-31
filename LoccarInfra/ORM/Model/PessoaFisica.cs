using System;
using System.Collections.Generic;

namespace LoccarInfra.ORM.model;

public partial class PessoaFisica
{
    public int Idlocatario { get; set; }

    public string Cpf { get; set; } = null!;

    public string? EstadoCivil { get; set; }

    public bool? Contratado { get; set; }

    public virtual Locatario IdlocatarioNavigation { get; set; } = null!;
}
