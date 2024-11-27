using CoreContable.Entities;

namespace CoreContable.Models.ResultSet;

public class CiaResultSet {
    public required string Cod { get; set; }

    public required string RazonSocial { get; set; }

    public required string NomComercial { get; set; }

    public required string DirecEmpresa { get; set; }

    public required string TelefEmpresa { get; set; }

    public required string NitEmpresa { get; set; }

    public required string NumeroPatronal { get; set; }

    public int? MesCierre { get; set; }

    public int? MesProceso { get; set; }

    public int? Periodo { get; set; }

    public int? Cta1ResulAct { get; set; }

    public int? Cta2ResulAct { get; set; }

    public int? Cta3ResulAct { get; set; }

    public int? Cta4ResulAct { get; set; }

    public int? Cta5ResulAct { get; set; }

    public int? Cta6ResulAct { get; set; }

    public int? Cta1ResulAnt { get; set; }

    public int? Cta2ResulAnt { get; set; }

    public int? Cta3ResulAnt { get; set; }

    public int? Cta4ResulAnt { get; set; }

    public int? Cta5ResulAnt { get; set; }

    public int? Cta6ResulAnt { get; set; }

    public int? Cta1PerGan { get; set; }

    public int? Cta2PerGan { get; set; }

    public int? Cta3PerGan { get; set; }

    public int? Cta4PerGan { get; set; }

    public int? Cta5PerGan { get; set; }

    public int? Cta6PerGan { get; set; }

    public DateTime? FechUlt { get; set; }

    public DateTime? FecUltCie { get; set; }

    // public float? TasaIva { get; set; }
    public double? TasaIva { get; set; }

    public int? MesesChq { get; set; }

    // public float? TasaCam { get; set; }
    public double? TasaCam { get; set; }

    // public float? IvaPorc { get; set; }
    public double? IvaPorc { get; set; }

    public string? NdIva { get; set; }

    public DateTime? FdIva { get; set; }

    // public float? IsrPorc { get; set; }
    public double? IsrPorc { get; set; }

    public string? NdIsr { get; set; }

    public DateTime? FdIsr { get; set; }

    // public float? PrbPorc { get; set; }
    public double? PrbPorc { get; set; }

    public string? NdPrb { get; set; }

    public DateTime? FdPrb { get; set; }

    // public float? PrsPorc { get; set; }
    public double? PrsPorc { get; set; }

    public string? NdPrs { get; set; }

    public DateTime? FdPrs { get; set; }

    public string? CodMoneda { get; set; }
    public Select2ResultSet? MonedaSelect2 { get; set; }

    public string? DupDetPartidad { get; set; }

    // public float? ValMinDepreciar { get; set; }
    public double? ValMinDepreciar { get; set; }

    public int? IngresoCta1 { get; set; }

    public int? IngresoCta2 { get; set; }

    public int? IngresoCta3 { get; set; }

    public int? IngresoCta4 { get; set; }

    public int? IngresoCta5 { get; set; }

    public int? IngresoCta6 { get; set; }

    public int? GastoCta1 { get; set; }

    public int? GastoCta2 { get; set; }

    public int? GastoCta3 { get; set; }

    public int? GastoCta4 { get; set; }

    public int? GastoCta5 { get; set; }

    public int? GastoCta6 { get; set; }

    public int? CostoCta1 { get; set; }

    public int? CostoCta2 { get; set; }

    public int? CostoCta3 { get; set; }

    public int? CostoCta4 { get; set; }

    public int? CostoCta5 { get; set; }

    public int? CostoCta6 { get; set; }

    public static CiaResultSet EntityToResultSet(Cias cia) {
        return new CiaResultSet {
            Cod = cia.CodCia,
            RazonSocial = cia.RazonSocial ?? "",
            NomComercial = cia.NomComercial ?? "",
            DirecEmpresa = cia.DirecEmpresa ?? "",
            TelefEmpresa = cia.TelefEmpresa ?? "",
            NitEmpresa = cia.NitEmpresa ?? "",
            NumeroPatronal = cia.NumeroPatronal ?? "",
            MesCierre = cia.MesCierre,
            MesProceso = cia.MesProceso,
            Periodo = cia.Periodo,
            Cta1ResulAct = cia.Cta1ResulAct,
            Cta2ResulAct = cia.Cta2ResulAct,
            Cta3ResulAct = cia.Cta3ResulAct,
            Cta4ResulAct = cia.Cta4ResulAct,
            Cta5ResulAct = cia.Cta5ResulAct,
            Cta6ResulAct = cia.Cta6ResulAct,
            Cta1ResulAnt = cia.Cta1ResulAnt,
            Cta2ResulAnt = cia.Cta2ResulAnt,
            Cta3ResulAnt = cia.Cta3ResulAnt,
            Cta4ResulAnt = cia.Cta4ResulAnt,
            Cta5ResulAnt = cia.Cta5ResulAnt,
            Cta6ResulAnt = cia.Cta6ResulAnt,
            Cta1PerGan = cia.Cta1PerGan,
            Cta2PerGan = cia.Cta2PerGan,
            Cta3PerGan = cia.Cta3PerGan,
            Cta4PerGan = cia.Cta4PerGan,
            Cta5PerGan = cia.Cta5PerGan,
            Cta6PerGan = cia.Cta6PerGan,
            FechUlt = cia.FechUlt,
            FecUltCie = cia.FecUltCie,
            TasaIva = cia.TasaIva,
            MesesChq = cia.MesesChq,
            TasaCam = cia.TasaCam,
            IvaPorc = cia.IvaPorc,
            NdIva = cia.NdIva,
            FdIva = cia.FdIva,
            IsrPorc = cia.IsrPorc,
            NdIsr = cia.NdIsr,
            FdIsr = cia.FdIsr,
            PrbPorc = cia.PrbPorc,
            NdPrb = cia.NdPrb,
            FdPrb = cia.FdPrb,
            PrsPorc = cia.PrsPorc,
            NdPrs = cia.NdPrs,
            FdPrs = cia.FdPrs,
            CodMoneda = cia.CodMoneda,
            DupDetPartidad = cia.DupDetPartidad,
            ValMinDepreciar = cia.ValMinDepreciar,
            IngresoCta1 = cia.IngresoCta1,
            IngresoCta2 = cia.IngresoCta2,
            IngresoCta3 = cia.IngresoCta3,
            IngresoCta4 = cia.IngresoCta4,
            IngresoCta5 = cia.IngresoCta5,
            IngresoCta6 = cia.IngresoCta6,
            GastoCta1 = cia.GastoCta1,
            GastoCta2 = cia.GastoCta2,
            GastoCta3 = cia.GastoCta3,
            GastoCta4 = cia.GastoCta4,
            GastoCta5 = cia.GastoCta5,
            GastoCta6 = cia.GastoCta6,
            CostoCta1 = cia.CostoCta1,
            CostoCta2 = cia.CostoCta2,
            CostoCta3 = cia.CostoCta3,
            CostoCta4 = cia.CostoCta4,
            CostoCta5 = cia.CostoCta5,
            CostoCta6 = cia.CostoCta6
        };
    }

    public static CiaResultSet PartialEntityToResultSet(Cias cia) {
        return new CiaResultSet {
            Cod = cia.CodCia,
            RazonSocial = cia.RazonSocial ?? "",
            NomComercial = cia.NomComercial ?? "",
            DirecEmpresa = cia.DirecEmpresa ?? "",
            TelefEmpresa = cia.TelefEmpresa ?? "",
            NitEmpresa = cia.NitEmpresa ?? "",
            NumeroPatronal = cia.NumeroPatronal ?? "",
        };
    }
}