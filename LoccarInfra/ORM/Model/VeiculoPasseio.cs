using System;
using System.Collections.Generic;

namespace LoccarInfra.ORM.model;

public partial class VeiculoPasseio
{
    public int Idveiculo { get; set; }

    public bool? Automatico { get; set; }

    public bool? Direcaohidraulica { get; set; }

    public bool? Arcondicionado { get; set; }

    public string? Categoria { get; set; }

    public virtual Veiculo IdveiculoNavigation { get; set; } = null!;
}
