namespace CoreContable.Models.ResultSet;

public class DataTableResultSet<T>
{
    public bool success { get; set; }
    public string message { get; set; }
    public T data { get; set; }
    public int recordsFiltered { get; set; }
    public int draw { get; set; }
    public int recordsTotal { get; set; }
}