namespace CoreContable.Models.Dto {
    public class CentroCuentaFormatoDto {

        internal object? isUpdating;

        public string COD_CIA { get; set; }
        public string CENTRO_COSTO { get; set; }
        public int CTA_1 { get; set; }
        public int CTA_2 { get; set; }
        public int CTA_3 { get; set; }
        public int CTA_4 { get; set; }
        public int CTA_5 { get; set; }
        public int CTA_6 { get; set; }
        public string ESTADO { get; set; }
        public DateTime? FECHA_CREACION { get; set; }
        public DateTime? FECHA_MODIFICACION { get; set; }
        public string? USUARIO_CREACION { get; set; }
        public string? USUARIO_MODIFICACION { get; set; }
    }

}
