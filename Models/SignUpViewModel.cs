using System.ComponentModel.DataAnnotations;

namespace CoreContable.Models
{
    public class SignUpViewModel
    {
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public required string FirstName { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        public required string LastName { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        public required string UserName { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [DataType(DataType.Password)]
        public required string Password { get; set; }

    }
}
