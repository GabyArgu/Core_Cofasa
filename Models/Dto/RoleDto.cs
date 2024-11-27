namespace CoreContable.Models.Dto
{
    public class RoleDto
    {
        public int? Id { get; set; }
        public required string Name { get; set; }
        public bool? Active { get; set; }
        public required string CodCia { get; set; }
        public required string idsPermissions { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
