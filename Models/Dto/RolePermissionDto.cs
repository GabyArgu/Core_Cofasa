namespace CoreContable.Models.Dto
{
    public class RolePermissionDto
    {
        public int? Id { get; set; }
        public int IdRole { get; set; }
        public int IdPermission { get; set; }

        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
