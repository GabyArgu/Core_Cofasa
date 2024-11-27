using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CoreContable.Utils;

namespace CoreContable.Entities;

[Table(CC.DMGPARAM, Schema = CC.SCHEMA)]
public class DmgParam
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    [MaxLength(3)]
    [Column("COD_CIA")]
    public required string CodCia { get; set; }

    public int? DT1 { get; set; }
    public int? DT2 { get; set; }
    public int? DT3 { get; set; }
    public int? DT4 { get; set; }
    public int? DT5 { get; set; }
    public int? DT6 { get; set; }

    public int? CXC1 { get; set; }
    public int? CXC2 { get; set; }
    public int? CXC3 { get; set; }
    public int? CXC4 { get; set; }
    public int? CXC5 { get; set; }
    public int? CXC6 { get; set; }

    public int? CXP1 { get; set; }
    public int? CXP2 { get; set; }
    public int? CXP3 { get; set; }
    public int? CXP4 { get; set; }
    public int? CXP5 { get; set; }
    public int? CXP6 { get; set; }

    public int? EPF1 { get; set; }
    public int? EPF2 { get; set; }
    public int? EPF3 { get; set; }
    public int? EPF4 { get; set; }
    public int? EPF5 { get; set; }
    public int? EPF6 { get; set; }

    public int? IVA1 { get; set; }
    public int? IVA2 { get; set; }
    public int? IVA3 { get; set; }
    public int? IVA4 { get; set; }
    public int? IVA5 { get; set; }
    public int? IVA6 { get; set; }

    public int? CXPE1 { get; set; }
    public int? CXPE2 { get; set; }
    public int? CXPE3 { get; set; }
    public int? CXPE4 { get; set; }
    public int? CXPE5 { get; set; }
    public int? CXPE6 { get; set; }

    public int? DIF1 { get; set; }
    public int? DIF2 { get; set; }
    public int? DIF3 { get; set; }
    public int? DIF4 { get; set; }
    public int? DIF5 { get; set; }
    public int? DIF6 { get; set; }

    public int? DIA1 { get; set; }
    public int? DIA2 { get; set; }
    public int? DIA3 { get; set; }
    public int? DIA4 { get; set; }
    public int? DIA5 { get; set; }
    public int? DIA6 { get; set; }

    public int? OTR1 { get; set; }
    public int? OTR2 { get; set; }
    public int? OTR3 { get; set; }
    public int? OTR4 { get; set; }
    public int? OTR5 { get; set; }
    public int? OTR6 { get; set; }

    public int? IVAD1 { get; set; }
    public int? IVAD2 { get; set; }
    public int? IVAD3 { get; set; }
    public int? IVAD4 { get; set; }
    public int? IVAD5 { get; set; }
    public int? IVAD6 { get; set; }

    public int? AGE1 { get; set; }
    public int? AGE2 { get; set; }
    public int? AGE3 { get; set; }
    public int? AGE4 { get; set; }
    public int? AGE5 { get; set; }
    public int? AGE6 { get; set; }

    public int? CXC_TC1 { get; set; }
    public int? CXC_TC2 { get; set; }
    public int? CXC_TC3 { get; set; }
    public int? CXC_TC4 { get; set; }
    public int? CXC_TC5 { get; set; }
    public int? CXC_TC6 { get; set; }

    public int? DSV1 { get; set; }
    public int? DSV2 { get; set; }
    public int? DSV3 { get; set; }
    public int? DSV4 { get; set; }
    public int? DSV5 { get; set; }
    public int? DSV6 { get; set; }
    
    public int? RIVA1 { get; set; }
    public int? RIVA2 { get; set; }
    public int? RIVA3 { get; set; }
    public int? RIVA4 { get; set; }
    public int? RIVA5 { get; set; }
    public int? RIVA6 { get; set; }
}