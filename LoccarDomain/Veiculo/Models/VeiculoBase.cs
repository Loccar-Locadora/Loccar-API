namespace LoccarDomain.Veiculo.Models
{
    public class VeiculoBase
    {
        public bool Reservado { get; set; } = false;
        public string Marca { get; set; }
        public string Modelo { get; set; }
        public string Tipo { get; set; }
        public int AnoFabricacao { get; set; }
        public int AnoModelo { get; set; }
        public int Renavam { get; set; }
        public int CapacidadeTanque { get; set; }
        public float ValorDiaria { get; set; }
        public float ValorDiariaReduzida { get; set; }
        public float ValorDiariaEmpresarial { get; set; }
        public float ValorDiariaMensal { get; set; }
    }
}
