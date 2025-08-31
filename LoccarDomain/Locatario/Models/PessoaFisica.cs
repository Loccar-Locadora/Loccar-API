namespace LoccarDomain.Locatario.Models
{
    public class PessoaFisica : Locatario
    {
        public string Cpf { get; set; }
        public string EstadoCivil { get; set; }
        public bool Contratada { get; set; }
    }
}
