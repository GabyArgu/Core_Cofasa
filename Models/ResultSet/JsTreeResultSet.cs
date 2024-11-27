namespace CoreContable.Models.ResultSet;

public class JsTreeResultSet
{
    public int id { get; set; }
    public string text { get; set; }
    public bool icon { get; set; }
    public JSTreeStateResultSet state { get; set; }
    public List<JsTreeResultSet> children { get; set; }
}