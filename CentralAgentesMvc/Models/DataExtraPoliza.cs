using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CentralAgentesMvc.Models
{
    public class DataExtraPoliza
    {
        public long cotizacioId { get; set; }
        public int solicitudId { get; set; }
        public long polizaId { get; set; }
        public String statusFinal { get; set; }
        public decimal montoPagado { get; set; }
        public String segEstatus { get; set; }
    }
}