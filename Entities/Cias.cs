using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CoreContable.Utils;

namespace CoreContable.Entities
{
    [Table(CC.CIAS, Schema = CC.SCHEMA)]
    public class Cias
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [MaxLength(3)]
        [Column("COD_CIA")]
        public required string CodCia { get; set; }

        [MaxLength(60)]
        [Column("RAZON_SOCIAL")]
        public string? RazonSocial { get; set; }

        [MaxLength(60)]
        [Column("NOM_COMERCIAL")]
        public string? NomComercial { get; set; }

        [MaxLength(100)]
        [Column("DIREC_EMPRESA")]
        public string? DirecEmpresa { get; set; }

        [MaxLength(30)]
        [Column("TELEF_EMPRESA")]
        public string? TelefEmpresa { get; set; }

        [MaxLength(25)]
        [Column("NIT_EMPRESA")]
        public string? NitEmpresa { get; set; }

        [MaxLength(15)]
        [Column("NUMERO_PATRONAL")]
        public string? NumeroPatronal { get; set; }

        [Column("MES_CIERRE")]
        public int? MesCierre { get; set; }

        [Column("MES_PROCESO")]
        public int? MesProceso { get; set; }

        [Column("PERIODO")]
        public int? Periodo { get; set; }

        [Column("CTA_1RESUL_ACT")]
        public int? Cta1ResulAct { get; set; }

        [Column("CTA_2RESUL_ACT")]
        public int? Cta2ResulAct { get; set; }

        [Column("CTA_3RESUL_ACT")]
        public int? Cta3ResulAct { get; set; }

        [Column("CTA_4RESUL_ACT")]
        public int? Cta4ResulAct { get; set; }

        [Column("CTA_5RESUL_ACT")]
        public int? Cta5ResulAct { get; set; }

        [Column("CTA_6RESUL_ACT")]
        public int? Cta6ResulAct { get; set; }

        [Column("CTA_1RESUL_ANT")]
        public int? Cta1ResulAnt { get; set; }

        [Column("CTA_2RESUL_ANT")]
        public int? Cta2ResulAnt { get; set; }

        [Column("CTA_3RESUL_ANT")]
        public int? Cta3ResulAnt { get; set; }

        [Column("CTA_4RESUL_ANT")]
        public int? Cta4ResulAnt { get; set; }

        [Column("CTA_5RESUL_ANT")]
        public int? Cta5ResulAnt { get; set; }

        [Column("CTA_6RESUL_ANT")]
        public int? Cta6ResulAnt { get; set; }

        [Column("CTA_1PER_GAN")]
        public int? Cta1PerGan { get; set; }

        [Column("CTA_2PER_GAN")]
        public int? Cta2PerGan { get; set; }

        [Column("CTA_3PER_GAN")]
        public int? Cta3PerGan { get; set; }

        [Column("CTA_4PER_GAN")]
        public int? Cta4PerGan { get; set; }

        [Column("CTA_5PER_GAN")]
        public int? Cta5PerGan { get; set; }

        [Column("CTA_6PER_GAN")]
        public int? Cta6PerGan { get; set; }

        [Column("FECH_ULT")]
        public DateTime? FechUlt { get; set; }

        [Column("FEC_ULT_CIE")]
        public DateTime? FecUltCie { get; set; }

        [Column("TASA_IVA")]
        // public float? TasaIva { get; set; }
        public double? TasaIva { get; set; }

        [Column("MESES_CHQ")]
        public int? MesesChq { get; set; }

        [Column("TASA_CAM")]
        public double? TasaCam { get; set; }

        [Column("IVA_PORC")]
        public double? IvaPorc { get; set; }

        [MaxLength(6)]
        [Column("ND_IVA")]
        public string? NdIva { get; set; }

        [Column("FD_IVA")]
        public DateTime? FdIva { get; set; }

        [Column("ISR_PORC")]
        public double? IsrPorc { get; set; }

        [MaxLength(6)]
        [Column("ND_ISR")]
        public string? NdIsr { get; set; }

        [Column("FD_ISR")]
        public DateTime? FdIsr { get; set; }

        [Column("PRB_PORC")]
        public double? PrbPorc { get; set; }

        [MaxLength(6)]
        [Column("ND_PRB")]
        public string? NdPrb { get; set; }

        [Column("FD_PRB")]
        public DateTime? FdPrb { get; set; }

        [Column("PRS_PORC")]
        public double? PrsPorc { get; set; }

        [MaxLength(6)]
        [Column("ND_PRS")]
        public string? NdPrs { get; set; }

        [Column("FD_PRS")]
        public DateTime? FdPrs { get; set; }

        [MaxLength(4)]
        [Column("COD_MONEDA")]
        public string? CodMoneda { get; set; }

        [MaxLength(1)]
        [Column("DUP_DET_PARTIDAD")]
        public string? DupDetPartidad { get; set; }

        [Column("VAL_MIN_DEPRECIAR")]
        public double? ValMinDepreciar { get; set; }

        [Column("INGRESO_CTA1")]
        public int? IngresoCta1 { get; set; }

        [Column("INGRESO_CTA2")]
        public int? IngresoCta2 { get; set; }

        [Column("INGRESO_CTA3")]
        public int? IngresoCta3 { get; set; }

        [Column("INGRESO_CTA4")]
        public int? IngresoCta4 { get; set; }

        [Column("INGRESO_CTA5")]
        public int? IngresoCta5 { get; set; }

        [Column("INGRESO_CTA6")]
        public int? IngresoCta6 { get; set; }

        [Column("GASTO_CTA1")]
        public int? GastoCta1 { get; set; }

        [Column("GASTO_CTA2")]
        public int? GastoCta2 { get; set; }

        [Column("GASTO_CTA3")]
        public int? GastoCta3 { get; set; }

        [Column("GASTO_CTA4")]
        public int? GastoCta4 { get; set; }

        [Column("GASTO_CTA5")]
        public int? GastoCta5 { get; set; }

        [Column("GASTO_CTA6")]
        public int? GastoCta6 { get; set; }

        [Column("COSTO_CTA1")]
        public int? CostoCta1 { get; set; }

        [Column("COSTO_CTA2")]
        public int? CostoCta2 { get; set; }

        [Column("COSTO_CTA3")]
        public int? CostoCta3 { get; set; }

        [Column("COSTO_CTA4")]
        public int? CostoCta4 { get; set; }

        [Column("COSTO_CTA5")]
        public int? CostoCta5 { get; set; }

        [Column("COSTO_CTA6")]
        public int? CostoCta6 { get; set; }

        [ForeignKey("CodCia")]
        public ICollection<UserCia>? UserCia { get; set; }

        [ForeignKey("CodCia")]
        public ICollection<Role>? Role { get; set; }

    }
}
