namespace LoccarDomain.Veiculo.Models
{
    public class VeiculoPassageiro : VeiculoBase
    {
        public bool ArCondicionado {  get; set; }
        public bool DirecaoHidraulica { get; set; }
        public bool Televisao { get; set; }
        public int CapacidadePassageiros { get; set; }
    }
}
