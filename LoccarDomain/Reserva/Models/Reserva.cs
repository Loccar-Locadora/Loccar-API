using LoccarDomain.Veiculo.Models;
using LoccarDomain.Locatario.Models;

namespace LoccarDomain.Reserva.Models;

public class Reserva
{
    public int Diarias { get; set; }
    public int NumReserva { get; set; }
    public float ValorSeguroTerceiro { get; set; }
    public float ValorSeguroCarro { get; set; }
    public float ValorImposto { get; set; }
    public float ValorTotal { get; set; }
    public float ValorDiaria { get; set; }
    public float ValorDiarias { get; set; }
    public bool SeguroCarro { get; set; }
    public string TipoDiaria { get; set; }
    public string DataLocacao { get; set; }
    public string HoraLocacao { get; set; }
    public string DataEntrega { get; set; }
    public string HoraEntrega { get; set; }
    public PessoaFisica Locador { get; set; }
    public VeiculoBase VeiculoLocado { get; set; }
    public PessoaJuridica Empresa { get; set; }
}
