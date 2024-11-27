using System.Text.Json.Serialization;

namespace CoreContable.Models.Dto;

public class RepositorioDto
{
    public string COD_CIA { get; set; }
    public string PERIODO { get; set; }
    public string TIPO_DOCTO { get; set; }
    public string? NUM_POLIZA { get; set; }
    public string? NUM_REFERENCIA { get; set; }
    public string? FECHA { get; set; }
    public string ANIO { get; set; }
    public string MES { get; set; }
    public string? CONCEPTO { get; set; }
    public string? TOTAL_POLIZA { get; set; }
    public string? STAT_POLIZA { get; set; }
    // public DateTime? FECHA_CAMBIO { get; set; }
    public string? FECHA_CAMBIO { get; set; }

    [JsonIgnore]
    public string? GRABACION_USUARIO { get; set; }

    [JsonIgnore]
    public DateTime? GRABACION_FECHA { get; set; }

    [JsonIgnore]
    public string? MODIFICACION_USUARIO { get; set; }

    [JsonIgnore]
    public DateTime? MODIFICACION_FECHA { get; set; }

    [JsonIgnore]
    public string? OPERACION { get; set; }
}