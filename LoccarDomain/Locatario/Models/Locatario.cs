namespace LoccarDomain.Locatario.Models
{
    public class Locatario
    {
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Cellphone { get; set; }
        public string? Cnh { get; set; }
        public DateTime? Created { get; set; }
        public int? Locacoes { get; set; }
    }
}
