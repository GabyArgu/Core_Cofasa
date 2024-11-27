using CoreContable.Entities.FunctionResult;

namespace CoreContable.Models.Report {
    public class ReporteDiarioMayor {
        public string FechaReporte { get; set; }
        public string Compania { get; set; }
        public List<ReporteDiarioMayorFromFunc> Detalles { get; set; }
        public object NombreReporte { get; internal set; }
        public int NUM_POLIZA { get; internal set; }
        public string? ConceptoEncabezado { get; internal set; }
        public DateTime? Fecha_Poliza { get; internal set; }
        public string DescripcionTipoDocumento { get; internal set; }
        public string NombreCompania { get; internal set; }
        public string NumeroDeCuenta { get; internal set; }
        public string CentroCosto { get; internal set; }
        public double Cargo { get; internal set; }
        public double Abono { get; internal set; }
        public string Concepto { get; internal set; }
        public string NombreCuenta { get; internal set; }
        public string TipoDocto { get; internal set; }

        public string FechaInicio { get; set; }
        public string FechaFin { get; set; }
    }
}
