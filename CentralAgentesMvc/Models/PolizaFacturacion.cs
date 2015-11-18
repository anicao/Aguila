using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace CentralAgentesMvc.Models
{
    public class PolizaFacturacion
    {
        [Display(Name = "Poliza")]
        public long? nPoliza { get; set; }
        [Display(Name = "Expedición")]
        public String dfExpedicion { get; set; }
        [Display(Name = "Nombre")]
        public String cNombreA { get; set; }
        [Display(Name = "Endoso")]
        public int? nEndoso { get; set; }
        [Display(Name = "Recibo")]
        public int? nRecibo { get; set; }
        [Display(Name = "Fecha emisión")]
        public String Fecha_Emision { get; set; }
        [Display(Name = "Serie")]
        public String Serie { get; set; }
        [Display(Name = "Folio")]
        public String Folio { get; set; }
        public String LnkPDF { get; set; }
        public String LnkXML { get; set; }
        public String Lnk100 { get; set; }
        public String Css100 { get; set; }
        public String LnkSOB { get; set; }
        [Display(Name = "Inicio vigencia")]
        public String dFIniVig { get; set; }
        [Display(Name = "Fin vigencia")]
        public String dFFinVig { get; set; }
    }
}
