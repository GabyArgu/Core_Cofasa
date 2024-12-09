using System.ComponentModel.DataAnnotations;

namespace CoreContable.Entities.Views;

public class CuentasContablesView
{

    [MaxLength(3)]
    public required string COD_CIA { get; set; }

    [MaxLength(77)]
    public required string CuentasConcatenadas { get; set; }

    [MaxLength(72)]
    public required string CuentaContable { get; set; }

    [MaxLength(11)]
    public string? Cta_CONTABLE { get; set; }

    public int? Cta_Nivel { get; set; }

    [MaxLength(70)]
    public string? DESCRIP_ESP { get; set; }

    [MaxLength(70)]
    public string? DESCRIP_ING { get; set; }

    public string? ACEP_MOV { get; set; }
    public string? ACEP_PRESUP { get; set; }
    public string? ACEP_ACTIV { get; set; }

    [MaxLength(30)]
    public string? GRUPO_CTA { get; set; }

    public string? CLASE_SALDO { get; set; }

    public string? CTA_FLUJO { get; set; }

    [MaxLength(250)]
    public string? UTIL_CTA { get; set; }

    public string? ACEP_PRESUP_COMPRAS { get; set; }

    [MaxLength(2)]
    public string? BANDERA { get; set; }

    [MaxLength(50)]
    public string? UsuarioCreacion { get; set; }
    [MaxLength(50)]
    public string? UsuarioModificacion { get; set; }
    
    public DateTime? FechaCreacion { get; set; }
    public DateTime? FechaModificacion { get; set; }
    public string? Catalogo { get; set; }
    public int? Grupo_Nivel { get; set; }
    public string? Sub_Grupo { get; set; }
}