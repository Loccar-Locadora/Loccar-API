namespace LoccarDomain.Statistics.Models
{
    public class SystemStatistics
    {
        public int TotalCustomers { get; set; }
        public int TotalVehicles { get; set; }
        public int ActiveReservations { get; set; }
        public int AvailableVehicles { get; set; }
        public DateTime GeneratedAt { get; set; }
    }
}
