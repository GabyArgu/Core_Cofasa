using CoreContable.Entities.FunctionResult;

namespace CoreContable.Models.Report {
    public class ReporteBalanceGral {
        public string FechaReporte { get; set; }
        public string Compania { get; set; }
        public List<ReporteBalanceGralFromFunc> Detalles { get; set; }


    }
}
