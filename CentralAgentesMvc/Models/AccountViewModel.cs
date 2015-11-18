using System.ComponentModel.DataAnnotations;
//===========================================
using System.Web.Mvc;
using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;


namespace CentralAgentesMvc.Models
{
    public class LoginViewModel
    {
        [Display(Name = "Usuario:")]
        [Required(ErrorMessage = "El campo es requerido")]
        [RegularExpression("^[a-zA-Z0-9_ñÑ.]*$", ErrorMessage = "Nombre de Usuario invalido. Debe contener solo caracteres alfanumericos")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "El campo es requerido")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña:")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class UserProfileViewModel
    {
        public bool IsValidated { get; set; }

        [Required]
        public string CaptchaText { get; set; }

        public string AgenteID { get; set; }
        public string AgenteName { get; set; }
        public string Credencial { get; set; }
        public string ActualPassword { get; set; }

        [Required(ErrorMessage = "El email es requerido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Debe indicar la contraseña actual")]
        [Compare("ActualPassword", ErrorMessage = "La contraseña actual no es valida.")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "Debe indicar la nueva contraseña")]
        [StringLength(100, ErrorMessage = "La contraseña debe tener mínimo 8 caracteres", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string PasswordHash { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Debe indicar la confirmación de contraseña")]
        [Display(Name = "Confirme Contraseña")]
        [Compare("PasswordHash", ErrorMessage = "La contraseña y la confirmación no concuerdan.")]
        [CustomValidation(typeof(UserProfileViewModel), "ValidaPassword")]
        public string ConfirmPassword { get; set; }

        /// <summary>
        /// Validador de reglas de contraseña
        /// </summary>
        /// <param name="confirm"></param>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        public static ValidationResult ValidaPassword(string password, ValidationContext validationContext)
        {
            int numCombinacion = 0;
            if (!Regex.IsMatch(password.Trim(), @"[A-Z]"))      { numCombinacion += 1; }
            if (!Regex.IsMatch(password.Trim(), @"[a-z]"))      { numCombinacion += 1; }
            if (!Regex.IsMatch(password.Trim(), @"[0-9]"))      { numCombinacion += 1; }
            if (!Regex.IsMatch(password.Trim(), @"[\-@+&%$#]")) { numCombinacion += 1; }

            if (numCombinacion > 2)
            {
                var msje = "La contraseña debe de formarse con la combinación de 2 o mas de las siguientes opciones: \n-Mayusculas \n-Minusculas \n-Numeros \n-Simbolos especiales (-@+&%$#)";
                return new ValidationResult(msje, new List<string> { "Password" });
            }

            return ValidationResult.Success;
        }
    }

    public class DatoUsuario
    {
     public static string idAgente {get; set;}
     public static string nomAgente {get; set;}
     public static string sHostName {get; set;}
     public static string sIp {get; set;}
     public static int campaña { get; set; }
     public static System.Data.DataSet DSLogeado { get; set; }

    }
}