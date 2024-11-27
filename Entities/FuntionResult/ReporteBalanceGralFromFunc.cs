using Microsoft.EntityFrameworkCore;

namespace CoreContable.Entities.FunctionResult {
    [Keyless]
    public class ReporteBalanceGralFromFunc {
        public string COD_CIA { get; set; }
        public string clase_saldo { get; set; }
        public string Nombre_Cia { get; set; }
        public string Cta_CONTABLE { get; set; }
        public string DESCRIP_ESP { get; set; }
        public string grupo_cta { get; set; }
        public string sub_grupo { get; set; }
        public double? saldo { get; set; } // Cambiado a double
    }
}
