using System;
using System.Collections.Generic;

namespace LoccarInfra.ORM.model;

public partial class Veiculo
{
    public int Idveiculo { get; set; }

    public string? Marca { get; set; }

    public string? Modelo { get; set; }

    public int? Anofabricacao { get; set; }

    public int? Anomodelo { get; set; }

    public string? Renavam { get; set; }

    public int? Capacidadetanque { get; set; }

    public decimal? Valordiaria { get; set; }

    public decimal? Valordiariareduzida { get; set; }

    public decimal? Valordiariamensal { get; set; }

    public decimal? Valordiariaempresa { get; set; }

    public virtual Motocicletum? Motocicletum { get; set; }

    public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();

    public virtual VeiculoCarga? VeiculoCarga { get; set; }

    public virtual VeiculoPassageiro? VeiculoPassageiro { get; set; }

    public virtual VeiculoPasseio? VeiculoPasseio { get; set; }
}
