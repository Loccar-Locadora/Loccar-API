using System;
using System.Collections.Generic;

namespace LoccarInfra.ORM.model;

public partial class VeiculoCarga
{
    public int Idveiculo { get; set; }

    public decimal? Capacidadecarga { get; set; }

    public string? Tipocarga { get; set; }

    public decimal? Tara { get; set; }

    public string? Tamanhocompartimentocarga { get; set; }

    public virtual Veiculo IdveiculoNavigation { get; set; } = null!;
}
