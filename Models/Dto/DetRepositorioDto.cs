using System.Text.Json.Serialization;
using CoreContable.Entities;

namespace CoreContable.Models.Dto;

public class DetRepositorioDto
{
    public string? det_COD_CIA { get; set; }
    [JsonIgnore]
    public int? det_PERIODO { get; set; }
    [JsonIgnore]
    public string det_TIPO_DOCTO { get; set; }
    [JsonIgnore]
    public int det_NUM_POLIZA { get; set; }
    public int? det_CORRELAT { get; set; }
    public int CTA_1 { get; set; }
    public int CTA_2 { get; set; }
    public int CTA_3 { get; set; }
    public int CTA_4 { get; set; }
    public int CTA_5 { get; set; }
    public int CTA_6 { get; set; }
    public string? det_CONCEPTO { get; set; }
    public double? CARGO { get; set; }
    public double? ABONO { get; set; }
    [JsonIgnore]
    public string? detOPERACION { get; set; }
    [JsonIgnore]
    public DateTime? FECHA_CAMBIO { get; set; }
    [JsonIgnore]
    public string? GRABACION_USUARIO { get; set; }
    [JsonIgnore]
    public DateTime? GRABACION_FECHA { get; set; }
    [JsonIgnore]
    public string? MODIFICACION_USUARIO { get; set; }
    [JsonIgnore]
    public DateTime? MODIFICACION_FECHA { get; set; }
    
    public string? CENTRO_COSTO { get; set; }
    [JsonIgnore] // TODO: evaluar
    public string? CENTRO_CUENTA { get; set; }
    
    public static DetRepositorioDto fromEntityToDto(DetRepositorio entity)
    {
        return new DetRepositorioDto
        {
            det_COD_CIA = entity.COD_CIA,
            det_PERIODO = entity.PERIODO,
            det_TIPO_DOCTO = entity.TIPO_DOCTO,
            det_NUM_POLIZA = entity.NUM_POLIZA,
            det_CORRELAT = entity.CORRELAT,
            CTA_1 = entity.CTA_1,
            CTA_2 = entity.CTA_2,
            CTA_3 = entity.CTA_3,
            CTA_4 = entity.CTA_4,
            CTA_5 = entity.CTA_5,
            CTA_6 = entity.CTA_6,
            det_CONCEPTO = entity.CONCEPTO,
            CARGO = entity.CARGO,
            ABONO = entity.ABONO,
            CENTRO_COSTO = entity.CENTRO_COSTO
        };
    }
}