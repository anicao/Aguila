using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Negocio.AccesosYPermisos;

namespace CentralAgentesMvc.Models
{
    public class ReportViewModel
    {
        public string TitleReport { get; set; }
        public string ReportID    { get; set; }
        public string ReportName  { get; set; }
        public string ClienteID   { get; set; }
        public string AgenteName  { get; set; }


        [DataType(DataType.Text)]
        [Display(Name = "Agente:")]
        public string AgenteID { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Fecha Inicio:")]
        [DisplayFormat(DataFormatString = "{0:dd/MMM/yyyy}", ApplyFormatInEditMode = true)]
        public string FechaDesde { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Fecha Final:")]
        [DisplayFormat(DataFormatString = "{0:dd/MMM/yyyy}", ApplyFormatInEditMode = true)]
        public string FechaHasta { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Póliza:")]
        public string PolizaID { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Asegurado:")]
        public string NombreAsegurado { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Periodo:")]
        public string PeriodoID { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Mes:")]
        public string MesPeriodoID { get; set; }

        public IEnumerable<TypAgentes> AgentesSource { get; set; }
        public List<string> PeriodosSource { get; set; }
        public IEnumerable<TypMeses> PeriodosMesSource { get; set; }
    }
}