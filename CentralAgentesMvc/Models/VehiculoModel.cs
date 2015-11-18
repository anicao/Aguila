using System.Collections;
using System.Collections.Generic;
//==================================
using Negocio.ClasesCentral;
using Negocio.AccesosYPermisos;


namespace CentralAgentesMvc.Models
{
    [System.Serializable]
    public class VehiculoModel : ClsVehiculo
    {
        public string MarcaID { get; set; }
        public string SubMarcaID { get; set; }
        public string TipoID { get; set; }
        
        #region <-- Propiedades para Carga de DropDown -->
        public IEnumerable<TypCatalogString> SubRamoSource { get; set; }
        public Dictionary<string, string> YearsSource { get; set; }
        public IEnumerable MarcasSource { get; set; }
        public IEnumerable SubMarcasSource { get; set; }
        public IEnumerable TipoVehiculoSource { get; set; }

        public IEnumerable<TypDeduc> PorcentajeDMSource { get; set; }
        public IEnumerable<TypDeduc> PorcentajeRTSource { get; set; }

        public IEnumerable<TypDeduc> CoberturaGMSource { get; set; }
        public IEnumerable<TypDeduc> CoberturaRCSource { get; set; }
        public IEnumerable<TypDeduc> CoberturaRCCSource { get; set; }

        public Dictionary<string, string> SumaAseguradaSource { get; set; }
        public Dictionary<string, string> AsistenciaViajeSource { get; set; }
        public Dictionary<string, string> VehSustitutoSource { get; set; }

        public IEnumerable<TypCatalogString> TipoTransmisionSource { get; set; }
        public IEnumerable<TypCatalogString> TipoAlarmaSource { get; set; }

        public IEnumerable GuiaEBCSource { get; set; }
        #endregion
    }
}