using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace CentralAgentesMvc.Models
{
    public class SiniestroModel
    {
        public string polizaId { get; set; }
        [Display(Name = "Número de siniestro")]
        public string siniestroId { get; set; }
        [Display(Name = "Conductor")]
        public string chofer { get; set; }
        [Display(Name = "Fecha")]
        public string fecha { get; set; }
        [Display(Name = "Automóvil")]
        public string auto { get; set; }
        [Display(Name = "Tipo de siniestro")]
        public string tipoSiniestro { get; set; }
        [Display(Name = "Detalle")]
        public string detalle { get; set; }
        [Display(Name = "Número de serie")]
        public string NoSerie { get; set; }
        [Display(Name = "Placas del auto")]
        public string Placas { get; set; }

        [Display(Name = "Taller de reparación")]
        public String taller { get; set; }
        [Display(Name = "Dirección del taller")]
        public string direccion { get; set; }
        [Display(Name = "Telefonos")]
        public string telefonos { get; set; }
        [Display(Name = "Fecha de recepción")]
        public string fechaRecepcion { get; set; }
        [Display(Name = "Fecha de entrega")]
        public string promesaEntrega { get; set; }
    }
}