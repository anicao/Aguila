using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Negocio.AccesosYPermisos;
using System.Web.Mvc;

namespace CentralAgentesMvc.Models
{
    public class ServiciosModel
    {
        [DataType(DataType.Text)]
        [Display(Name = "Póliza:")]
        public string PolizaID { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Asegurado:")]
        public string NombreAsegurado { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Solicitud:")]
        public string SolicitudID { get; set; }
    }
}