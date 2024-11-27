using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace CoreContable.Entities;

[Keyless]
public class ObtenerDatosRepositorioResult
{
    [MaxLength(3)]
    public string COD_CIA { get; set; }

    public int PERIODO { get; set; }

    [MaxLength(2)]
    public string TIPO_DOCTO { get; set; }

    public int NUM_POLIZA { get; set; }

    [MaxLength(170)]
    public string? NUM_REFERENCIA { get; set; }

    public DateTime? FECHA { get; set; }

    public int ANIO { get; set; }

    public int MES { get; set; }

    [MaxLength(400)]
    public string? CONCEPTO { get; set; }

    // public float? TOTAL_POLIZA { get; set; }
    public double? TOTAL_POLIZA { get; set; }

    public string STAT_POLIZA { get; set; }

    public DateTime? FECHA_CAMBIO { get; set; }

    [MaxLength(30)]
    public string? GRABACION_USUARIO { get; set; }

    public DateTime? GRABACION_FECHA { get; set; }

    [MaxLength(30)]
    public string? MODIFICACION_USUARIO { get; set; }

    public DateTime? MODIFICACION_FECHA { get; set; }

    public double? DiferenciaCargoAbono { get; set; }
}