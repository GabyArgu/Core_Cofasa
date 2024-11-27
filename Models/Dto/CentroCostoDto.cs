namespace CoreContable.Models.Dto;

public class CentroCostoDto
{
    public string? isUpdating { get; set; }
    public required string COD_CIA { get; set; }
    public required string CENTRO_COSTO { get; set; }
    public required string DESCRIPCION { get; set; }
    public string? ACEPTA_DATOS { get; set; }

    public DateTime? FECHA_CREACION { get; set; }
    public DateTime? FECHA_MODIFICACION { get; set; }
    public string? USUARIO_CREACION { get; set; }
    public string? USUARIO_MODIFICACION { get; set; }
}