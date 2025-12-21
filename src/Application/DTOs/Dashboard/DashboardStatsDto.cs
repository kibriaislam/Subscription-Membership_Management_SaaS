namespace Application.DTOs.Dashboard;

public class DashboardStatsDto
{
    public int TotalMembers { get; set; }
    public int ActiveMembers { get; set; }
    public int ExpiredMembers { get; set; }
    public int RenewalsDueToday { get; set; }
    public int RenewalsDueThisWeek { get; set; }
    public decimal MonthlyCollection { get; set; }
    public decimal TotalOutstanding { get; set; }
}

