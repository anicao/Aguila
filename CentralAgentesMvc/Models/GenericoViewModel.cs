using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
//==================================
using Negocio.ClasesCentral;
using Negocio.AccesosYPermisos;
using System.ComponentModel.DataAnnotations;


namespace CentralAgentesMvc.Models
{
    /// <summary>
    /// Objeto generico para hacer conversiones entre Póliza / Cotizacion / Renovación
    /// </summary>
    [Serializable]
    public partial class GenericoViewModel : ClsPolizas
    {
        #region <-- Propiedades Propias de Cotizaciones -->
        public DataExtraPoliza dataExtraPoliza { get; set; }
        public int cotizacionID { get; set; }
        public DateTime fechaCotizacion { get; set; }
        public string estatus { get; set; }
        public string tipo { get; set; }
        public string CIE { get; set; }
        [CustomValidation(typeof(ClsPolizas), "ValidacionRFC")]
        public string RFC { get; set; }

        public List<ClsCalculo> calculos { get; set; }

        public string cVendida { get; set; }
        public string VendidaPor { get; set; }

        public int promocion { get; set; }
        public string nomPromocion { get; set; }
        
        public string nSolicitud { get; set; }
        public string nSolicitud_Ant { get; set; }
        
        public bool guardarcomo { get; set; }
        
        private long mlngNumError { get; set; }
        private string mstrDesError { get; set; }
        private string mstrSolicitud { get; set; }
        private string mstrSolicitud_Ant { get; set; }
        public string lblRelevante { get; set; }
        public double nReferenciaWeb { get; set; }
        public System.Data.DataTable tblCotizacionesCober { get; set; }
        #endregion

        #region <-- Propiedades Compartidas Cotizacion / Renovación -->
        public bool bndPromocion { get; set; }
        public int maxProcarVehi { get; set; }
        #endregion

        #region <-- Propiedades Propias de Renovaciones -->
        public byte bandRenovacion { get; set; }
        public double montoInicial { get; set; }
        public double montoSiguiente { get; set; }
        public double dblTotalGeneral { get; set; }
        #endregion

        #region <-- Propiedades para Carga de DropDown -->
        public IEnumerable<TypCatalogString> OficinasSource { get; set; }
        public IEnumerable<TypCampaña> CampañasSource { get; set; }
        public IEnumerable<TypAgentes> AgentesSource { get; set; }
        public IEnumerable<TypPromo> PromocionesSource { get; set; }
        public IEnumerable<TypCatalogString> FormaPagoSource { get; set; }
        public IEnumerable<TypCatalogString> TipoPagoSource { get; set; }
        public IEnumerable<TypMeses> PeriodosMesSource { get; set; }

        public Dictionary<int, string> RiesgoConductorSource { get; set; }
        public Dictionary<int, string> PorcentajeComisionSource { get; set; }
        public double porcentajeCede { get; set; }
        public IEnumerable CoberturaSource { get; set; }
        #endregion

        #region <-- Propiedades de Extensión -->
        public string ModulodeTrabajo { get; set; }
        public bool IsNew { get; set; }
        public bool TieneMasAutos
        {
            get
            {
                return (masAutos == "S" ? true : false);
            }
            set
            {
                masAutos = (value ? "S" : "N");
            }
        }
        public bool CotizacionFinanciada
        {
            get
            {
                return (cFinanciamiento == "S" ? true : false);
            }
            set
            {
                cFinanciamiento = (value ? "S" : "N");
            }
        }
        public bool AplicaCobertura100
        {
            get
            {
                return (cobertura100 == 1 ? true : false);
            }
            set
            {
                cobertura100 = (value ? 1 : 0);
                DescripcionCobertura100 = (value ? "Sí" : "No");
            }
        }
        public bool AplicaERA
        {
            get
            {
                return (cEra == "S" ? true : false);
            }
            set
            {
                cEra = (value ? "S" : "N");
            }
        }
        public bool AplicaTERSSA
        {
            get
            {
                return (tersa == 1 ? true : false);
            }
            set
            {
                tersa = (value ? 1 : 0);
                DescripcionTersa = (value ? "Sí" : "No");
            }
        }
        public bool LlevaKit { get; set; }
        public bool EsHombre { get; set; }
        public bool EsPersonaFisica
        {
            get
            {
                return (persona == "1" ? false : true);
            }
            set
            {
                persona = (value ? "0" : "1");
            }
        }
        public bool QuiereCederComision
        {
            get
            {
                return (chkCedeComision == "S" ? true : false);
            }
            set
            {
                chkCedeComision = (value ? "S" : "N");
            }
        }


        [Required(ErrorMessage = "Debe indicar el Nro. de Teléfono")]
        public string PrimerTelefono
        {
            get
            {
                if (this.telefono != null)
                {
                    if (this.telefono.Count > 0)
                    {
                        _primertlf = this.telefono[0].cTel;
                    }
                }
                return _primertlf;
            }
            set
            {
                _primertlf = value;
                if (this.telefono != null)
                {
                    if (this.telefono.Count > 0)
                    {
                        this.telefono[0].cTel = value;
                    }
                }
            } 
        }
        private string _primertlf;


        //public CedeComision comisionOrig{get;set;}
       
        #endregion

        #region <-- Propiedades para registro individual -->
        public ResumenCotizar CalculoCotizacion { get; set; }
        public List<DesglosePagoModel> DocumentosPago { get; set; }
        #endregion
    }
}