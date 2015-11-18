using System.Collections;
//================================
using Negocio.ClasesCentral;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CentralAgentesMvc.Models
{
    [System.Serializable]
    public class ConductorModel : ClsConductor
    {
        [Required(ErrorMessage = "Debe indicar la edad o la fecha de nacimiento")]
        [Range(14, 85, ErrorMessage = "La edad debe estar entre los 14 y los 85 años")]
        public int Edad { get; set; }

        #region <-- Propiedades para Carga de DropDown -->
        public bool EsHombre { get; set; }
        public bool TieneExtensionRC
        {
            get
            {
                return (extRespCivil == 1 ? true : false);
            }
            set
            {
                extRespCivil = (value ? 1 : 0);
                DescripcionExRespCivil = (value ? "Sí" : "No");
            }
        }
        public Dictionary<string, string> EstadoCivilSource { get; set; }
        #endregion
    }
}