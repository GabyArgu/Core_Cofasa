using CoreContable.Entities;

namespace CoreContable.Models.ResultSet;

public class ContaParamsResultSet
{
    public ContaParamsResultSet(DmgParam entity)
    {
        COD_CIA = entity.CodCia;
        DT1 = entity.DT1;
        DT2 = entity.DT2;
        DT3 = entity.DT3;
        DT4 = entity.DT4;
        DT5 = entity.DT5;
        DT6 = entity.DT6;
        CXC1 = entity.CXC1;
        CXC2 = entity.CXC2;
        CXC3 = entity.CXC3;
        CXC4 = entity.CXC4;
        CXC5 = entity.CXC5;
        CXC6 = entity.CXC6;
        CXP1 = entity.CXP1;
        CXP2 = entity.CXP2;
        CXP3 = entity.CXP3;
        CXP4 = entity.CXP4;
        CXP5 = entity.CXP5;
        CXP6 = entity.CXP6;
        EPF1 = entity.EPF1;
        EPF2 = entity.EPF2;
        EPF3 = entity.EPF3;
        EPF4 = entity.EPF4;
        EPF5 = entity.EPF5;
        EPF6 = entity.EPF6;
        IVA1 = entity.IVA1;
        IVA2 = entity.IVA2;
        IVA3 = entity.IVA3;
        IVA4 = entity.IVA4;
        IVA5 = entity.IVA5;
        IVA6 = entity.IVA6;
        CXPE1 = entity.CXPE1;
        CXPE2 = entity.CXPE2;
        CXPE3 = entity.CXPE3;
        CXPE4 = entity.CXPE4;
        CXPE5 = entity.CXPE5;
        CXPE6 = entity.CXPE6;
        DIF1 = entity.DIF1;
        DIF2 = entity.DIF2;
        DIF3 = entity.DIF3;
        DIF4 = entity.DIF4;
        DIF5 = entity.DIF5;
        DIF6 = entity.DIF6;
        DIA1 = entity.DIA1;
        DIA2 = entity.DIA2;
        DIA3 = entity.DIA3;
        DIA4 = entity.DIA4;
        DIA5 = entity.DIA5;
        DIA6 = entity.DIA6;
        OTR1 = entity.OTR1;
        OTR2 = entity.OTR2;
        OTR3 = entity.OTR3;
        OTR4 = entity.OTR4;
        OTR5 = entity.OTR5;
        OTR6 = entity.OTR6;
        IVAD1 = entity.IVAD1;
        IVAD2 = entity.IVAD2;
        IVAD3 = entity.IVAD3;
        IVAD4 = entity.IVAD4;
        IVAD5 = entity.IVAD5;
        IVAD6 = entity.IVAD6;
        AGE1 = entity.AGE1;
        AGE2 = entity.AGE2;
        AGE3 = entity.AGE3;
        AGE4 = entity.AGE4;
        AGE5 = entity.AGE5;
        AGE6 = entity.AGE6;
        CXC_TC1 = entity.CXC_TC1;
        CXC_TC2 = entity.CXC_TC2;
        CXC_TC3 = entity.CXC_TC3;
        CXC_TC4 = entity.CXC_TC4;
        CXC_TC5 = entity.CXC_TC5;
        CXC_TC6 = entity.CXC_TC6;
        DSV1 = entity.DSV1;
        DSV2 = entity.DSV2;
        DSV3 = entity.DSV3;
        DSV4 = entity.DSV4;
        DSV5 = entity.DSV5;
        DSV6 = entity.DSV6;

        RIVA1 = entity.RIVA1;
        RIVA2 = entity.RIVA2;
        RIVA3 = entity.RIVA3;
        RIVA4 = entity.RIVA4;
        RIVA5 = entity.RIVA5;
        RIVA6 = entity.RIVA6;
    }

    public string? COD_CIA { get; set; }

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