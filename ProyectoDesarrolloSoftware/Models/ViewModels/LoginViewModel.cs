using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace ProyectoDesarrolloSoftware.Models.ViewModels
{
    // ViewModel para el login de usuario
    public class LoginViewModel
    {
        [ValidateNever]
        // System.ComponentModel.DataAnnotations.Required es para validar que el campo no esté vacío y EmailAddress es para validar que el correo tenga un formato válido
        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "El correo es obligatorio")]
        [System.ComponentModel.DataAnnotations.EmailAddress]
        public string Email { get; set; } = string.Empty;

        [ValidateNever]
        // Aquí también se valida que la contraseña no esté vacía y se especifica que es un campo de tipo contraseña para que el navegador lo oculte al escribirlo
        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "La contraseña es obligatoria")]
        [System.ComponentModel.DataAnnotations.DataType(System.ComponentModel.DataAnnotations.DataType.Password)]
        public string Contrasennia { get; set; } = string.Empty;

        public bool Recordarme { get; set; } = false;
    }
}
 