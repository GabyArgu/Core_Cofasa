namespace CoreContable.Models.ResultSet {
    public class CentroCuentaFormatoResultSet {
        public string COD_CIA { get; set; }               // Código de la compañía
        public string CENTRO_COSTO { get; set; }          // Centro de costo
        public int? CTA_1 { get; set; }                   // Primer nivel de cuenta
        public int? CTA_2 { get; set; }                   // Segundo nivel de cuenta
        public int? CTA_3 { get; set; }                   // Tercer nivel de cuenta
        public int? CTA_4 { get; set; }                   // Cuarto nivel de cuenta
        public int? CTA_5 { get; set; }                   // Quinto nivel de cuenta
        public int? CTA_6 { get; set; }                   // Sexto nivel de cuenta
        public string ESTADO { get; set; }                // Estado de la cuenta
        public string USUARIO_CREACION { get; set; }      // Usuario que creó el registro
        public DateTime? FECHA_CREACION { get; set; }     // Fecha de creación del registro
        public string USUARIO_MODIFICACION { get; set; }  // Usuario que modificó el registro
        public DateTime? FECHA_MODIFICACION { get; set; } // Fecha de la última modificación del registro
    }
}
