using System.ComponentModel.DataAnnotations;
using CoreContable.Models.ResultSet;

namespace CoreContable.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string? UserName { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Required(ErrorMessage = "El campo compañía es requerido")]
        public string? CiaCode { get; set; }

        public List<CiaResultSet>? Cias { get; set; }
    }
}
