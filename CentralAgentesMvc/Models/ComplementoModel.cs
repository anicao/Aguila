using System.Collections.Generic;
//================================
using Negocio.AccesosYPermisos;
using Negocio.ClasesCentral;


namespace CentralAgentesMvc.Models
{
    [System.Serializable]
    public class ComplementoModel : ClsComplemento
    {
        public string PersonaQueRecomienda { get; set; }
        public string Responsable { get; set; }

        #region <-- Propiedades para Carga de DropDown -->
        public IEnumerable<TypCatalogString> CompañiaAnteriorSource { get; set; }
        public IEnumerable<TypCatalogString> InteresesSource { get; set; }
        public IEnumerable<TypCatalogString> PublicidadSource { get; set; }
        public IEnumerable<TypCatalogString> ResponsableSource { get; set; }
        public IEnumerable<TypCatalogString> EstatusSource { get; set; }
        #endregion
    }
}