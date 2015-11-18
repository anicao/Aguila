using System.Collections.Generic;
//================================
using Negocio.ClasesCentral;
using Negocio.AccesosYPermisos;
using System.ComponentModel.DataAnnotations;

namespace CentralAgentesMvc.Models
{
    [System.Serializable]
    public class DireccionModel : ClsDir
    {
        #region <-- Propiedades para Carga de DropDown -->
        public IEnumerable<TypCatalogString> TipoDirSource { get; set; }
        public IEnumerable<TypCatalogString> EstadosSource { get; set; }

        [Required(ErrorMessage = "Debe indicar el RFC")]
        [CustomValidation(typeof(ClsPolizas), "ValidacionRFC")]
        public string RFC_ { get; set; }
        #endregion
    }
}