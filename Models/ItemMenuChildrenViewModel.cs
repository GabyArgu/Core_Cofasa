using CoreContable.Entities;

namespace CoreContable.Models
{
    public class ItemMenuChildrenViewModel
    {
        public int? ChildrenId { get; set; }
        public String ChildrenName { get; set; }
        public String ChildrenAlias { get; set; }
        public String ChildrenUrl { get; set; }
        public String ChildrenIcon { get; set; }
        public bool ChildrenVisibility { get; set; }
    }
}
