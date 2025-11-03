namespace LoccarDomain.Statistics.Models
{
    public class UserStatisticsData
    {
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public Dictionary<string, int> RoleCounts { get; set; } = new Dictionary<string, int>();
    }
}
