using System;
using System.Collections.Generic;

namespace LoccarInfra.ORM.model;

public partial class Reserva
{
    public int Numeroreserva { get; set; }

    public int Idlocatario { get; set; }

    public int Idveiculo { get; set; }

    public DateTime Datalocacao { get; set; }

    public DateTime Dataentrega { get; set; }

    public int? Diarias { get; set; }

    public decimal? Valordiaria { get; set; }

    public string? Tipodiaria { get; set; }

    public decimal? Valorsegurocarro { get; set; }

    public decimal? Valorseguroterceiro { get; set; }

    public decimal? Valorimposto { get; set; }

    public virtual Locatario IdlocatarioNavigation { get; set; } = null!;

    public virtual Veiculo IdveiculoNavigation { get; set; } = null!;
}
