namespace CoreContable.Models;

public class CiaDto {
    public string? COD_CIA { get; set; }
    public string? RAZON_SOCIAL { get; set; }
    public string? NOM_COMERCIAL { get; set; }
    public string? NOM_CORTO { get; set; }
    public string? DIREC_EMPRESA { get; set; }
    public string? TELEF_EMPRESA { get; set; }
    public string? NIT_EMPRESA { get; set; }
    public string? NUMERO_PATRONAL { get; set; }
    public int? MES_CIERRE { get; set; }
    public int? MES_PROCESO { get; set; }
    public int? PERIODO { get; set; }
    public double? IVA_PORC { get; set; }
    public string? ND_IVA { get; set; }
    public string? FD_IVA { get; set; } // fecha
    public double? TASA_CAM { get; set; }
    public double? ISR_PORC { get; set; }
    public string? ND_ISR { get; set; }
    public string? FD_ISR { get; set; } // fecha
    public double? VAL_MIN_DEPRECIAR { get; set; }
    public int? CTA_1RESUL_ACT { get; set; }
    public int? CTA_2RESUL_ACT { get; set; }
    public int? CTA_3RESUL_ACT { get; set; }
    public int? CTA_4RESUL_ACT { get; set; }
    public int? CTA_5RESUL_ACT { get; set; }
    public int? CTA_6RESUL_ACT { get; set; }
    public string? NOM_RESUL_ACT { get; set; }
    public int? CTA_1RESUL_ANT { get; set; }
    public int? CTA_2RESUL_ANT { get; set; }
    public int? CTA_3RESUL_ANT { get; set; }
    public int? CTA_4RESUL_ANT { get; set; }
    public int? CTA_5RESUL_ANT { get; set; }
    public int? CTA_6RESUL_ANT { get; set; }
    public string? NOM_RESUL_ANT { get; set; }
    public int? CTA_1PER_GAN { get; set; }
    public int? CTA_2PER_GAN { get; set; }
    public int? CTA_3PER_GAN { get; set; }
    public int? CTA_4PER_GAN { get; set; }
    public int? CTA_5PER_GAN { get; set; }
    public int? CTA_6PER_GAN { get; set; }
    public string? NOM_PER_GAN { get; set; }
    public int? INGRESO_CTA1 { get; set; }
    public int? INGRESO_CTA2 { get; set; }
    public int? INGRESO_CTA3 { get; set; }
    public int? INGRESO_CTA4 { get; set; }
    public int? INGRESO_CTA5 { get; set; }
    public int? INGRESO_CTA6 { get; set; }
    public string? NOM_INGRESO { get; set; }
    public int? GASTO_CTA1 { get; set; }
    public int? GASTO_CTA2 { get; set; }
    public int? GASTO_CTA3 { get; set; }
    public int? GASTO_CTA4 { get; set; }
    public int? GASTO_CTA5 { get; set; }
    public int? GASTO_CTA6 { get; set; }
    public string? NOM_GASTO { get; set; }
    public int? COSTO_CTA1 { get; set; }
    public int? COSTO_CTA2 { get; set; }
    public int? COSTO_CTA3 { get; set; }
    public int? COSTO_CTA4 { get; set; }
    public int? COSTO_CTA5 { get; set; }
    public int? COSTO_CTA6 { get; set; }
    public string? NOM_COSTO { get; set; }
    public string? FEC_ULT_CIE { get; set; } // fecha
    public string? FECH_ULT { get; set; } // fecha
    public double? TASA_IVA { get; set; }
    public int? MESES_CHQ { get; set; }
    public double? PRB_PORC { get; set; }
    public string? ND_PRB { get; set; }
    public string? FD_PRB { get; set; }
    public double? PRS_PORC { get; set; }
    public string? ND_PRS { get; set; }
    public string? FD_PRS { get; set; }
    public string? COD_MONEDA { get; set; }

    public string? DUP_DET_PARTIDAD { get; set; } // checkbox
    public string? APLICA_POLIZA_IMPUESTO { get; set; } // checkbox
    public string? CONTA_X_CLIENTE { get; set; } // checkbox
}