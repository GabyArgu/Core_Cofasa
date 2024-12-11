using System.ComponentModel.DataAnnotations.Schema;

namespace CoreContable.Models.Report {
    public class ReporteEstadoResultadosDetalle {
        public string clase_saldo { get; set; }      // Coincide con clase_saldo en la función
        public string Nombre_Cia { get; set; }       // Coincide con Nombre_Cia en la función
        public string Cta_CONTABLE { get; set; }     // Coincide con Cta_CONTABLE en la función
        public string Descrip_Esp { get; set; }      // Coincide con Descrip_Esp en la función
        public string grupo_cta { get; set; }        // Coincide con grupo_cta en la función
        public string Grupo_Cta { get; internal set; }
        public string sub_grupo { get; set; }        // Coincide con sub_grupo en la función
        public decimal Saldo { get; set; }     
        // Coincide con Saldo en la función
        [NotMapped]
        public decimal PorcentajeMes { get; set; }

        [NotMapped]
        public decimal PorcentajeAcumulado { get; set; }



        public decimal saldo_acumulado { get; set; }

        // Coincide con Saldo en la función
        [NotMapped]
        public decimal PorcentajeMes2 { get; set; }

        [NotMapped]
        public decimal PorcentajeAcumulado2 { get; set; }
    }
}
