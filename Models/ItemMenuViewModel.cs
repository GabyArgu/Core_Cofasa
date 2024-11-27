using CoreContable.Entities;

namespace CoreContable.Models
{
    public class ItemMenuViewModel
    {
        public int? FatherId { get; set; }
        public String FatherName { get; set; }
        public String FatherAlias { get; set; }
        public String FatherUrl { get; set; }
        public String FatherIcon { get; set; }
        public bool FatherVisibility { get; set; }
        public ICollection<ItemMenuChildrenViewModel>? ChildrenList { get; set; }
    }
}
