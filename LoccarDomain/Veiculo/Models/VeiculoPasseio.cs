namespace LoccarDomain.Veiculo.Models
{
    public class VeiculoPasseio : VeiculoBase
    {
        public string Categoria { get; set; }
        public bool ArCondicionado { get; set; }
        public bool DirecaoHidraulica { get; set; }
        public bool Automatico { get; set; }

    }
}
