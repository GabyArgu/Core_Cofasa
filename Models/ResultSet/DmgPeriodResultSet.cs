namespace CoreContable.Models.ResultSet;

public class DmgPeriodResultSet
{
    public DmgPeriodResultSet()
    {
    }

    public required string CodCia { get; set; }
    public required string Period { get; set; }
    public required DateTime Opened { get; set; }
    public required DateTime Closed { get; set; }
    public required string Status { get; set; }
    public required string StartMonth { get; set; }
    public required string FinishMonth { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
}