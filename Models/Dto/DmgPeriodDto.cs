namespace CoreContable.Models.Dto;

public class DmgPeriodDto
{
    public required string CodCia { get; set; }
    public required int Period { get; set; }
    public DateTime? Opened { get; set; }
    public DateTime? Closed { get; set; }
    public required string Status { get; set; }
    public required string StartMonth { get; set; }
    public required string FinishMonth { get; set; }
    public required string CreatedBy { get; set; }
    public required DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
}