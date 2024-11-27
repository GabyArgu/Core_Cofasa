using CoreContable.Entities;

namespace CoreContable.Models.ResultSet;

public class CurrencyResultSet
{
    public CurrencyResultSet()
    {
    }

    public string? MON_CODIGO { get; set; }
    public string? MON_NOMBRE { get; set; }
    public string? MON_SIGLAS { get; set; }
    public string? MON_SIMBOLO { get; set; }
}