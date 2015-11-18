using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CentralAgentesMvc.Models
{
    public class CotizadorExpressModel
    {
        public String tipoPoliza { get; set; }
        public String descripcionPoliza { get; set; }
        public String roboTotal { get; set; }
        public String rcCatastrofica { get; set; }
        public String daniosMateriales { get; set; }
        public String responCivil { get; set; }
        public String gastosMedicos { get; set; }
        public String calle { get; set; }
        public String noExterior { get; set; }
        public String noInterior { get; set; }
        public String cp { get; set; }
        public String poblacion { get; set; }
        public String colonia { get; set; }
        public string estado { get; set; }
        public string placas{ get; set; }
        public string serie { get; set; }
        public string nombreTarjeta { get; set; }
        public string numeroTarjeta { get; set; }
        public String banco { get; set; }
        public string vigenciaTarjeta { get; set; }
        public string cds { get; set; }
        public string montoPago { get; set; }
        public String nombre { get; set; }
        public String email { get; set; }
        public String telefono { get; set; }
        public String autoMarca { get; set; }
        public String autoSubmarca { get; set; }
        public String autoTipo { get; set; }
        public String autoAnio { get; set; }
        public String planPago { get; set; }
        public String tersa { get; set; }
        public String cobertura100 { get; set; }
        public String autoPrestado { get; set; }
    }
}