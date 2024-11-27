using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CoreContable.Utils;

namespace CoreContable.Entities;

[Table(CC.CENTRO_CUENTA, Schema = CC.SCHEMA)]
public class CentroCuenta
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    [MaxLength(3)]
    public required string COD_CIA { get; set; }

    [MaxLength(25)]
    public required string CENTRO_COSTO { get; set; }

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
}