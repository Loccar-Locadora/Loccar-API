namespace LoccarDomain.Veiculo.Models
{
    public class VeiculoCarga : VeiculoBase
    {
        public int Tara { get; set; }
        public int TamanhoCompartimento { get; set; }
        public string TipoCarga { get; set; }
        public int CapacidadeCarga { get; set; }
    }
}
