using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CoreContable.Models.Dto;
using CoreContable.Utils;

namespace CoreContable.Entities;

[Table(CC.DET_REPOSITORIO, Schema = CC.SCHEMA)]
public class DetRepositorio
{

    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    [MaxLength(3)]
    public string COD_CIA { get; set; }

    public int PERIODO { get; set; }

    [MaxLength(2)]
    public string TIPO_DOCTO { get; set; }

    public int NUM_POLIZA { get; set; }

    public int CORRELAT { get; set; }

    public int CTA_1 { get; set; }

    public int CTA_2 { get; set; }

    public int CTA_3 { get; set; }

    public int CTA_4 { get; set; }

    public int CTA_5 { get; set; }

    public int CTA_6 { get; set; }

    [MaxLength(400)]
    public string? CONCEPTO { get; set; }

    public double? CARGO { get; set; }

    public double? ABONO { get; set; }

    [MaxLength(25)]
    public string CENTRO_COSTO { get; set; }
}

internal static class DetRepositorioExtensions
{
    public static DetRepositorio ToEntity(this DetRepositorio entity, DetRepositorioDto dto)
    {
        entity.PERIODO = dto.det_PERIODO ?? 0;
        entity.TIPO_DOCTO = dto.det_TIPO_DOCTO;
        entity.NUM_POLIZA = dto.det_NUM_POLIZA;
        entity.CORRELAT = dto.det_CORRELAT ?? 0;
        entity.CTA_1 = dto.CTA_1;
        entity.CTA_2 = dto.CTA_2;
        entity.CTA_3 = dto.CTA_3;
        entity.CTA_4 = dto.CTA_4;
        entity.CTA_5 = dto.CTA_5;
        entity.CTA_6 = dto.CTA_6;
        entity.CONCEPTO = dto.det_CONCEPTO;
        entity.CARGO = dto.CARGO;
        entity.ABONO = dto.ABONO;
        entity.CENTRO_COSTO = dto.CENTRO_COSTO;
        return entity;
    }
}