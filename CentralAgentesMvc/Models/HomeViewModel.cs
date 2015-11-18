using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CentralAgentesMvc.Models
{
    public class HomeViewModel
    {
        public int ID { get; set; }
        public string DiaMes { get; set; }
        public int? CountCotiza { get; set; }
        public int? CountPoliza { get; set; }

        public int? CotizacionesMes { get; set; }
        public int? CotizacionesAño { get; set; }
        public decimal? PolPrimNetaMes { get; set; }
        public decimal? PolPrimNetaAño { get; set; }

        public int? RenovacionesMes { get; set; }
        public int? RenovacionesAño { get; set; }
        public decimal? RenPolPrimNetaMes { get; set; }
        public decimal? RenPolPrimNetaAño { get; set; }

        public int? EndososMes { get; set; }
        public int? EndososAño { get; set; }
        public decimal? MontoEndososMes { get; set; }
        public decimal? MontoEndososAño { get; set; }

        public decimal? PrimaTotalMes { get; set; }
        public decimal? PrimaTotalAño { get; set; }

        public string UltimoPagoCom { get; set; }
    }
}