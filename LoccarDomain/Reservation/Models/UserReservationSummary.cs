namespace LoccarDomain.Reservation.Models;

public class UserReservationSummary
{
    public int ActiveCount { get; set; }
    public int CompletedCount { get; set; }
    public int CancelledCount { get; set; }
    public List<UserReservationDetail> ActiveReservations { get; set; } = new List<UserReservationDetail>();
    public List<UserReservationDetail> CompletedReservations { get; set; } = new List<UserReservationDetail>();
    public List<UserReservationDetail> CancelledReservations { get; set; } = new List<UserReservationDetail>();
}
