namespace LoccarDomain.Vehicle.Models
{
    public class PassengerVehicle : Vehicle
    {
        public int? PassengerCapacity { get; set; }

        public bool? Tv { get; set; }

        public bool? AirConditioning { get; set; }

        public bool? PowerSteering { get; set; }
    }
}
