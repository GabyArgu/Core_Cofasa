using Microsoft.EntityFrameworkCore;

namespace CoreContable.Entities.FuntionResult;

[Keyless]
public class ValidateUserOnLoginFromFunctionResult
{
    public required string CodCia { get; set; }
    public required string CiaName { get; set; }
    public required int UserId { get; set; }
    public required string UserName { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    // public required int RoleId { get; set; }
    // public required string RoleName { get; set; }
    public required int PerId { get; set; }
    public required string PerName { get; set; }
    public required string PerUrl { get; set; }
    public string? PerIcon { get; set; }
    public int? PerIdFather { get; set; }
    public required bool PerVisibility { get; set; }
    public required string PerAlias { get; set; }
    public required int PerOrder { get; set; }
    public required string PerType { get; set; }
}