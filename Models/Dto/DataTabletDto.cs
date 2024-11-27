namespace CoreContable.Models.Dto;

public class DataTabletDto
{
    public int pageIndex { get; set; }
    public int start { get; set; }
    public int length { get; set; }
    public int draw { get; set; }
    public string search { get; set; }
    public int orderIndex { get; set; }
    public string orderDirection { get; set; }
}