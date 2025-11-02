namespace LoccarDomain.Statistics.Models
{
    public class MonthlyRevenue
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public string MonthName { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalReservations { get; set; }
        public decimal AverageRevenuePerReservation { get; set; }
        public DateTime GeneratedAt { get; set; }
    }

    public class RevenueBreakdown
    {
        public decimal BaseRevenue { get; set; }
        public decimal InsuranceRevenue { get; set; }
        public decimal TaxRevenue { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    public class MonthlyRevenueDetailed
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public string MonthName { get; set; }
        public RevenueBreakdown Revenue { get; set; }
        public int TotalReservations { get; set; }
        public decimal AverageRevenuePerReservation { get; set; }
        public DateTime GeneratedAt { get; set; }
    }
}
