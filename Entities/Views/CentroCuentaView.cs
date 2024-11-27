using System.ComponentModel.DataAnnotations;

namespace CoreContable.Entities.Views;

public class CentroCuentaView
{
    [MaxLength(3)]
    public required string COD_CIA { get; set; }

    [MaxLength(25)]
    public required string CENTRO_COSTO { get; set; }

    public required string CuentasConcatenadas { get; set; }
    public required string CuentaContable { get; set; }

    public required int CTA_1 { get; set; }
    public required int CTA_2 { get; set; }
    public required int CTA_3 { get; set; }
    public required int CTA_4 { get; set; }
    public required int CTA_5 { get; set; }
    public required int CTA_6 { get; set; }

    [MaxLength(1)]
    public required string ESTADO { get; set; }
    
    public DateTime? FECHA_CREACION { get; set; }
    public DateTime? FECHA_MODIFICACION { get; set; }
    [MaxLength(50)]
    public string? USUARIO_CREACION { get; set; }
    [MaxLength(50)]
    public string? USUARIO_MODIFICACION { get; set; }

    public string? Descripcion_CTA { get; set; }
    public string Cta_CONTABLE { get; internal set; }
}