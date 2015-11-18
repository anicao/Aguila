using System.Collections.Generic;
using Negocio.ClasesCentral;
using Negocio.AccesosYPermisos;
//==============================
using System;


namespace CentralAgentesMvc.Models
{
    [Serializable]
    public class DesglosePagoModel : ClsTarjetaCredito
    {
        public string TipoPagoDesc { get; set; }
        public string VigenciaMonth { get; set; }
        public string VigenciaYear { get; set; }

        public Dictionary<string, string> Months { get; set; }
        public Dictionary<string, string> Years { get; set; }

        public IEnumerable<ArrCatalogoString> BancosSource { get; set; }
        public IEnumerable<TypCatalogString> TipoPagoSource { get; set; }
    }
}
