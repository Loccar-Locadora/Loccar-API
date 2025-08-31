using System;
using System.Collections.Generic;

namespace LoccarInfra.ORM.model;

public partial class VeiculoPassageiro
{
    public int Idveiculo { get; set; }

    public int? Capacidadepassageiro { get; set; }

    public bool? Televisao { get; set; }

    public bool? Arcondicionado { get; set; }

    public bool? Direcaohidraulica { get; set; }

    public virtual Veiculo IdveiculoNavigation { get; set; } = null!;
}
