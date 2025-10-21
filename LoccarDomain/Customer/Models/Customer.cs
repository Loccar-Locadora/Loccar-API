namespace LoccarDomain.Customer.Models
{
    public class Customer
    {
        public int? IdCustomer { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Cellphone { get; set; }
        public string? DriverLicense { get; set; }
        public DateTime? Created { get; set; }
        public bool Authenticated { get; set; }
    }
}
