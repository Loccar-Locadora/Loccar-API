namespace LoccarDomain.Statistics.Models
{
    public class UserStatistics
    {
        public int TotalUsers { get; set; }
        public int AdminUsers { get; set; }
        public int EmployeeUsers { get; set; }
        public int CommonUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int InactiveUsers { get; set; }
        public DateTime GeneratedAt { get; set; }
    }

    public class UserRoleBreakdown
    {
        public string RoleName { get; set; }
        public int UserCount { get; set; }
        public decimal Percentage { get; set; }
    }

    public class DetailedUserStatistics
    {
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int InactiveUsers { get; set; }
        public List<UserRoleBreakdown> RoleBreakdown { get; set; }
        public DateTime GeneratedAt { get; set; }
    }
}
