namespace LoccarDomain.Vehicle.Models
{
    public class Motorcycle : Vehicle
    {
        public bool? TractionControl { get; set; }

        public bool? AbsBrakes { get; set; }

        public bool? CruiseControl { get; set; }
    }
}
