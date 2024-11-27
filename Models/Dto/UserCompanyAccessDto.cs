namespace CoreContable.Models.Dto;

public class UserCompanyAccessDto {
    public required int UserId { get; set; }
    public string? OldRoles { get; set; }
    public required string SelCia { get; set; }
    public string? SelRoles { get; set; }
}