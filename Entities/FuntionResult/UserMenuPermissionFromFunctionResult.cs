using Microsoft.EntityFrameworkCore;

namespace CoreContable.Entities
{
    [Keyless]
    public class UserMenuPermissionFromFunctionResult
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        // public int RoleId { get; set; }
        // public string RoleName { get; set; }
        public int PerId { get; set; }
        public string PerName { get; set; }
        public string PerUrl { get; set; }
        public string PerIcon { get; set; }
        public int? PerIdFather { get; set; }
        public bool PerVisibility { get; set; }
        public required string PerAlias { get; set; }
        public int PerOrder { get; set; }
        public string PerType { get; set; }
    }
}
