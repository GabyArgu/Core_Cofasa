using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CoreContable.Utils;

namespace CoreContable.Entities;

[Table(CC.DMGDOCTOS, Schema = CC.SCHEMA)]
public class DmgDoctos
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    [MaxLength(3)]
    public required string COD_CIA { get; set; }

    public string? TIPO_DOCTO { get; set; }
    public int? CONTADOR_POLIZA { get; set; }
    public string? DESCRIP_TIPO { get; set; }
    public string? PROCESO { get; set; }
    public string? POLIZA_MANUAL { get; set; }
    public string? UsuarioCreacion { get; set; }
    public DateTime? FechaCreacion { get; set; }
    public string? UsuarioModificacion { get; set; }
    public DateTime? FechaModificacion { get; set; }
}