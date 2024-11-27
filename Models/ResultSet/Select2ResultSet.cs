namespace CoreContable.Models.ResultSet {
    public class Select2ResultSet {
        public string id { get; set; }
        public string text { get; set; }
        public bool more { get; set; } // Asegúrate de que esta propiedad esté presente

        // Propiedad calculada para formatear el texto con ceros
        public string FormattedText {
            get {
                return AddLeadingZeros(this.text); // Aplicar ceros
            }
        }

        // Función para agregar ceros a la izquierda
        private string AddLeadingZeros(string accountCode) {
            const int totalLength = 10;  // Longitud deseada
            return accountCode.PadLeft(totalLength, '0'); // Añade ceros a la izquierda
        }
    }
}
