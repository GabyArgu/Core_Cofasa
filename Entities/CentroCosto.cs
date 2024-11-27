using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CoreContable.Utils;

namespace CoreContable.Entities;

[Table(CC.CENTRO_COSTO, Schema = CC.SCHEMA)]
public class CentroCosto
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    [MaxLength(3)]
    public required string COD_CIA { get; set; }

    [MaxLength(25)]
    public required string CENTRO_COSTO { get; set; }

    [MaxLength(200)]
    public required string DESCRIPCION { get; set; }

    [MaxLength(1)]
    public required string ACEPTA_DATOS { get; set; }

    public DateTime? FECHA_CREACION { get; set; }
    public DateTime? FECHA_MODIFICACION { get; set; }
    [MaxLength(50)]
    public string? USUARIO_CREACION { get; set; }
    [MaxLength(50)]
    public string? USUARIO_MODIFICACION { get; set; }
}