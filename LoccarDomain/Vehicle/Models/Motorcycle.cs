namespace LoccarDomain.Vehicle.Models
{
    public class Motorcycle : Vehicle
    {

        public int? IdVehicle { get; set; }
        public bool? TractionControl { get; set; }

        public bool? AbsBrakes { get; set; }

        public bool? CruiseControl { get; set; }
    }
}
