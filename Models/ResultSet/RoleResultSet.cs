namespace CoreContable.Models.ResultSet
{
    public class RoleResultSet
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public bool? Active { get; set; }
        public required string CodCia { get; set; }

        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
