using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CoreContable.Utils;
using Microsoft.EntityFrameworkCore;

namespace CoreContable.Entities;


[Table(CC.DMGNUMERA, Schema = CC.SCHEMA)]
public class DmgNumera
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    [MaxLength(3)]
    public string COD_CIA { get; set; }

    [MaxLength(2)]
    public string TIPO_DOCTO { get; set; }
    
    public int ANIO { get; set; }

    public int MES { get; set; }

    public int CONTADOR_POLIZA { get; set; }

    [MaxLength(30)]
    public string? UsuarioCreacion { get; set; }

    public DateTime? FechaCreacion { get; set; }

    [MaxLength(30)]
    public string? UsuarioModificacion { get; set; }

    public DateTime? FechaModificacion { get; set; }
}