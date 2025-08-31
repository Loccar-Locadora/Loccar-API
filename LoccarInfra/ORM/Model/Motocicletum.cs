using System;
using System.Collections.Generic;

namespace LoccarInfra.ORM.model;

public partial class Motocicletum
{
    public int Idveiculo { get; set; }

    public bool? Controletracao { get; set; }

    public bool? Freioabs { get; set; }

    public bool? Pilotoautomatico { get; set; }

    public virtual Veiculo IdveiculoNavigation { get; set; } = null!;
}
