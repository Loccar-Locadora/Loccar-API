namespace LoccarDomain.Locatario.Models
{
    public class Locatario
    {
        public string? Nome { get; set; }
        public string? Email { get; set; }
        public string? Telefone { get; set; }
        public Endereco? Endereco { get; set; }
        public bool? Locador { get; set; }
        public int? Locacoes { get; set; }
        public string? Login { get; set; }
        public string? Senha { get; set; }
        public PessoaFisica? PessoaFisica { get; set; }
        public PessoaJuridica? PessoaJuridica { get; set; }
    }
}
