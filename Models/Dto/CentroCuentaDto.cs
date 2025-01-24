namespace CoreContable.Models.Dto;

public class CentroCuentaDto
{
    public string? isUpdating { get; set; }
    public required string COD_CIA { get; set; }
    public required string CENTRO_COSTO { get; set; }
    public string CENTRO_CUENTA { get; set; }

    public required int CTA_1 { get; set; }
    public required int CTA_2 { get; set; }
    public required int CTA_3 { get; set; }
    public required int CTA_4 { get; set; }
    public required int CTA_5 { get; set; }
    public required int CTA_6 { get; set; }

    public string? ESTADO { get; set; }

    public DateTime? FECHA_CREACION { get; set; }
    public DateTime? FECHA_MODIFICACION { get; set; }
    public string? USUARIO_CREACION { get; set; }
    public string? USUARIO_MODIFICACION { get; set; }
}