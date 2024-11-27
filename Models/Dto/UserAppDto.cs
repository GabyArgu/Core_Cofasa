namespace CoreContable.Models.Dto;

public class UserAppDto {
    public int? Id { get; set; }
    public required string UserName { get; set; }
    public string? Email { get; set; }
    public required string Password { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public bool? Active { get; set; } = false;
    public DateTime? CreatedAt { get; set; } = DateTime.Now;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    // public List<int>? OldRoles { get; set; }
    public string? OldRoles { get; set; }
    // public List<int>? SelRoles { get; set; }
    public string? SelRoles { get; set; }
    public string? SelCias { get; set; }
    public string? OldCias { get; set; }
}