namespace CoreContable.Models.ResultSet
{
    public class UserAppResultSet
    {
        public int Id { get; set; }
        
        public required string UserName { get; set; }
        
        public string? Email { get; set; }

        public required string FirstName { get; set; }

        public required string LastName { get; set; }

        public bool? Active { get; set; } = true;

        public DateTime? CreatedAt { get; set; }

        public List<Select2ResultSet>? SelRoles { get; set; }
        public List<int>? OldRoles { get; set; }
        
        public List<Select2ResultSet>? SelCias { get; set; }
        public List<string>? OldCias { get; set; }

        // public DateTime? UpdatedAt { get; set; }
        //
        // public DateTime? DeletedAt { get; set; }
    }
}
