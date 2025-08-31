namespace LoccarDomain.Locatario.Models
{
    public class PessoaJuridica : Locatario
    {
        public string Cnpj { get; set; }
        public List<PessoaFisica>? Funcionarios { get; set; }
    }
}
