using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace CentralAgentesMvc.Models
{
    public class PagosModel
    {
        [Display(Name = "Endoso")]
        public int endoso { get; set; }
         [Display(Name = "Recibo")]
        public int recibo { get; set; }
         [Display(Name = "Inicio de vigencia")]
        public String vigenciaInicio { get; set; }
        [Display(Name = "Fin de vigencia ")]
        public String vigenciafin { get; set; }
        [Display(Name = "Total")]
        public string total { get; set; }
    }
}
