using CoreContable.Models.ResultSet;

namespace CoreContable.Models.Report {
    public class ReporteEstadoResultados {
        public string NombreCia { get; set; }
        public string FechaInicio { get; set; }
        public string FechaFin { get; set; }
        public string Dia { get; set; }      // Nuevo campo para el día
        public string Mes { get; set; }      // Nuevo campo para el mes
        public string Anio { get; set; }     // Nuevo campo para el año
        public string Subtitulo { get; set; } // Nuevo campo para el subtítulo
        public List<ReporteEstadoResultadosDetalle> Detalles { get; set; }


    }
}
