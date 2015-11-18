using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
//==================================
using Negocio.ClasesCentral;
using Negocio.AccesosYPermisos;


namespace CentralAgentesMvc.Models
{
    [System.Serializable]
    public class CarExpressModel
    {
        public int VehiculoID { get; set; }

        [Required(ErrorMessage = "Debe indicar la placa del vehículo")]
        public string Placa { get; set; }

        [Required(ErrorMessage = "Debe indicar la serie")]
        [Range(1, 16, ErrorMessage = "La serie debe ser de 16 caracteres")]
        public string Serie { get; set; }
    }
}