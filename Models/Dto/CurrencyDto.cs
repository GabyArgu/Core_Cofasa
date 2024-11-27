namespace CoreContable.Models.Dto;

public class CurrencyDto
{
    public CurrencyDto()
    {
    }

    public string? isUpdating { get; set; }
    public string? MON_CODIGO { get; set; }
    public string? MON_NOMBRE { get; set; }
    public string? MON_SIGLAS { get; set; }
    public string? MON_SIMBOLO { get; set; }
    public string? UsuarioCreacion { get; set; }
    public DateTime? FechaCreacion { get; set; }
    public string? UsuarioModificacion { get; set; }
    public DateTime? FechaModificacion { get; set; }
}