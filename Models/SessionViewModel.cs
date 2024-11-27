namespace CoreContable.Models
{
    public class SessionViewModel
    {
        public required string CiaCode { get; set; }
        public required string CiaName{ get; set; }
        public required int UserId { get; set; }
        public required string UserName { get; set; }
        public required string FullName { get; set; }
        public required string UserPermissions { get; set; }
    }
}
