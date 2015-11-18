using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.WebPages.Html;
using System.Web.Helpers;
//===========================================
using Negocio.ClasesCentral;

namespace CentralAgentesMvc.Models
{
    [System.Serializable]
    public class ResumenCotizar : ClsPolizas, System.ICloneable
    {
        public decimal SubTotalPrimas { get; set; }
        public decimal SubTotalServicios { get; set; }
        public decimal DerechoPoliza { get; set; }
        public decimal ExtensionRC { get; set; }
        public decimal AccidentesPersonales { get; set; }
        public decimal Recargos { get; set; }
        public decimal SubTotal { get; set; }
        public decimal TotalCotizacion { get; set; }

        public string Descripcion2Pago { get; set; }
        public List<string> WarningCalculoInfo { get; set; }

        public List<ClsOpcionesPago> OpcionesPagoInfo { get; set; }
        public List<ClsVehiculo> VehiculosInfo { get; set; }
        public List<ClsCalculo> calculos { get; set; }
        public bool QuiereCederComision { get; set; }
        public double porcentajeCede { get; set; }
        public GenericoViewModel cotizacionCom { get; set; }
        public Dictionary<int, string> PorcentajeComisionSource { get; set; }
        public string Origen { get; set; }
    }
}