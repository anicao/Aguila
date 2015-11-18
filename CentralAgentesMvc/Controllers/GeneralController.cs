using System;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using System.Collections.Generic;
//================================
using CentralAgentesMvc.Models;
//================================
using Negocio.ClasesCentral;
using Negocio.AccesosYPermisos;
using Negocio.HerramientasVarios;

namespace CentralAgentesMvc.Controllers
{
    [Authorize]
    public class GeneralController : Controller
    {
        #region <-- Private Properties -->
        private CmbCatalogos ctlg = new CmbCatalogos();
        private CotizacionController ctzC = new CotizacionController();
        #endregion

        #region <-- Action para mostrar Detalle General -->
        /// <summary>
        /// Método que muestra la ventana de detalle de los objetos Cotizacion / Renovacion
        /// </summary>
        /// <param name="model">Objeto convertido al ViewModel</param>
        /// <returns>Vista de DetalleGeneral</returns>
        public ActionResult DetalleGeneral(string id, string modulo)
        {
            try
            {
                // Busco info del documento
                GenericoViewModel d = (GenericoViewModel)Session["ModeloEnUso"];
                modulo = (modulo == null ? d.ModulodeTrabajo : modulo);
                if (id != "0")
                {
                    d = GetDataObject(id, modulo);
                }
                
                // Guardo el modelo
                ViewBag.Modulo = modulo;
                d.ModulodeTrabajo = modulo;
                d.codigoPostal = (d.vehiculos != null ? d.vehiculos.First().codigoPostal : "");

                // Inicializo coleccion si estan null
                if (d.vehiculos == null) d.vehiculos = new List<ClsVehiculo>();
                if (d.conductores == null) d.conductores = new List<ClsConductor>();
                if (d.prospectos == null) d.prospectos = new List<ClsProspecto>();
                if (d.complemento == null) d.complemento = new ClsComplemento();

                if (d.conductores != null)
                {
                    var cndR = d.conductores.FirstOrDefault(c => c.nNumCond == 0);
                    if (cndR != null) d.conductores.Remove(cndR);
                }
                if (!string.IsNullOrEmpty(d.SeguimientoPol.FProxLlamada)) {
                    d.SeguimientoPol.FProxLlamada = DateTime.Parse(d.SeguimientoPol.FProxLlamada).ToString("dd/MM/yyy");
                }
                if (!string.IsNullOrEmpty(d.SeguimientoPol.FUltLlamada)) {
                    d.SeguimientoPol.FUltLlamada = DateTime.Parse(d.SeguimientoPol.FUltLlamada).ToString("dd/MM/yyy");
                }
                if(!string.IsNullOrEmpty(d.SeguimientoPol.Hora)){
                    d.SeguimientoPol.Hora = DateTime.Parse(d.SeguimientoPol.Hora).ToString("H:mm tt");
                }
                SetPorcentajeComisionList(ref d);
                SetMediosDePago(ref d);
                GuardaEnMemoria(d);
                return PartialView(d);
            }
            catch (Exception err)
            {
                Response.StatusCode = (int)(System.Net.HttpStatusCode.InternalServerError);
                LogDeErrores.RegistroErrorWeb(err, DatoUsuario.idAgente, DatoUsuario.nomAgente, DatoUsuario.sIp, DatoUsuario.sHostName);
                return Json("ErrInterno");
            }
        }

        public void ObtenCedeComision(bool cede, double porcentaje)
        {
            try
            {
                GenericoViewModel d = (GenericoViewModel)Session["ModeloEnUso"];
                d.QuiereCederComision = cede;
                d.porcentajeCede = porcentaje;
                Session["ModeloEnUso"] = d;
            }
            catch (Exception err)
            {
                Response.StatusCode = (int)(System.Net.HttpStatusCode.InternalServerError);
                LogDeErrores.RegistroErrorWeb(err, DatoUsuario.idAgente, DatoUsuario.nomAgente, DatoUsuario.sIp, DatoUsuario.sHostName);
                throw err;
            }

        }

        public void ObtenRegresaComision()
        {
            try
            {
                GenericoViewModel d = (GenericoViewModel)Session["ModeloEnUso"];
                if (CedeComision.cargado)
                {
                    d.control = CedeComision.ProcarOriginal;
                    d.ObjCalculo.PrimaNetaInicial = CedeComision.ProcarOriginal;
                    d.ObjCalculo.mSubtotal = CedeComision.ProcarOriginal;
                    d.ObjCalculo.mTotal = CedeComision.ProcarOriginal;
                    d.QuiereCederComision = false;
                    d.chkCedeComision = "N";
                    CedeComision.cargado = false;
                }
                Session["ModeloEnUso"] = d;
            }
            catch (Exception err)
            {
                Response.StatusCode = (int)(System.Net.HttpStatusCode.InternalServerError);
                LogDeErrores.RegistroErrorWeb(err, DatoUsuario.idAgente, DatoUsuario.nomAgente, DatoUsuario.sIp, DatoUsuario.sHostName);
                throw err;
            }

        }

        public ActionResult RefreshDetalleGeneral()
        {
            try
            {
                GenericoViewModel d = (GenericoViewModel)Session["ModeloEnUso"];

                return PartialView("_DetalleGenPartial", d);
            }
            catch (Exception err)
            {
                Response.StatusCode = (int)(System.Net.HttpStatusCode.InternalServerError);
                LogDeErrores.RegistroErrorWeb(err, DatoUsuario.idAgente, DatoUsuario.nomAgente, DatoUsuario.sIp, DatoUsuario.sHostName);
                return Json("ErrInterno");
            }
        }

        private void SetPorcentajeComisionList(ref GenericoViewModel model)
        {
            try
            {
                model.PorcentajeComisionSource = new Dictionary<int, string>();
                for (int i = 0; i < 21; i++)
                {
                    model.PorcentajeComisionSource.Add(i, i.ToString());
                }
            }
            catch (Exception err)
            {
                Response.StatusCode = (int)(System.Net.HttpStatusCode.InternalServerError);
                LogDeErrores.RegistroErrorWeb(err, DatoUsuario.idAgente, DatoUsuario.nomAgente, DatoUsuario.sIp, DatoUsuario.sHostName);
                throw err;
            }
        }
        #endregion

        #region <-- Action para guardar datos -->
        [HttpPost]
        public ActionResult GuardarDocumento(string modulo)
        {
            //var logged = (DataSet)Session["UserObj"];
            var agentID = DatoUsuario.idAgente;// logged.Tables["catAgentes"].Rows[0]["nAgenteID"].ToString();

            // Realizo el calculo
            GenericoViewModel modelComplete = (GenericoViewModel)Session["ModeloEnUso"];
            try
            {
                if (modelComplete.calculos == null)
                {
                    // EJecuto el calculo
                    ctzC.ControllerContext = this.ControllerContext;
                    ctzC.Calculo(ref modelComplete, "C");
                }

                modelComplete.conductores.Remove(modelComplete.conductores.FirstOrDefault(x => x.nombre == "-- Seleccione un conductor asignado --"));
                // Envio los datos a guardar
                Dictionary<string, object> results = new Dictionary<string, object>();
                if (modelComplete.ModulodeTrabajo == "Cotizaciones")
                {
                    ClsCotizacion cotizacion = CentralAgentesMvc.App_Start.CastObject.Cast<ClsCotizacion>(modelComplete);
                    //cotizacion.usuario=
                   
                   
                    results = cotizacion.GuardaCotizacionWeb(cotizacion, (modelComplete.IsNew ? "A" : "M"), agentID);
                }
                else
                {
                    ClsRenovaciones renovacion = CentralAgentesMvc.App_Start.CastObject.Cast<ClsRenovaciones>(modelComplete);
                    var logged = (System.Data.DataSet)Session["UserObj"];
                    renovacion.usuario = logged.Tables["catAgentes"].Rows[0]["cNombre"].ToString();
                    results = renovacion.GuardaRenovacionWeb(renovacion, (modelComplete.IsNew ? "A" : "M"), agentID);
                }

                if (results.First().Value.ToString() != "OK")
                {
                    return Json(new { success = false, errores = "", validaciones = results });
                }

                modelComplete.IsNew = false;
                modelComplete.cotizacionID = (modelComplete.ModulodeTrabajo == "Cotizaciones" ? Convert.ToInt32(results["ID"]) : modelComplete.cotizacionID);
                modelComplete.poliza = (modelComplete.ModulodeTrabajo == "Cotizaciones" ? modelComplete.poliza : Convert.ToInt64(results["ID"]));

                // Guardo el modelo
                GuardaEnMemoria(modelComplete);

                return Content(results["ID"].ToString());
            }
            catch (Exception err)
            {
                Response.StatusCode = (int)(System.Net.HttpStatusCode.InternalServerError);
                LogDeErrores.RegistroErrorWeb(err, DatoUsuario.idAgente, DatoUsuario.nomAgente, DatoUsuario.sIp, DatoUsuario.sHostName);
                return Json("ErrInterno");
            }
        }
        #endregion

        #region <-- Datos Generales Methods -->
        /// <summary>
        /// GET Accion para crear un nuevo documento
        /// </summary>
        /// <param name="modulo">Modulo que se esta trabajando:
        ///                         Cotizaciones
        ///                         Reovaciones
        ///                         Polizas
        /// </param>
        /// <returns></returns>
        public ActionResult CreateDocumento(string modulo)
        {
            try
            {
                GenericoViewModel gm = new GenericoViewModel();
                gm.VendidaPor = "Internet";
                gm.cVendida = "I";
                ViewBag.IsNew = gm.IsNew = true;

                // Seteo las propiedades de lista
                SetCotizacionListProperties(ref gm);

                // Seteo los valores por defecto
                var logged = (DataSet)Session["UserObj"];
                gm.ModulodeTrabajo = modulo;
                gm.meses = 12;
                gm.cobertura = 12;
                gm.AplicaTERSSA = true;
                gm.AplicaCobertura100 = false;
                gm.AplicaERA = false;
                gm.conductorRestringido = 1;
                gm.EsHombre = true;
                gm.EsPersonaFisica = true;
                gm.QuiereCederComision = false;
                gm.strchkCedeComision = "N";
                gm.chkCedeComision = "N";
                gm.porcentajecomision = 0;
                gm.cEndosoCancela = "N";
                gm.control = 30;
                gm.fechaCotizacion = DateTime.Now;
                gm.oficinaID = Convert.ToInt32(logged.Tables["catAgentes"].Rows[0]["nOficinaID"]);
                gm.agente = Convert.ToInt32(logged.Tables["catAgentes"].Rows[0]["nAgenteID"]);
                gm.clienteID = 1;
                gm.tipoPago = "C";

                // Seteo los datos relacionados
                SetRelatedData(ref gm);

                // Default nFormaPago = 1
                gm.formaPago = 1;
                gm.cFormaPago = "T";
                gm.ValidaFormaPago(gm.formaPago.ToString());

                //Agente Responsable
                var agenteResponsable = VarProcInterfazX.arrAgentes.Where(agente => agente.lngAgente == gm.agente).First().cresponsa;
                gm.responsable = agenteResponsable == "" ? "XX" : agenteResponsable;

                var descOfc = "agentes " + (gm.oficinaID == 1 ? "df" : gm.OficinasSource.First(o => o.strCve == gm.oficinaID.ToString()).strDesc.ToLower());
                var campain = gm.CampañasSource.FirstOrDefault(c => c.strDesc.ToLower().Trim() == descOfc);
                if (campain != null)
                    gm.campaña = Convert.ToInt32(campain.strCve);

                // Guardo el modelo
                GuardaEnMemoria(gm);

                // Retorno la vista
                return PartialView("DatosGenerales", gm);
            }
            catch (Exception err)
            {
                Response.StatusCode = (int)(System.Net.HttpStatusCode.InternalServerError);
                LogDeErrores.RegistroErrorWeb(err, DatoUsuario.idAgente, DatoUsuario.nomAgente, DatoUsuario.sIp, DatoUsuario.sHostName);
                return Json("ErrInterno");
            }
        }

        /// <summary>
        /// GET Accion para crear un nuevo documento o editar uno ya registrado
        /// </summary>
        /// <param name="id">ID del documento. Si es cero es indicativo de nuevo</param>
        /// <param name="modulo">Modulo que se esta trabajando:
        ///                         Cotizaciones
        ///                         Reovaciones
        ///                         Polizas
        /// </param>
        /// <returns></returns>
        public ActionResult DocumentoEnDB(string id, string modulo)
        {

            try
            {// Busco info del documento
                GenericoViewModel gm = (GenericoViewModel)Session["ModeloEnUso"];
                if (id == "0")
                {
                    gm.IsNew = true;
                }

                // Seteo las propiedades de lista
                ViewBag.IsNew = false;
                SetCotizacionListProperties(ref gm);
                SetRelatedData(ref gm);

                ValidaVehiculos vv = new ValidaVehiculos();
                // Valido Cobertura 100
                int contCober = 0;
                int contCond = 0;
                gm.cobertura100 = vv.ValidaConvierteCobertura100Siempre(ref contCober
                                                                          , ref contCond
                                                                          , gm.cobertura100
                                                                          , gm.vehiculos
                                                                          , gm.conductores
                                                                          , "C");
                // Guardo el modelo
                GuardaEnMemoria(gm);

                // Retorno la vista
                return PartialView("DatosGenerales", gm);
            }
            catch (Exception err)
            {
                Response.StatusCode = (int)(System.Net.HttpStatusCode.InternalServerError);
                LogDeErrores.RegistroErrorWeb(err, DatoUsuario.idAgente, DatoUsuario.nomAgente, DatoUsuario.sIp, DatoUsuario.sHostName);
                return Json("ErrInterno");
            }
        }

        /// <summary>
        /// Validar el ERA en función de la forma de pago
        /// </summary>
        /// <param name="fPagoID">Forma de Pago seleccionada</param>
        /// <returns>S / N</returns>
        public ActionResult ValidaERA(string fPagoID)
        {
            try
            {
                var cERA = "N";
                var fp = VarProcInterfazX.arrPago.ToList().FirstOrDefault(p => p.strCve == fPagoID);
                if (fp != null) { cERA = fp.strEra; }

                return Content(cERA);
            }
            catch (Exception err)
            {
                Response.StatusCode = (int)(System.Net.HttpStatusCode.InternalServerError);
                LogDeErrores.RegistroErrorWeb(err, DatoUsuario.idAgente, DatoUsuario.nomAgente, DatoUsuario.sIp, DatoUsuario.sHostName);
                return Json("ErrInterno");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateDatosGenerales(GenericoViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { success = false, errores = GetElementsFromModelState(), validaciones = "" });
                }

                // Recupero el modelo
                GenericoViewModel modelComplete = (GenericoViewModel)Session["ModeloEnUso"];
                model.QuiereCederComision = false;
                CedeComision.cargado = false;
                model.conductores = modelComplete.conductores;
                model.vehiculos = modelComplete.vehiculos;
                model.complemento = modelComplete.complemento;
                model.prospectos = modelComplete.prospectos;
                model.direcciones = modelComplete.direcciones;
                model.telefono = modelComplete.telefono;
                model.SeguimientoPol = modelComplete.SeguimientoPol;
                model.TarjetCredito = modelComplete.TarjetCredito;
                model.TarjetaDebito = modelComplete.TarjetaDebito;
                model.pagos = modelComplete.pagos;
                model.sexo = (model.EsHombre ? 0 : 1);
                model.DescripcionSexo = (model.EsHombre ? "Masculino" : "Femenino");
                model.persona = (model.EsPersonaFisica ? "0" : "1");
                model.recomienda = modelComplete.recomienda;
               
                //model.control         = modelComplete.procarinicial;

                // Valido Cobertura 100
                if ((model.vehiculos != null) && (model.conductores != null))
                {
                    ValidaVehiculos vv = new ValidaVehiculos();
                    int contCober = 0;
                    int contCond = 0;
                    model.cobertura100 = vv.ValidaConvierteCobertura100Siempre(ref contCober
                                                                             , ref contCond
                                                                             , model.cobertura100
                                                                             , model.vehiculos
                                                                             , model.conductores
                                                                             , "C");
                }

                //Mandar Tipo Pago 1 Default
                modelComplete.ValidaFormaPago(model.formaPago.ToString());

                //Responsable 
                model.responsable = modelComplete.responsable;

                if (model.telefono == null)
                {
                    if ((model.PrimerTelefono.Length > 0) && (model.PrimerTelefono != null))
                    {
                        model.telefono = new List<ClsTelefono>();
                        ClsTelefono phone = new ClsTelefono(model.PrimerTelefono);
                        phone.cLada = phone.cCelular = phone.cExtension = phone.cFax = phone.cHistorial = phone.cOficina = phone.cOtro1 = phone.cRadio = phone.cRadioID = "";
                        phone.cTpo1 = "O";
                        model.telefono.Add(phone);
                    }
                }

                if (model.IsNew)
                {
                    SetRelatedData(ref model);
                }
                if (model.cNombreFinanciamiento == null)
                {
                    model.cNombreFinanciamiento = "";
                }
                // Guardo el modelo
                model.subRamo = modelComplete.subRamo;
                GuardaEnMemoria(model);
                ClearErrors();

                // Redirecciono
                return RedirectToAction("CreateConductor");
            }
            catch (Exception err)
            {
                Response.StatusCode = (int)(System.Net.HttpStatusCode.InternalServerError);
                LogDeErrores.RegistroErrorWeb(err, DatoUsuario.idAgente, DatoUsuario.nomAgente, DatoUsuario.sIp, DatoUsuario.sHostName);
                return Json("ErrInterno");
            }
        }

        private void SetRelatedData(ref GenericoViewModel model)
        {
            try
            {
                var prmID = model.promocion;
                var promo = VarProcInterfazX.arrPromocion.FirstOrDefault(p => p.nPromocionId == prmID);
                if (promo != null)
                {
                    model.nomPromocion = promo.cNombre;
                }

                var clieID = model.clienteID == 0 ? 1 : model.clienteID;
                model.clienteID = clieID;
                var client = VarProcInterfazX.arrClientes.First(c => c.lngClave == clieID);
                model.ClienteInfo = new ClsCliente()
                {
                    ClienteID = client.lngClave,
                    RazonSocial = client.strDescrip,
                };
                model.nomTitular = client.strDescrip;
            }
            catch (Exception err)
            {
                Response.StatusCode = (int)(System.Net.HttpStatusCode.InternalServerError);
                LogDeErrores.RegistroErrorWeb(err, DatoUsuario.idAgente, DatoUsuario.nomAgente, DatoUsuario.sIp, DatoUsuario.sHostName);
                throw err;
            }
        }

        /// <summary>
        /// Método que llena las listas de los dropdown
        /// </summary>
        /// <param name="model">Model en ejecución</param>
        private void SetCotizacionListProperties(ref GenericoViewModel model)
        {
            try
            {
                DropDownList ddlRiesgo = new DropDownList();
                ctlg.CargaCmbCondRestWeb(ddlRiesgo);
                Dictionary<int, string> riesgos = new Dictionary<int, string>();
                for (int i = 0; i < ddlRiesgo.Items.Count; i++)
                {
                    int idx = (ddlRiesgo.Items[i].Text.ToLower() == "no" ? 1 : (ddlRiesgo.Items[i].Text.ToLower() == "sí" ? 0 : 2));
                    riesgos.Add(idx, ddlRiesgo.Items[i].Text);
                }

                DropDownList ddlCobr = new DropDownList();
                ctlg.CargaCmbCoberturaWeb(ddlCobr);

                model.OficinasSource = VarProcInterfazX.arrCatOfic;
                model.CampañasSource = VarProcInterfazX.arrCampañas;
                model.AgentesSource = VarProcInterfazX.arrAgentes;
                model.PromocionesSource = VarProcInterfazX.arrPromocion;
                model.FormaPagoSource = VarProcInterfazX.arrTipoPago;
                model.TipoPagoSource = VarProcInterfazX.arrPago;
                model.RiesgoConductorSource = riesgos;
                model.CoberturaSource = ddlCobr.Items;
            }
            catch (Exception err)
            {
                Response.StatusCode = (int)(System.Net.HttpStatusCode.InternalServerError);
                LogDeErrores.RegistroErrorWeb(err, DatoUsuario.idAgente, DatoUsuario.nomAgente, DatoUsuario.sIp, DatoUsuario.sHostName);
                throw err;
            }
        }
        #endregion

        #region <-- Conductores Methods -->
        /// <summary>
        /// GET Accion para cuando es un documento no registrado en la DB
        /// Se crea el conductor sobre el objeto en memoria
        /// </summary>
        /// <param name="model">Modelo de datos en memoria</param>
        /// <returns></returns>
        public ActionResult CreateConductor()
        {
            try
            {
                // Recupero el modelo
                GenericoViewModel modelComplete = (GenericoViewModel)Session["ModeloEnUso"];

                // Consecutivo de Conductores
                int consec = 1;
                if (modelComplete.conductores != null)
                    consec = modelComplete.conductores.Count(c => c.nNumCond != 0) + 1;

                var driver = new ConductorModel();
                driver.nNumCond = consec;
                driver.nombre = (consec == 1 ? modelComplete.nombreAsegurado.ToUpper() : "");
                driver.EstadoCivilSource = SetConductorListProperties();
                driver.EsHombre = true;

                // Redirecciono
                return PartialView("Conductores", driver);
            }
            catch (Exception err)
            {
                Response.StatusCode = (int)(System.Net.HttpStatusCode.InternalServerError);
                LogDeErrores.RegistroErrorWeb(err, DatoUsuario.idAgente, DatoUsuario.nomAgente, DatoUsuario.sIp, DatoUsuario.sHostName);
                return Json("ErrInterno");
            }
        }

        /// <summary>
        /// Método para crear o editar conductores cuando esta registrado el documento en la DB.
        /// </summary>
        /// <param name="id">Id del documento</param>
        /// <param name="modulo">Modulo que se esta trabajando:
        ///                         Cotizaciones
        ///                         Reovaciones
        ///                         Polizas
        /// </param>
        /// <param name="conductorID">ID del conductor. Si es cero es indicativo de nuevo</param>
        /// <returns></returns>
        public ActionResult ConductorEnDocumento(string id, string modulo, int conductorID)
        {
            try
            {
                // Busco info del documento
                GenericoViewModel modelComplete = (GenericoViewModel)Session["ModeloEnUso"];
                ClsConductor cndctr = modelComplete.conductores.FirstOrDefault(c => c.nNumCond == conductorID);
                var driver = CentralAgentesMvc.App_Start.CastObject.Cast<ConductorModel>(cndctr);
                driver.nombre = cndctr.nombre.ToUpper();
                driver.Edad = DateTime.Now.Year - Convert.ToDateTime(cndctr.fechaNacimiento).Year;
                driver.fechaNacimiento = cndctr.fechaNacimiento.Substring(0, 10).Trim();
                driver.EsHombre = (cndctr.sexo == 0 ? true : false);
                driver.EstadoCivilSource = SetConductorListProperties();

                // Redirecciono
                return PartialView("Conductores", driver);
            }
            catch (Exception err)
            {
                Response.StatusCode = (int)(System.Net.HttpStatusCode.InternalServerError);
                LogDeErrores.RegistroErrorWeb(err, DatoUsuario.idAgente, DatoUsuario.nomAgente, DatoUsuario.sIp, DatoUsuario.sHostName);
                return Json("ErrInterno");
            }
        }

        /// <summary>
        /// Método usado para refrescar los datos de Conductores en el View
        /// </summary>
        /// <returns></returns>
        public JsonResult RefreshDrivers()
        {
            try
            {
                // Recupero el modelo
                GenericoViewModel d = (GenericoViewModel)Session["ModeloEnUso"];

                return Json(d.conductores.Where(c => c.nNumCond != 0), JsonRequestBehavior.AllowGet);
            }
            catch (Exception err)
            {
                Response.StatusCode = (int)(System.Net.HttpStatusCode.InternalServerError);
                LogDeErrores.RegistroErrorWeb(err, DatoUsuario.idAgente, DatoUsuario.nomAgente, DatoUsuario.sIp, DatoUsuario.sHostName);
                return Json("ErrInterno");
            }
        }

        /// <summary>
        /// Método que elimina un conductor de la lista
        /// </summary>
        /// <param name="conductorID">ID del conductor</param>
        /// <returns></returns>
        public ActionResult EliminarConductor(int conductorID)
        {
            try
            {
                var msje = "NO";

                GenericoViewModel model = (GenericoViewModel)Session["ModeloEnUso"];
                if (model.conductores.FirstOrDefault(c => c.nNumCond == conductorID) != null)
                {
                    var remove = model.conductores.FirstOrDefault(c => c.nNumCond == conductorID);
                    model.conductores.Remove(remove);
                    msje = "OK";
                }

                return Content(msje);
            }
            catch (Exception err)
            {
                Response.StatusCode = (int)(System.Net.HttpStatusCode.InternalServerError);
                LogDeErrores.RegistroErrorWeb(err, DatoUsuario.idAgente, DatoUsuario.nomAgente, DatoUsuario.sIp, DatoUsuario.sHostName);
                return Json("ErrInterno");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateConductor(ConductorModel conductor)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { success = false, errores = GetElementsFromModelState(), validaciones = "" });
                }

                // Recupero el modelo
                GenericoViewModel modelComplete = (GenericoViewModel)Session["ModeloEnUso"];

                // Valido la coleccion de conductores generales
                if (modelComplete.conductores == null) modelComplete.conductores = new List<ClsConductor>();
                if (modelComplete.conductores.FirstOrDefault(c => c.nNumCond == conductor.nNumCond) != null)
                {
                    var remove = modelComplete.conductores.FirstOrDefault(c => c.nNumCond == conductor.nNumCond);
                    modelComplete.conductores.Remove(remove);
                }

                // Agrego el conductor a la coleccion
                var driver = CentralAgentesMvc.App_Start.CastObject.Cast<ClsConductor>(conductor);
                driver.DescripcionEdoCivil = SetConductorListProperties().FirstOrDefault(e => e.Key == conductor.estadoCivil).Value;
                driver.nombre = driver.nombre.ToUpper();
                driver.sexo = (conductor.EsHombre ? 0 : 1);
                driver.DescripcionSexo = (conductor.EsHombre ? "Masculino" : "Femenino");
                modelComplete.conductores.Add(driver);

                // Guardo el modelo
                GuardaEnMemoria(modelComplete);
                ClearErrors();

                // Redirecciono
                return RedirectToAction("CreateVehiculo");
            }
            catch (Exception err)
            {
                Response.StatusCode = (int)(System.Net.HttpStatusCode.InternalServerError);
                LogDeErrores.RegistroErrorWeb(err, DatoUsuario.idAgente, DatoUsuario.nomAgente, DatoUsuario.sIp, DatoUsuario.sHostName);
                return Json("ErrInterno");
            }
        }

        /// <summary>
        /// Método que llena las listas de los dropdown
        /// </summary>
        private Dictionary<string, string> SetConductorListProperties()
        {
            try
            {
                DropDownList ddlEdoC = new DropDownList();
                ctlg.CargaCmbEdoCivilWeb(ddlEdoC);

                Dictionary<string, string> edoCiv = new Dictionary<string, string>();
                for (int i = 0; i < ddlEdoC.Items.Count; i++)
                {
                    edoCiv.Add(ddlEdoC.Items[i].Value, ddlEdoC.Items[i].Text);
                }

                return edoCiv;
            }
            catch (Exception err)
            {
                Response.StatusCode = (int)(System.Net.HttpStatusCode.InternalServerError);
                LogDeErrores.RegistroErrorWeb(err, DatoUsuario.idAgente, DatoUsuario.nomAgente, DatoUsuario.sIp, DatoUsuario.sHostName);
                throw err;
            }
        }
        #endregion

        #region <-- Vehiculos Methods -->
        /// <summary>
        /// GET Accion para cuando es un documento no registrado en la DB
        /// Se crea el vehiculo sobre el objeto en memoria
        /// </summary>
        /// <param name="model">Modelo de datos en memoria</param>
        /// <returns></returns>
        public ActionResult CreateVehiculo()
        {
            try
            {
                // Recupero el modelo
                var modelComplete = (GenericoViewModel)Session["ModeloEnUso"];

                // Consecutivo de Vehiculos
                int consec = 1;
                if (modelComplete.vehiculos != null)
                {
                    consec = modelComplete.vehiculos.Count + 1;
                }

                // Valores por defecto de propiedades
                var car = new VehiculoModel();
                car.nNumVehi = consec;
                car.codigoPostal = modelComplete.codigoPostal;
                car.EstacionaEnCasa = true;
                car.EstacionaEnOfic = true;
                car.UsaParaTrabajo = false;
                car.UsaAsesoriaLegal = true;

                //Default cValidaSerie 
                //Tipo Número de Serie
                car.EsSerieCorrecta = false;
                car.numeroSerie = "";
                car.placas = "";
                car.descEqEsp = "";
                car.ccNCI = "";
                car.fecEBC = "";
                car.pagEBC = "";
                car.autorizaSerie = "";
                car.VIN = "";

                car.TieneSUVA = false;
                car.deducDañMat = 5;
                car.deducRobTot = 10;
                car.coberGtoMed = 100000;
                car.coberRepCiv = 500000;
                car.coberRepCivCat = 3000000;

                // Datos heredados de póliza
                SetHerenciaProperties(ref car, modelComplete);

                // Seteo las listas
                SetVehiculoListProperties(ref car);

                // Conductor por defecto
                car.conductores = modelComplete.conductores;
                if (car.conductores == null) car.conductores = new List<ClsConductor>();
                if (car.conductores.FirstOrDefault(c => c.nNumCond == 0) == null)
                {
                    car.conductores.Insert(0, new ClsConductor() { nNumCond = 0, nombre = "-- Seleccione un conductor asignado --" });
                    car.conducAsign = 0;
                }

                // Redirecciono
                return PartialView("Vehiculos", car);
            }
            catch (Exception err)
            {
                Response.StatusCode = (int)(System.Net.HttpStatusCode.InternalServerError);
                LogDeErrores.RegistroErrorWeb(err, DatoUsuario.idAgente, DatoUsuario.nomAgente, DatoUsuario.sIp, DatoUsuario.sHostName);
                return Json("ErrInterno");
            }
        }

        /// <summary>
        /// Método para crear o editar vehiculos cuando esta registrado el documento en la DB.
        /// </summary>
        /// <param name="id">Id del documento</param>
        /// <param name="modulo">Modulo que se esta trabajando:
        ///                         Cotizaciones
        ///                         Reovaciones
        ///                         Polizas
        /// </param>
        /// <param name="vehiculoID">ID del Vehiculo. Si es cero es indicativo de nuevo</param>
        /// <returns></returns>
        public ActionResult VehiculoEnDocumento(string id, string modulo, int vehiculoID)
        {
            try
            {
                // Busco info del documento
                GenericoViewModel model = (GenericoViewModel)Session["ModeloEnUso"];
                ClsVehiculo vhclo = model.vehiculos.FirstOrDefault(c => c.nNumVehi == vehiculoID);
                var car = CentralAgentesMvc.App_Start.CastObject.Cast<VehiculoModel>(vhclo);

                // Seteo las listas
                SetVehiculoListProperties(ref car);

                // Datos heredados de póliza
                SetHerenciaProperties(ref car, model);

                // Conductor por defecto
                car.conductores = model.conductores;
                car.codigoPostal = vhclo.codigoPostal;

                if (car.conductores.FirstOrDefault(c => c.nNumCond == 0) == null)
                    car.conductores.Insert(0, new ClsConductor() { nNumCond = 0, nombre = "-- Seleccione un conductor asignado --" });

                // Redirecciono
                return PartialView("Vehiculos", car);
            }
            catch (Exception err)
            {
                Response.StatusCode = (int)(System.Net.HttpStatusCode.InternalServerError);
                LogDeErrores.RegistroErrorWeb(err, DatoUsuario.idAgente, DatoUsuario.nomAgente, DatoUsuario.sIp, DatoUsuario.sHostName);
                return Json("ErrInterno");
            }
        }

        /// <summary>
        /// Método usado para refrescar los datos de Vehiculos en el View
        /// </summary>
        /// <returns></returns>
        public JsonResult RefreshCars()
        {
            try
            {
                // Recupero el modelo
                GenericoViewModel d = (GenericoViewModel)Session["ModeloEnUso"];

                return Json(d.vehiculos, JsonRequestBehavior.AllowGet);
            }
            catch (Exception err)
            {
                Response.StatusCode = (int)(System.Net.HttpStatusCode.InternalServerError);
                LogDeErrores.RegistroErrorWeb(err, DatoUsuario.idAgente, DatoUsuario.nomAgente, DatoUsuario.sIp, DatoUsuario.sHostName);
                return Json("ErrInterno");
            }
        }

        /// <summary>
        /// Validar la serie
        /// </summary>
        /// <param name="subID">Subramo del vehículo</param>
        /// <param name="modID">Modelo del vehículo</param>
        /// <param name="year">Año del vehículo</param>
        /// <param name="serie">Serie del vehículo</param>
        /// <returns>OK ó Mensaje de Error </returns>
        public ActionResult ValidaSerie(string subID, string modID, string year, string serie)
        {
            try
            {
                ValidaVehiculos vldV = new ValidaVehiculos();
                var msje = "OK";
                //if (vldV.ValidaSerieOriginal(serie, subID, modID, year, "S") == 1)
                //{
                //    msje = MensajesError.mms;
                //}
                return Content(msje);
            }
            catch (Exception err)
            {
                Response.StatusCode = (int)(System.Net.HttpStatusCode.InternalServerError);
                LogDeErrores.RegistroErrorWeb(err, DatoUsuario.idAgente, DatoUsuario.nomAgente, DatoUsuario.sIp, DatoUsuario.sHostName);
                return Json("ErrInterno");
            }
        }

        /// <summary>
        /// Método que elimina un vehículo de la lista
        /// </summary>
        /// <param name="vehiculoID">ID del vehículo</param>
        /// <returns></returns>
        public ActionResult EliminarVehiculo(int vehiculoID)
        {
            try
            {
                var msje = "NO";

                GenericoViewModel model = (GenericoViewModel)Session["ModeloEnUso"];
                if (model.vehiculos.FirstOrDefault(c => c.nNumVehi == vehiculoID) != null)
                {
                    var remove = model.vehiculos.FirstOrDefault(c => c.nNumVehi == vehiculoID);
                    model.vehiculos.Remove(remove);
                    msje = "OK";
                }

                return Content(msje);
            }
            catch (Exception err)
            {
                Response.StatusCode = (int)(System.Net.HttpStatusCode.InternalServerError);
                LogDeErrores.RegistroErrorWeb(err, DatoUsuario.idAgente, DatoUsuario.nomAgente, DatoUsuario.sIp, DatoUsuario.sHostName);
                return Json("ErrInterno");
            }
        }

        /// <summary>
        /// Levantar y mostrar los datos de placas y serie
        /// </summary>
        /// <returns></returns>
        public ActionResult SeriePlacaExp()
        {
            return DetallePlacaExpress("LOAD");
        }

        /// <summary>
        /// Levantar y mostrar los datos de oplacas y serie
        /// </summary>
        /// <returns></returns>
        public ActionResult DetallePlacaExpress(string option)
        {
            try
            {
                // Recupero el modelo
                GenericoViewModel d = (GenericoViewModel)Session["ModeloEnUso"];
                var car = new CarExpressModel();
                if (d != null)
                {
                    if (d.vehiculos != null)
                    {
                        if (d.vehiculos.Count > 0)
                        {
                            car.VehiculoID = d.vehiculos.First().numVehiculos;
                            car.Placa = d.vehiculos.First().placas;
                            car.Serie = d.vehiculos.First().numeroSerie;
                        }
                    }
                }

                // Redirecciono
                var view = (option == "LOAD" ? "DatosPlaca" : "_DatosPlacaPartial");
                return PartialView(view, car);
            }
            catch (Exception err)
            {
                Response.StatusCode = (int)(System.Net.HttpStatusCode.InternalServerError);
                LogDeErrores.RegistroErrorWeb(err, DatoUsuario.idAgente, DatoUsuario.nomAgente, DatoUsuario.sIp, DatoUsuario.sHostName);
                return Json("ErrInterno");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdatePlacaVehiculo(CarExpressModel coche)
        {
            try
            {
                // Recupero el modelo
                GenericoViewModel mc = (GenericoViewModel)Session["ModeloEnUso"];

                // Busco el registro
                if (mc.vehiculos.FirstOrDefault(c => c.nNumVehi == coche.VehiculoID) != null)
                {
                    mc.vehiculos.FirstOrDefault(c => c.nNumVehi == coche.VehiculoID).numeroSerie = coche.Serie;
                    mc.vehiculos.FirstOrDefault(c => c.nNumVehi == coche.VehiculoID).placas = coche.Placa;
                }

                GuardaEnMemoria(mc);
                ClearErrors();

                // Redirecciono
                return PartialView("_DatosPlacaPartial", coche);
            }
            catch (Exception err)
            {
                Response.StatusCode = (int)(System.Net.HttpStatusCode.InternalServerError);
                LogDeErrores.RegistroErrorWeb(err, DatoUsuario.idAgente, DatoUsuario.nomAgente, DatoUsuario.sIp, DatoUsuario.sHostName);
                return Json("ErrInterno");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateVehiculo(VehiculoModel coche)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { success = false, errores = GetElementsFromModelState(), validaciones = "" });
                }
                if (coche.PolObligatoria == "S")
                {
                    ValSeguroObli(coche.nNumVehi, coche.coberRepCiv);
                }
                //Antes de Guardar un Vehículo, hay algunas validaciones adicionales

                var logged = (DataSet)Session["UserObj"];
                var agentID = logged.Tables["catAgentes"].Rows[0]["nAgenteID"].ToString();

                // Recupero el modelo
                GenericoViewModel modelComplete = (GenericoViewModel)Session["ModeloEnUso"];
                modelComplete.conductores.Remove(modelComplete.conductores.FirstOrDefault(x=> x.nombre== "-- Seleccione un conductor asignado --"));
                Dictionary<string, object> results = new Dictionary<string, object>();
                if (modelComplete.ModulodeTrabajo == "Cotizaciones")
                {
                    // Validaciones Nueva Cotización

                    ClsVehiculo vehiculo = CentralAgentesMvc.App_Start.CastObject.Cast<ClsVehiculo>(coche);
                    results = vehiculo.ValidaVehiculoWeb(vehiculo, "C", agentID);
                }
                else
                {
                    ClsVehiculo vehiculo = CentralAgentesMvc.App_Start.CastObject.Cast<ClsVehiculo>(coche);
                    results = vehiculo.ValidaVehiculoWeb(vehiculo, "R", agentID);
                    // Validaciones Renovación
                }

                if (results.First().Value.ToString() != "OK")
                {
                    return Json(new { success = false, errores = "", validaciones = results });
                }


                // Inicializo coleccion si estan null
                if (modelComplete.vehiculos == null) modelComplete.vehiculos = new List<ClsVehiculo>();
                if (modelComplete.prospectos == null) modelComplete.prospectos = new List<ClsProspecto>();
                if (modelComplete.complemento == null) modelComplete.complemento = new ClsComplemento();

                // Elimino el conductor 0
                if (modelComplete.conductores != null)
                {
                    var cndR = modelComplete.conductores.FirstOrDefault(c => c.nNumCond == 0);
                    if (cndR != null)
                        modelComplete.conductores.Remove(cndR);
                }

                // Verifico si es edición
                if (modelComplete.vehiculos.FirstOrDefault(c => c.nNumVehi == coche.nNumVehi) != null)
                {
                    var remove = modelComplete.vehiculos.FirstOrDefault(c => c.nNumVehi == coche.nNumVehi);
                    modelComplete.vehiculos.Remove(remove);
                }

                // Agrego el vehiculo a la colección
                var car = CentralAgentesMvc.App_Start.CastObject.Cast<ClsVehiculo>(coche);
                if (coche.conducAsign != 0)
                {
                    car.NombreConductorA = modelComplete.conductores.First(c => c.nNumCond == coche.conducAsign).nombre.ToUpper();
                }
                modelComplete.vehiculos.Add(car);

                // Valido el TERSSA
                int contTerssa = 0;
                ValidaVehiculos vv = new ValidaVehiculos();
                modelComplete.tersa = vv.ValidaConvierteTerssa(ref contTerssa, modelComplete.tersa, modelComplete.vehiculos, "C");

                // Valido Cobertura 100
                int contCober = 0;
                int contCond = 0;
                modelComplete.cobertura100 = vv.ValidaConvierteCobertura100Siempre(ref contCober
                                                                                 , ref contCond
                                                                                 , modelComplete.cobertura100
                                                                                 , modelComplete.vehiculos
                                                                                 , modelComplete.conductores
                                                                                 , "C");

                // Guardo el modelo
                SetPorcentajeComisionList(ref modelComplete);
                ViewBag.SubTitle = (modelComplete.ModulodeTrabajo == "Cotizaciones" ? "cotización" : "renovación");
                GuardaEnMemoria(modelComplete);
                ClearErrors();

                // Redirecciono
                return PartialView("DetalleGeneral", modelComplete);
            }
            catch (Exception err)
            {
                Response.StatusCode = (int)(System.Net.HttpStatusCode.InternalServerError);
                LogDeErrores.RegistroErrorWeb(err, DatoUsuario.idAgente, DatoUsuario.nomAgente, DatoUsuario.sIp, DatoUsuario.sHostName);
                return Json("ErrInterno");
            }
        }

        private void SetVehiculoListProperties(ref VehiculoModel car)
        {
            try
            {
                ctzC.ControllerContext = this.ControllerContext;
                DropDownList ddlSumA = new DropDownList();
                ctlg.CargaCmbSAFWeb(ddlSumA);
                car.SumaAseguradaSource = new Dictionary<string, string>();
                for (int i = 0; i < ddlSumA.Items.Count; i++)
                    car.SumaAseguradaSource.Add(ddlSumA.Items[i].Value, ddlSumA.Items[i].Text);

                DropDownList ddlGuiaEBC = new DropDownList();
                ctlg.CargaCmbDatGuiaEbcWeb(ddlGuiaEBC);

                car.SubRamoSource = VarProcInterfazX.arrSubRamo; 
                car.YearsSource = (Dictionary<string, string>)ctzC.ObtenAños("R").Data;

                DropDownList ddlAViaje = new DropDownList();
                ctlg.CargaCmbAsisteViajeWeb(ddlAViaje);
                car.AsistenciaViajeSource = new Dictionary<string, string>();
                for (int i = 0; i < ddlAViaje.Items.Count; i++)
                    car.AsistenciaViajeSource.Add(ddlAViaje.Items[i].Value, ddlAViaje.Items[i].Text);

                DropDownList ddlVhSus = new DropDownList();
                ctlg.CargaCmbVehSusWeb(ddlVhSus);
                car.VehSustitutoSource = new Dictionary<string, string>();
                for (int i = 0; i < ddlVhSus.Items.Count; i++)
                    car.VehSustitutoSource.Add(ddlVhSus.Items[i].Value, ddlVhSus.Items[i].Text);

                car.PorcentajeDMSource = VarProcInterfazX.arrDeducibleDM.OrderBy(d => d.curMonto);
                car.PorcentajeRTSource = VarProcInterfazX.arrDeducibleRT.OrderBy(d => d.curMonto);
                car.CoberturaGMSource = VarProcInterfazX.arrCoberturaGM.OrderBy(d => d.curMonto);
                car.CoberturaRCSource = VarProcInterfazX.arrCoberturaRC.OrderBy(d => d.curMonto);
                car.CoberturaRCCSource = VarProcInterfazX.arrCoberturaRCCat.OrderBy(d => d.curMonto);
                car.TipoTransmisionSource = VarProcInterfazX.arrTransmision;
                car.TipoAlarmaSource = VarProcInterfazX.arrTpoAlarma;
                car.GuiaEBCSource = ddlGuiaEBC.Items;
            }
            catch (Exception err)
            {
                Response.StatusCode = (int)(System.Net.HttpStatusCode.InternalServerError);
                LogDeErrores.RegistroErrorWeb(err, DatoUsuario.idAgente, DatoUsuario.nomAgente, DatoUsuario.sIp, DatoUsuario.sHostName);
                throw err;
            }
        }
        private void SetHerenciaProperties(ref VehiculoModel car, GenericoViewModel modelComplete)
        {
            try
            {
                // Datos heredados de póliza
                car.titulo = modelComplete.titulo;
                car.apellidoPaterno = modelComplete.apellidoPaterno;
                car.apellidoMaterno = modelComplete.apellidoMaterno;
                car.nombres = modelComplete.nombres;
                car.campaña = modelComplete.campaña;
                car.numConductores = modelComplete.numConductores;
                car.numVehiculos = modelComplete.numVehiculos;
            }
            catch (Exception err)
            {
                Response.StatusCode = (int)(System.Net.HttpStatusCode.InternalServerError);
                LogDeErrores.RegistroErrorWeb(err, DatoUsuario.idAgente, DatoUsuario.nomAgente, DatoUsuario.sIp, DatoUsuario.sHostName);
                throw err;
            }
        }
        #endregion

        #region <-- Direcciones Methods -->
        public ActionResult DireccionExp()
        {
            try
            {
                var modelComplete = (GenericoViewModel)Session["ModeloEnUso"];
                if (modelComplete.direcciones.Count == 0)
                    return CreateDireccion();
                else
                    return DireccionEnDocumento(modelComplete.cotizacionID.ToString(), "Cotizaciones", int.Parse(modelComplete.direcciones[0].nTipoDirID.ToString()));
            }
            catch (Exception err)
            {
                Response.StatusCode = (int)(System.Net.HttpStatusCode.InternalServerError);
                LogDeErrores.RegistroErrorWeb(err, DatoUsuario.idAgente, DatoUsuario.nomAgente, DatoUsuario.sIp, DatoUsuario.sHostName);
                return Json("ErrInterno");
            }
        }

        public ActionResult CreateDireccion()
        {
            try
            {
                // Recupero el modelo
                var modelComplete = (GenericoViewModel)Session["ModeloEnUso"];

                // Consecutivo de direcciones
                int consec = 1;
                if (modelComplete.direcciones != null)
                    consec = modelComplete.direcciones.Count + 1;

                var address = new DireccionModel();
                address.Consecutivo = consec;
                address.nCP = modelComplete.codigoPostal;
                SetAdressListProperties(ref address);
                if (User.Identity.Name.ToString().Trim() == "COTIZADOR EN LINEA")
                {
                    address.SeImprime = true;
                    address.EsDireccionFiscal = true;
                    address.cFiscal = "S";
                    address.cRequerida = "S";
                }
                // Redirecciono
                return PartialView("Direcciones", address);
            }
            catch (Exception err)
            {
                Response.StatusCode = (int)(System.Net.HttpStatusCode.InternalServerError);
                LogDeErrores.RegistroErrorWeb(err, DatoUsuario.idAgente, DatoUsuario.nomAgente, DatoUsuario.sIp, DatoUsuario.sHostName);
                return Json("ErrInterno");
            }
        }

        public ActionResult DireccionEnDocumento(string id, string modulo, int addressID)
        {
            try
            {
                // Busco info del documento
                GenericoViewModel modelComplete = (GenericoViewModel)Session["ModeloEnUso"];
                ClsDir dir = modelComplete.direcciones.FirstOrDefault(c => c.nTipoDirID == addressID);
                var address = CentralAgentesMvc.App_Start.CastObject.Cast<DireccionModel>(dir);
                address.EsDireccionFiscal = (dir.cFiscal == "S" ? true : false);
                address.SeImprime = (dir.cRequerida == "S" ? true : false);
                address.RFC_ = modelComplete.RFC;
                SetAdressListProperties(ref address);

                // Redirecciono
                return PartialView("Direcciones", address);
            }
            catch (Exception err)
            {
                Response.StatusCode = (int)(System.Net.HttpStatusCode.InternalServerError);
                LogDeErrores.RegistroErrorWeb(err, DatoUsuario.idAgente, DatoUsuario.nomAgente, DatoUsuario.sIp, DatoUsuario.sHostName);
                return Json("ErrInterno");
            }
        }

        /// <summary>
        /// Método usado para refrescar los datos de Direcciones en el View
        /// </summary>
        /// <returns></returns>
        public JsonResult RefreshAddress()
        {
            try
            {
                // Recupero el modelo
                GenericoViewModel d = (GenericoViewModel)Session["ModeloEnUso"];

                return Json(d.direcciones, JsonRequestBehavior.AllowGet);
            }
            catch (Exception err)
            {
                Response.StatusCode = (int)(System.Net.HttpStatusCode.InternalServerError);
                LogDeErrores.RegistroErrorWeb(err, DatoUsuario.idAgente, DatoUsuario.nomAgente, DatoUsuario.sIp, DatoUsuario.sHostName);
                return Json("ErrInterno");
            }
        }

        /// <summary>
        /// Validar el Código Postal ingresado y cambiar el combo de Estado
        /// </summary>
        /// <param name="cp">Código Postal</param>
        /// <returns>Id del Estado</returns>
        public JsonResult ValidatePostalCode(long cp)
        {
            try
            {
                var stateID = 2;
                var objStat = VarProcInterfazX.arrCodigoPostal.FirstOrDefault(c => c.Clave == cp);
                if (objStat != null)
                {
                    stateID = objStat.Estado;
                }

                return Json(stateID, JsonRequestBehavior.AllowGet);
            }
            catch (Exception err)
            {
                Response.StatusCode = (int)(System.Net.HttpStatusCode.InternalServerError);
                LogDeErrores.RegistroErrorWeb(err, DatoUsuario.idAgente, DatoUsuario.nomAgente, DatoUsuario.sIp, DatoUsuario.sHostName);
                return Json("ErrInterno");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateDireccion(DireccionModel dir)
        {
            try
            {
                if (User.Identity.Name.ToString().Trim() != "COTIZADOR EN LINEA")
                {
                    ModelState["RFC_"].Errors.Clear();
                }
                if (!ModelState.IsValid)
                {
                    return Json(new { success = false, errores = GetElementsFromModelState(), validaciones = "" });
                }

                // Recupero el modelo
                GenericoViewModel modelComplete = (GenericoViewModel)Session["ModeloEnUso"];

                // Agrego la direccion a la coleccion
                if (modelComplete.direcciones == null) modelComplete.direcciones = new List<ClsDir>();

                if (modelComplete.direcciones.FirstOrDefault(d => d.Consecutivo == dir.Consecutivo) != null)
                {
                    var remove = modelComplete.direcciones.FirstOrDefault(d => d.Consecutivo == dir.Consecutivo);
                    modelComplete.direcciones.Remove(remove);
                }

                var address = CentralAgentesMvc.App_Start.CastObject.Cast<ClsDir>(dir);
                address.Consecutivo = modelComplete.direcciones.Count + 1;
                address.cFiscal = (dir.EsDireccionFiscal ? "S" : "N");
                address.cRequerida = (dir.SeImprime ? "S" : "N");
                modelComplete.direcciones.Add(address);

                // Seteo las propiedades
                SetAdressListProperties(ref dir);

                modelComplete.RFC = dir.RFC_;
                // Guardo el modelo
                GuardaEnMemoria(modelComplete);
                ClearErrors();

                // Redirecciono
                return PartialView("Direcciones", dir);
            }
            catch (Exception err)
            {
                Response.StatusCode = (int)(System.Net.HttpStatusCode.InternalServerError);
                LogDeErrores.RegistroErrorWeb(err, DatoUsuario.idAgente, DatoUsuario.nomAgente, DatoUsuario.sIp, DatoUsuario.sHostName);
                return Json("ErrInterno");
            }
        }

        private void SetAdressListProperties(ref DireccionModel address)
        {
            try
            {
                address.TipoDirSource = VarProcInterfazX.arrTipoDir;
                address.EstadosSource = VarProcInterfazX.arrEstados;
                address.nEstadoID = 2;
            }
            catch (Exception err)
            {
                Response.StatusCode = (int)(System.Net.HttpStatusCode.InternalServerError);
                LogDeErrores.RegistroErrorWeb(err, DatoUsuario.idAgente, DatoUsuario.nomAgente, DatoUsuario.sIp, DatoUsuario.sHostName);
                throw err;
            }
        }

        public ActionResult DetalleDireccionExpress()
        {
            try
            {
                // Recupero el modelo
                GenericoViewModel d = (GenericoViewModel)Session["ModeloEnUso"];
                var address = new DireccionModel();
                if (d != null)
                {
                    if (d.direcciones != null)
                    {
                        if (d.direcciones.Count > 0)
                            address = CentralAgentesMvc.App_Start.CastObject.Cast<DireccionModel>(d.direcciones.First());
                    }
                }
                SetAdressListProperties(ref address);

                // Redirecciono
                return PartialView("_DatosDirPartial", address);
            }
            catch (Exception err)
            {
                Response.StatusCode = (int)(System.Net.HttpStatusCode.InternalServerError);
                LogDeErrores.RegistroErrorWeb(err, DatoUsuario.idAgente, DatoUsuario.nomAgente, DatoUsuario.sIp, DatoUsuario.sHostName);
                return Json("ErrInterno");
            }
        }
        #endregion

        #region <-- Teléfonos Methods -->
        public ActionResult CreateTelefono()
        {
            try
            {
                // Recupero el modelo
                var modelComplete = (GenericoViewModel)Session["ModeloEnUso"];

                var phone = new ClsTelefono();
                phone.cTpo1 = "O";

                // Redirecciono
                return PartialView("Telefonos", phone);
            }
            catch (Exception err)
            {
                Response.StatusCode = (int)(System.Net.HttpStatusCode.InternalServerError);
                LogDeErrores.RegistroErrorWeb(err, DatoUsuario.idAgente, DatoUsuario.nomAgente, DatoUsuario.sIp, DatoUsuario.sHostName);
                return Json("ErrInterno");
            }
        }

        public ActionResult TelefonoEnDocumento(string phoneID)
        {
            try
            {
                // Busco info del documento
                GenericoViewModel modelComplete = (GenericoViewModel)Session["ModeloEnUso"];
                ClsTelefono tlf = modelComplete.telefono.FirstOrDefault(c => c.cTpo1 == phoneID);
                tlf.nCotizaID = modelComplete.cotizacionID.ToString();

                // Redirecciono
                return PartialView("Telefonos", tlf);
            }
            catch (Exception err)
            {
                Response.StatusCode = (int)(System.Net.HttpStatusCode.InternalServerError);
                LogDeErrores.RegistroErrorWeb(err, DatoUsuario.idAgente, DatoUsuario.nomAgente, DatoUsuario.sIp, DatoUsuario.sHostName);
                return Json("ErrInterno");
            }
        }

        /// <summary>
        /// Método usado para refrescar los datos de Teléfonos en el View
        /// </summary>
        /// <returns></returns>
        public JsonResult RefreshPhones()
        {
            try
            {
                // Recupero el modelo
                GenericoViewModel d = (GenericoViewModel)Session["ModeloEnUso"];

                return Json(d.telefono, JsonRequestBehavior.AllowGet);
            }
            catch (Exception err)
            {
                Response.StatusCode = (int)(System.Net.HttpStatusCode.InternalServerError);
                LogDeErrores.RegistroErrorWeb(err, DatoUsuario.idAgente, DatoUsuario.nomAgente, DatoUsuario.sIp, DatoUsuario.sHostName);
                return Json("ErrInterno");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateTelefono(ClsTelefono phone)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { success = false, errores = GetElementsFromModelState(), validaciones = "" });
                }

                // Recupero el modelo
                GenericoViewModel modelComplete = (GenericoViewModel)Session["ModeloEnUso"];

                // Agrego el teléfono a la coleccion
                if (modelComplete.telefono == null) modelComplete.telefono = new List<ClsTelefono>();

                if (modelComplete.telefono.FirstOrDefault() != null)
                {
                    var remove = modelComplete.telefono.FirstOrDefault();
                    modelComplete.telefono.Remove(remove);
                }

                phone.cTpo1 = "O";
                phone.cLada = (phone.cLada == null ? "" : phone.cLada);
                phone.cCelular = (phone.cCelular == null ? "" : phone.cCelular);
                phone.cExtension = (phone.cExtension == null ? "" : phone.cExtension);
                phone.cFax = (phone.cFax == null ? "" : phone.cFax);
                phone.cHistorial = (phone.cHistorial == null ? "" : phone.cHistorial);
                phone.cOficina = (phone.cOficina == null ? "" : phone.cOficina);
                phone.cOtro1 = (phone.cOtro1 == null ? "" : phone.cOtro1);
                phone.cRadio = (phone.cRadio == null ? "" : phone.cRadio);
                phone.cRadioID = (phone.cRadioID == null ? "" : phone.cRadioID);
                modelComplete.telefono.Add(phone);

                // Guardo el modelo
                GuardaEnMemoria(modelComplete);
                ClearErrors();

                // Redirecciono
                return PartialView("Telefonos", phone);
            }
            catch (Exception err)
            {
                Response.StatusCode = (int)(System.Net.HttpStatusCode.InternalServerError);
                LogDeErrores.RegistroErrorWeb(err, DatoUsuario.idAgente, DatoUsuario.nomAgente, DatoUsuario.sIp, DatoUsuario.sHostName);
                return Json("ErrInterno");
            }
        }
        #endregion

        #region <-- Desglose Pagos Methods -->
        /// <summary>
        /// Método que levanta la ventana para cargar el detalle del pago cuando es 
        /// Cotización Express
        /// </summary>
        /// <returns></returns>
        public ActionResult DesgloseExp()
        {
            try
            {
                var modelComplete = (GenericoViewModel)Session["ModeloEnUso"];
                if (modelComplete.TarjetCredito.Count == 0)
                    return CreateDesglosePago();
                else
                    return DesglosePagoEnDocumento("TARJETA DE CREDITO", int.Parse(modelComplete.TarjetCredito[0].ID.ToString()));
            }
            catch (Exception err)
            {
                Response.StatusCode = (int)(System.Net.HttpStatusCode.InternalServerError);
                LogDeErrores.RegistroErrorWeb(err, DatoUsuario.idAgente, DatoUsuario.nomAgente, DatoUsuario.sIp, DatoUsuario.sHostName);
                return Json("ErrInterno");
            }
        }

        public ActionResult CreateDesglosePago()
        {
            try
            {
                // Recupero el modelo
                var modelComplete = (GenericoViewModel)Session["ModeloEnUso"];

                // Consecutivo de Desglose Pagos
                int consec = 1;

                switch (modelComplete.TipoPagoInfo.Descripcion.ToUpper())
                {
                    case "EFECTIVO":
                    case "DEPOSITO":
                        if (modelComplete.pagos != null)
                        {
                            if (modelComplete.pagos.pagoOtro != null)
                                consec = modelComplete.pagos.pagoOtro.Count + 1;
                        }
                        break;

                    case "TARJETA DE DEBITO":
                        if (modelComplete.TarjetaDebito != null)
                            consec = modelComplete.TarjetaDebito.Count + 1;
                        break;

                    case "TARJETA DE CREDITO":
                        if (modelComplete.TarjetCredito != null)
                            consec = modelComplete.TarjetCredito.Count + 1;
                        break;
                }

                var desglosePago = new DesglosePagoModel();
                desglosePago.ID = consec;
                desglosePago.indice = consec;

                if (modelComplete.ObjCalculo != null)
                {
                    desglosePago.mMonto = (modelComplete.ObjCalculo.mPago1de == 0 ? modelComplete.ObjCalculo.mTotal : modelComplete.ObjCalculo.mPago1de);
                    desglosePago.mMontoSig = modelComplete.ObjCalculo.mPago2de;
                }
                desglosePago.TipoPagoDesc = modelComplete.TipoPagoInfo.Descripcion;
                SetDesgloseListProperties(ref desglosePago);

                return PartialView("DesglosePago", desglosePago);
            }
            catch (Exception err)
            {
                Response.StatusCode = (int)(System.Net.HttpStatusCode.InternalServerError);
                LogDeErrores.RegistroErrorWeb(err, DatoUsuario.idAgente, DatoUsuario.nomAgente, DatoUsuario.sIp, DatoUsuario.sHostName);
                return Json("ErrInterno");
            }
        }

        public ActionResult DesglosePagoEnDocumento(string tipoPago, int pagoID)
        {
            try
            {
                // Busco info del documento
                GenericoViewModel modelComplete = (GenericoViewModel)Session["ModeloEnUso"];
                DesglosePagoModel dp = new DesglosePagoModel();

                switch (tipoPago.ToUpper())
                {
                    case "EFECTIVO":
                    case "DEPOSITO":
                        var pagoE = modelComplete.pagos.pagoOtro.FirstOrDefault(t => t.ID == pagoID);
                        dp = CentralAgentesMvc.App_Start.CastObject.Cast<DesglosePagoModel>(pagoE);
                        dp.ID = pagoE.ID;
                        dp.dfVigtarjeta = "";
                        dp.VigenciaMonth = "";
                        dp.VigenciaYear = "";
                        dp.TipoPagoDesc = tipoPago;
                        break;

                    case "TARJETA DE DEBITO":
                        var pagoD = modelComplete.TarjetaDebito.FirstOrDefault(t => t.ID == pagoID);
                        dp = CentralAgentesMvc.App_Start.CastObject.Cast<DesglosePagoModel>(pagoD);
                        dp.ID = pagoD.ID;
                        dp.dfVigtarjeta = dp.dfVigtarjeta.Substring(3);
                        dp.VigenciaMonth = dp.dfVigtarjeta.Substring(0, 2);
                        dp.VigenciaYear = dp.dfVigtarjeta.Substring(3, 4);
                        dp.TipoPagoDesc = tipoPago;
                        break;

                    case "TARJETA DE CREDITO":
                        var pagoC = modelComplete.TarjetCredito.FirstOrDefault(t => t.ID == pagoID);
                        dp = CentralAgentesMvc.App_Start.CastObject.Cast<DesglosePagoModel>(pagoC);
                        dp.ID = pagoC.ID;
                        dp.dfVigtarjeta = dp.dfVigtarjeta.Substring(3);
                        dp.VigenciaMonth = dp.dfVigtarjeta.Substring(0, 2);
                        dp.VigenciaYear = dp.dfVigtarjeta.Substring(3, 4);
                        dp.TipoPagoDesc = tipoPago;
                        break;
                }

                SetDesgloseListProperties(ref dp);
                if (modelComplete.ObjCalculo != null)
                {
                    dp.mMonto = (modelComplete.ObjCalculo.mPago1de == 0 ? modelComplete.ObjCalculo.mTotal : modelComplete.ObjCalculo.mPago1de);
                    dp.mMontoSig = modelComplete.ObjCalculo.mPago2de;
                }
                // Redirecciono
                return PartialView("DesglosePago", dp);
            }
            catch (Exception err)
            {
                Response.StatusCode = (int)(System.Net.HttpStatusCode.InternalServerError);
                LogDeErrores.RegistroErrorWeb(err, DatoUsuario.idAgente, DatoUsuario.nomAgente, DatoUsuario.sIp, DatoUsuario.sHostName);
                return Json("ErrInterno");
            }
        }

        /// <summary>
        /// Validar el Número de Tarjeta ingresado
        /// </summary>
        /// <param name="tarjeta"></param>
        /// <returns></returns>
        public ActionResult ValidaTarjeta(string tarjeta)
        {
            try
            {
                cValCC vldT = new cValCC();
                vldT.NumeroTC = tarjeta;
                bool isValid = vldT.ValidaTar();
                return Content(vldT.TipoTC);
            }
            catch (Exception err)
            {
                Response.StatusCode = (int)(System.Net.HttpStatusCode.InternalServerError);
                LogDeErrores.RegistroErrorWeb(err, DatoUsuario.idAgente, DatoUsuario.nomAgente, DatoUsuario.sIp, DatoUsuario.sHostName);
                return Json("ErrInterno");
            }
        }

        /// <summary>
        /// Método usado para refrescar la tabla de los desgloses de pago
        /// </summary>
        /// <returns></returns>
        public JsonResult RefreshDesglosePagos()
        {
            try
            {
                // Recupero el modelo
                GenericoViewModel d = (GenericoViewModel)Session["ModeloEnUso"];
                SetMediosDePago(ref d);

                return Json(d.DocumentosPago, JsonRequestBehavior.AllowGet);
            }
            catch (Exception err)
            {
                Response.StatusCode = (int)(System.Net.HttpStatusCode.InternalServerError);
                LogDeErrores.RegistroErrorWeb(err, DatoUsuario.idAgente, DatoUsuario.nomAgente, DatoUsuario.sIp, DatoUsuario.sHostName);
                return Json("ErrInterno");
            }
        }

        private void SetMediosDePago(ref GenericoViewModel d)
        {
            try
            {
                // Cargo los pagos en una unica colección
                d.DocumentosPago = new List<DesglosePagoModel>();
                if (d.TarjetCredito != null)
                {
                    foreach (var tc in d.TarjetCredito)
                    {
                        var dp = new DesglosePagoModel()
                        {
                            TipoPagoDesc = "TARJETA DE CREDITO",
                            ID = tc.ID,
                            cTpoTarjeta = tc.cTpoTarjeta,
                            cBancoDesc = tc.cBancoDesc,
                            mMonto = tc.mMonto,
                            mMontoSig = tc.mMontoSig,
                            cNoTarjeta = tc.cNoTarjeta,
                            dfVigtarjeta = tc.dfVigtarjeta.Substring(3),
                            VigenciaMonth = tc.dfVigtarjeta.Substring(3, 2),
                            VigenciaYear = tc.dfVigtarjeta.Substring(6),
                            cTitular = tc.cTitular,
                            cCodNegro = tc.cCodNegro,
                        };

                        d.DocumentosPago.Add(dp);
                    }
                }
                if (d.TarjetaDebito != null)
                {
                    foreach (var td in d.TarjetaDebito)
                    {
                        var dp = new DesglosePagoModel()
                        {
                            TipoPagoDesc = "TARJETA DE DEBITO",
                            ID = td.ID,
                            cTpoTarjeta = td.cTpoTarjeta,
                            cBancoDesc = td.cBancoDesc,
                            mMonto = td.mMonto,
                            mMontoSig = td.mMontoSig,
                            cNoTarjeta = td.cNoTarjeta,
                            dfVigtarjeta = td.dfVigtarjeta.Substring(3),
                            VigenciaMonth = td.dfVigtarjeta.Substring(3, 2),
                            VigenciaYear = td.dfVigtarjeta.Substring(6),
                            cTitular = td.cTitular,
                            cCodNegro = "",
                        };
                        d.DocumentosPago.Add(dp);
                    }
                }
                if (d.pagos != null)
                {
                    if (d.pagos.pagoOtro != null)
                    {
                        foreach (var op in d.pagos.pagoOtro)
                        {
                            var dp = new DesglosePagoModel()
                            {
                                TipoPagoDesc = op.nFormaPagoIDDesc,
                                ID = op.ID,
                                cTpoTarjeta = "",
                                cBancoDesc = "",
                                mMonto = op.mMonto,
                                mMontoSig = op.mMontoSig,
                                cNoTarjeta = "",
                                dfVigtarjeta = "",
                                VigenciaMonth = "",
                                VigenciaYear = "",
                                cTitular = "",
                                cCodNegro = "",
                            };
                            d.DocumentosPago.Add(dp);
                        }
                    }
                }
            }
            catch (Exception err)
            {
                Response.StatusCode = (int)(System.Net.HttpStatusCode.InternalServerError);
                LogDeErrores.RegistroErrorWeb(err, DatoUsuario.idAgente, DatoUsuario.nomAgente, DatoUsuario.sIp, DatoUsuario.sHostName);
                throw err;
            }
        }

        /// <summary>
        /// Método que devuelve la vista parcial del desglose de pagos
        /// </summary>
        /// <returns></returns>
        public ActionResult DetallePagoExpress()
        {
            try
            {
                // Recupero el modelo
                GenericoViewModel d = (GenericoViewModel)Session["ModeloEnUso"];
                var dp = new DesglosePagoModel();
                if (d != null)
                {
                    if (d.TarjetCredito != null)
                    {
                        if (d.TarjetCredito.Count > 0)
                        {
                            dp = CentralAgentesMvc.App_Start.CastObject.Cast<DesglosePagoModel>(d.TarjetCredito.First());
                            dp.dfVigtarjeta = dp.dfVigtarjeta.Substring(3);
                        }
                    }

                    // Coloco los montos
                    if (dp.mMonto == 0)
                    {
                        dp.mMonto = (d.ObjCalculo.mPago1de == 0 ? d.ObjCalculo.mTotal : d.ObjCalculo.mPago1de);
                        dp.mMontoSig = d.ObjCalculo.mPago2de;
                    }
                }
                dp.BancosSource = VarProcInterfazX.arrCatBancos;

                // Redirecciono
                return PartialView("_DatosPagoPartial", dp);
            }
            catch (Exception err)
            {
                Response.StatusCode = (int)(System.Net.HttpStatusCode.InternalServerError);
                LogDeErrores.RegistroErrorWeb(err, DatoUsuario.idAgente, DatoUsuario.nomAgente, DatoUsuario.sIp, DatoUsuario.sHostName);
                return Json("ErrInterno");
            }
        }

        /// <summary>
        /// Método que elimina un pago la lista
        /// </summary>
        /// <param name="pagoID">ID del pago</param>
        /// <returns></returns>
        public ActionResult EliminarPago(string tipoPago, int pagoID)
        {
            try
            {
                var msje = "NO";

                GenericoViewModel model = (GenericoViewModel)Session["ModeloEnUso"];

                switch (tipoPago.ToUpper())
                {
                    case "EFECTIVO":
                    case "DEPOSITO":
                        if (model.pagos.pagoOtro.FirstOrDefault(c => c.ID == pagoID) != null)
                        {
                            var remove = model.pagos.pagoOtro.FirstOrDefault(c => c.ID == pagoID);
                            model.pagos.pagoOtro.Remove(remove);
                            msje = "OK";
                        }
                        break;

                    case "TARJETA DE DEBITO":
                        if (model.TarjetaDebito.FirstOrDefault(c => c.ID == pagoID) != null)
                        {
                            var remove = model.TarjetaDebito.FirstOrDefault(c => c.ID == pagoID);
                            model.TarjetaDebito.Remove(remove);
                            msje = "OK";
                        }
                        break;

                    case "TARJETA DE CREDITO":
                        if (model.TarjetCredito.FirstOrDefault(c => c.ID == pagoID) != null)
                        {
                            var remove = model.TarjetCredito.FirstOrDefault(c => c.ID == pagoID);
                            model.TarjetCredito.Remove(remove);
                            msje = "OK";
                        }
                        break;
                }

                return Content(msje);
            }
            catch (Exception err)
            {
                Response.StatusCode = (int)(System.Net.HttpStatusCode.InternalServerError);
                LogDeErrores.RegistroErrorWeb(err, DatoUsuario.idAgente, DatoUsuario.nomAgente, DatoUsuario.sIp, DatoUsuario.sHostName);
                return Json("ErrInterno");
            }
        }

        private void SetDesgloseListProperties(ref DesglosePagoModel dp)
        {
            try
            {
                dp.BancosSource = VarProcInterfazX.arrCatBancos;
                dp.TipoPagoSource = VarProcInterfazX.arrTipoPago;

                dp.Years = new Dictionary<string, string>();
                for (int i = DateTime.Now.Year; i < DateTime.Now.Year + 11; i++)
                {
                    dp.Years.Add(i.ToString(), i.ToString());
                }

                dp.Months = new Dictionary<string, string>();
                for (int i = 1; i < 13; i++)
                {
                    dp.Months.Add(i.ToString().PadLeft(2, '0'), i.ToString().PadLeft(2, '0'));
                }

                // Prevenir valores requeridos
                if (dp.TipoPagoDesc.ToUpper() == "EFECTIVO" || dp.TipoPagoDesc.ToUpper() == "DEPOSITO")
                {
                    dp.cNoTarjeta = "11111";
                    dp.dfVigtarjeta = "01/1900";
                    dp.cCodNegro = "999";
                    dp.cTitular = "XXXX";
                }

                if (dp.TipoPagoDesc.ToUpper() == "TARJETA DE DEBITO")
                {
                    dp.cCodNegro = "999";
                }
            }
            catch (Exception err)
            {
                Response.StatusCode = (int)(System.Net.HttpStatusCode.InternalServerError);
                LogDeErrores.RegistroErrorWeb(err, DatoUsuario.idAgente, DatoUsuario.nomAgente, DatoUsuario.sIp, DatoUsuario.sHostName);
                throw err;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateDesglosePago(DesglosePagoModel dp)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { success = false, errores = GetElementsFromModelState(), validaciones = "" });
                }

                // Recupero el modelo
                var modelComplete = (GenericoViewModel)Session["ModeloEnUso"];

                switch (dp.TipoPagoDesc.ToUpper())
                {
                    case "EFECTIVO":
                    case "DEPOSITO":
                        {
                            // Agregar el pago a la colección
                            if (modelComplete.pagos == null) modelComplete.pagos = new ClsPago();
                            if (modelComplete.pagos.pagoOtro == null) modelComplete.pagos.pagoOtro = new List<ClsPagoOtro>();

                            if (modelComplete.pagos.pagoOtro.FirstOrDefault(t => t.ID == dp.ID) != null)
                            {
                                var remove = modelComplete.pagos.pagoOtro.FirstOrDefault(t => t.ID == dp.ID);
                                modelComplete.pagos.pagoOtro.Remove(remove);
                            }
                            var idT = VarProcInterfazX.arrTipoPago.First(p => p.strDesc == dp.TipoPagoDesc).strCve;
                            var idF = VarProcInterfazX.arrFormaPago.First(f => f.strStat == idT).strCve;
                            ClsPagoOtro pgO = new ClsPagoOtro()
                            {
                                indice = (modelComplete.pagos.pagoOtro.Count + 1),
                                nFormaPagoID = Convert.ToInt32(idF),
                                nFormaPagoIDDesc = dp.TipoPagoDesc,
                                Accion = "A",
                                mMonto = dp.mMonto,
                                mMontoSig = dp.mMontoSig,
                                dfAlta = DateTime.Now.ToString("dd/MM/yyyy"),
                            };
                            modelComplete.pagos.pagoOtro.Add(pgO);
                        }
                        break;

                    case "TARJETA DE DEBITO":
                        {
                            // Agregar Tarjeta de Debito a la Colección
                            if (modelComplete.TarjetaDebito == null) modelComplete.TarjetaDebito = new List<ClsTarjetaDebito>();

                            if (modelComplete.TarjetaDebito.FirstOrDefault(t => t.ID == dp.ID) != null)
                            {
                                var remove = modelComplete.TarjetaDebito.FirstOrDefault(t => t.ID == dp.ID);
                                modelComplete.TarjetaDebito.Remove(remove);
                            }

                            var bncDesc = VarProcInterfazX.arrCatBancos.FirstOrDefault(b => b.strClave == dp.cBanco);
                            var desglose = CentralAgentesMvc.App_Start.CastObject.Cast<ClsTarjetaDebito>(dp);
                            desglose.indice = (modelComplete.TarjetaDebito.Count + 1);
                            desglose.cTitular = desglose.cTitular.ToUpper();
                            desglose.cBancoDesc = (bncDesc != null ? bncDesc.strDescrip : "");
                            desglose.dfVigtarjeta = DateTime.Parse(desglose.dfVigtarjeta).ToString("dd/MM/yyyy");
                            desglose.dfAlta = DateTime.Now.ToString("dd/MM/yyyy");
                            modelComplete.TarjetaDebito.Add(desglose);
                        }
                        break;

                    case "TARJETA DE CREDITO":
                        {
                            // Agregar Tarjeta de Credito a la Colección
                            if (modelComplete.TarjetCredito == null) modelComplete.TarjetCredito = new List<ClsTarjetaCredito>();

                            if (modelComplete.TarjetCredito.FirstOrDefault(t => t.ID == dp.ID) != null)
                            {
                                var remove = modelComplete.TarjetCredito.FirstOrDefault(t => t.ID == dp.ID);
                                modelComplete.TarjetCredito.Remove(remove);
                            }

                            var bncDesc = VarProcInterfazX.arrCatBancos.FirstOrDefault(b => b.strClave == dp.cBanco);
                            var desglose = CentralAgentesMvc.App_Start.CastObject.Cast<ClsTarjetaCredito>(dp);
                            desglose.indice = (modelComplete.TarjetCredito.Count + 1);
                            desglose.cTitular = desglose.cTitular.ToUpper();
                            desglose.cBancoDesc = (bncDesc != null ? bncDesc.strDescrip : "");
                            desglose.dfVigtarjeta = DateTime.Parse(desglose.dfVigtarjeta).ToString("dd/MM/yyyy");
                            desglose.dfAlta = DateTime.Now.ToString("dd/MM/yyyy");
                            desglose.mMonto = dp.mMonto;
                            desglose.mMontoSig = dp.mMontoSig;
                            modelComplete.TarjetCredito.Add(desglose);
                        }
                        break;
                }


                // Guardo el modelo
                GuardaEnMemoria(modelComplete);
                ClearErrors();

                //Redireccionar Vista
                SetDesgloseListProperties(ref dp);
                return View("DesglosePago", dp);
            }
            catch (Exception err)
            {
                Response.StatusCode = (int)(System.Net.HttpStatusCode.InternalServerError);
                LogDeErrores.RegistroErrorWeb(err, DatoUsuario.idAgente, DatoUsuario.nomAgente, DatoUsuario.sIp, DatoUsuario.sHostName);
                return Json("ErrInterno");
            }
        }


        public JsonResult ValSeguroObli(int idVehi, double coberRepCiv)
        {
            try
            {// Recupero el modelo
                GenericoViewModel modelComplete = (GenericoViewModel)Session["ModeloEnUso"];
                ClsCotizacion cot = CentralAgentesMvc.App_Start.CastObject.Cast<ClsCotizacion>(modelComplete);
                if (modelComplete.ModulodeTrabajo == "Cotizaciones")
                {
                    ClsVehiculo vhclo = cot.vehiculos.FirstOrDefault(c => c.nNumVehi == idVehi);
                    vhclo.coberRepCiv =coberRepCiv;
                    ManejoInformacion MIObj = new ManejoInformacion();
                    MIObj.ValSegObli(cot, "C", VarProcAMC.strPeriodo);
                }
                return Json(cot.PolObligatoria, JsonRequestBehavior.AllowGet);
            }
            catch (Exception err)
            {
                Response.StatusCode = (int)(System.Net.HttpStatusCode.InternalServerError);
                LogDeErrores.RegistroErrorWeb(err, DatoUsuario.idAgente, DatoUsuario.nomAgente, DatoUsuario.sIp, DatoUsuario.sHostName);
                return Json("ErrInterno");
            }
        }

        public JsonResult MontoSegObli(string coberRepCiv)
        {
            // Recupero el modelo
            string polObligatoria = "N";
            GenericoViewModel modelComplete = (GenericoViewModel)TempData.Peek("ModeloEnUso");
            ManejoInformacion MIObj = new ManejoInformacion();
            if (coberRepCiv == MIObj.MontoSegObli(VarProcAMC.strPeriodo).ToString())
            {
                MIObj.MontoSegObli(VarProcAMC.strPeriodo);
                polObligatoria = "S";
            }
            else
            {
                polObligatoria = "N";
            }
            return Json(polObligatoria, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region <-- Prospectos Methods -->
        public ActionResult ProspectoEnDocumento(string id, string modulo, int prospectID)
        {
            try
            {
                // Busco info del documento
                GenericoViewModel modelComplete = (GenericoViewModel)Session["ModeloEnUso"];
                var prospect = (modelComplete.prospectos != null ? modelComplete.prospectos.FirstOrDefault(c => c.nconsePros == prospectID) : null);
                if (prospect == null)
                {
                    prospect = new ClsProspecto();
                    prospect.TerminoVigencia = DateTime.Now;
                }

                // Redirecciono
                return PartialView("Prospectos", prospect);
            }
            catch (Exception err)
            {
                Response.StatusCode = (int)(System.Net.HttpStatusCode.InternalServerError);
                LogDeErrores.RegistroErrorWeb(err, DatoUsuario.idAgente, DatoUsuario.nomAgente, DatoUsuario.sIp, DatoUsuario.sHostName);
                return Json("ErrInterno");
            }
        }

        /// <summary>
        /// Método usado para refrescar los datos de Prospectos en el View
        /// </summary>
        /// <returns></returns>
        public JsonResult RefreshProspects()
        {
            try
            {
                // Recupero el modelo
                GenericoViewModel d = (GenericoViewModel)Session["ModeloEnUso"];

                return Json(d.prospectos, JsonRequestBehavior.AllowGet);
            }
            catch (Exception err)
            {
                Response.StatusCode = (int)(System.Net.HttpStatusCode.InternalServerError);
                LogDeErrores.RegistroErrorWeb(err, DatoUsuario.idAgente, DatoUsuario.nomAgente, DatoUsuario.sIp, DatoUsuario.sHostName);
                return Json("ErrInterno");
            }
        }

        /// <summary>
        /// Método que elimina un prospecto de la lista
        /// </summary>
        /// <param name="prospectID">ID del prospecto</param>
        /// <returns></returns>
        public ActionResult EliminarProspecto(int prospectID)
        {
            try
            {
                var msje = "NO";

                GenericoViewModel model = (GenericoViewModel)Session["ModeloEnUso"];
                if (model.prospectos.FirstOrDefault(c => c.nconsePros == prospectID) != null)
                {
                    var remove = model.prospectos.FirstOrDefault(c => c.nconsePros == prospectID);
                    model.prospectos.Remove(remove);
                    msje = "OK";
                }

                return Content(msje);
            }
            catch (Exception err)
            {
                Response.StatusCode = (int)(System.Net.HttpStatusCode.InternalServerError);
                LogDeErrores.RegistroErrorWeb(err, DatoUsuario.idAgente, DatoUsuario.nomAgente, DatoUsuario.sIp, DatoUsuario.sHostName);
                return Json("ErrInterno");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateProspecto(ClsProspecto prospect)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { success = false, errores = GetElementsFromModelState(), validaciones = "" });
                }

                // Recupero el modelo
                GenericoViewModel modelComplete = (GenericoViewModel)Session["ModeloEnUso"];

                // Agrego el prospecto a la coleccion
                if (modelComplete.prospectos == null) modelComplete.prospectos = new List<ClsProspecto>();
                if (modelComplete.prospectos.FirstOrDefault(c => c.nconsePros == prospect.nconsePros) != null)
                {
                    var remove = modelComplete.prospectos.FirstOrDefault(c => c.nconsePros == prospect.nconsePros);
                    modelComplete.prospectos.Remove(remove);
                }

                prospect.nconsePros = modelComplete.prospectos.Count + 1;
                modelComplete.prospectos.Add(prospect);

                // Guardo el modelo
                GuardaEnMemoria(modelComplete);
                ClearErrors();

                // Redirecciono
                return PartialView("Prospectos", prospect);
            }
            catch (Exception err)
            {
                Response.StatusCode = (int)(System.Net.HttpStatusCode.InternalServerError);
                LogDeErrores.RegistroErrorWeb(err, DatoUsuario.idAgente, DatoUsuario.nomAgente, DatoUsuario.sIp, DatoUsuario.sHostName);
                return Json("ErrInterno");
            }
        }
        #endregion

        #region <-- Complementos Methods -->
        public ActionResult ComplementoEnDocumento(string id, string modulo)
        {
            try
            {
                // Busco info del documento
                GenericoViewModel modelComplete = (GenericoViewModel)Session["ModeloEnUso"];
                var complement = CentralAgentesMvc.App_Start.CastObject.Cast<ComplementoModel>(modelComplete.complemento);
                if (complement == null)
                {
                    complement = new ComplementoModel();
                }
                complement.PersonaQueRecomienda = modelComplete.recomienda;
                complement.Estatus = modelComplete.estatus;
                complement.Responsable = modelComplete.responsable;

                // Seteo las colecciones
                SetComplementListProperties(ref complement);

                // Redirecciono
                return PartialView("Complementos", complement);
            }
            catch (Exception err)
            {
                Response.StatusCode = (int)(System.Net.HttpStatusCode.InternalServerError);
                LogDeErrores.RegistroErrorWeb(err, DatoUsuario.idAgente, DatoUsuario.nomAgente, DatoUsuario.sIp, DatoUsuario.sHostName);
                return Json("ErrInterno");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateComplemento(ComplementoModel complement)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { success = false, errores = GetElementsFromModelState(), validaciones = "" });
                }

                // Recupero el modelo
                GenericoViewModel modelComplete = (GenericoViewModel)Session["ModeloEnUso"];

                modelComplete.complemento = CentralAgentesMvc.App_Start.CastObject.Cast<ClsComplemento>(complement);
                modelComplete.complemento.utimasObserv = (complement.utimasObserv != null ? complement.utimasObserv.ToUpper() : "");
                modelComplete.recomienda = (complement.PersonaQueRecomienda != null ? complement.PersonaQueRecomienda.ToUpper() : "");

                if (modelComplete.responsable != null)
                    modelComplete.complemento.EjecutivoResponsable = VarProcInterfazX.arrResponsables.First(e => e.strCve == complement.Responsable).strDesc;

                if (modelComplete.estatus != null)
                    modelComplete.complemento.DescripcionEstatus = VarProcInterfazX.arrEstCot.First(e => e.strCve == modelComplete.estatus).strDesc;

                SetComplementListProperties(ref complement);

                // Guardo el modelo
                GuardaEnMemoria(modelComplete);
                ClearErrors();

                // Redirecciono
                return RedirectToAction("RefreshComplement");
            }
            catch (Exception err)
            {
                Response.StatusCode = (int)(System.Net.HttpStatusCode.InternalServerError);
                LogDeErrores.RegistroErrorWeb(err, DatoUsuario.idAgente, DatoUsuario.nomAgente, DatoUsuario.sIp, DatoUsuario.sHostName);
                return Json("ErrInterno");
            }
        }

        public ActionResult RefreshComplement()
        {
            try
            {
                GenericoViewModel d = (GenericoViewModel)Session["ModeloEnUso"];

                return PartialView("_ComplementPartial", d);
            }
            catch (Exception err)
            {
                Response.StatusCode = (int)(System.Net.HttpStatusCode.InternalServerError);
                LogDeErrores.RegistroErrorWeb(err, DatoUsuario.idAgente, DatoUsuario.nomAgente, DatoUsuario.sIp, DatoUsuario.sHostName);
                return Json("ErrInterno");
            }
        }

        /// <summary>
        /// Método que llena las listas de los dropdown
        /// </summary>
        /// <param name="model">Model en ejecución</param>
        private void SetComplementListProperties(ref ComplementoModel complement)
        {
            try
            {
                complement.CompañiaAnteriorSource = VarProcInterfazX.arrCiasAnt;
                complement.InteresesSource = VarProcInterfazX.arrInteres;
                complement.PublicidadSource = VarProcInterfazX.arrPublicidad;
                complement.ResponsableSource = VarProcInterfazX.arrResponsables;
                complement.EstatusSource = VarProcInterfazX.arrEstCot;
            }
            catch (Exception err)
            {
                Response.StatusCode = (int)(System.Net.HttpStatusCode.InternalServerError);
                LogDeErrores.RegistroErrorWeb(err, DatoUsuario.idAgente, DatoUsuario.nomAgente, DatoUsuario.sIp, DatoUsuario.sHostName);
                throw err;
            }
        }
        #endregion

        #region <-- Seguimiento Methods -->
        public ActionResult SeguimientoEnDocumento(string id, string modulo)
        {
            try
            {
                // Busco info del documento
                GenericoViewModel modelComplete = (GenericoViewModel)Session["ModeloEnUso"];
                var tracking = modelComplete.SeguimientoPol;
                if (tracking == null)
                {
                    tracking = new ClsSeguimientoPol();
                }

                // Datos heredados de póliza
                tracking.titulo = modelComplete.titulo;
                tracking.codigoPostal = modelComplete.codigoPostal;
                tracking.apellidoPaterno = modelComplete.apellidoPaterno;
                tracking.apellidoMaterno = modelComplete.apellidoMaterno;
                tracking.nombres = modelComplete.nombres;
                tracking.campaña = modelComplete.campaña;
                ViewBag.EstatusTrackSource = VarProcInterfazX.arrEstSeg;
                tracking.numConductores = modelComplete.numConductores;
                tracking.numVehiculos = modelComplete.numVehiculos;
                tracking.FUltLlamada = DateTime.Parse(tracking.FUltLlamada).ToString("dd/MM/yyy");
                tracking.FProxLlamada = DateTime.Parse(tracking.FProxLlamada).ToString("dd/MM/yyy");
                // Redirecciono
                return PartialView("Seguimiento", tracking);
            }
            catch (Exception err)
            {
                Response.StatusCode = (int)(System.Net.HttpStatusCode.InternalServerError);
                LogDeErrores.RegistroErrorWeb(err, DatoUsuario.idAgente, DatoUsuario.nomAgente, DatoUsuario.sIp, DatoUsuario.sHostName);
                return Json("ErrInterno");
            }
        }

        public ActionResult RefreshTracking()
        {
            try
            {
                GenericoViewModel d = (GenericoViewModel)Session["ModeloEnUso"];

                return PartialView("_TrackingPartial", d);
            }
            catch (Exception err)
            {
                Response.StatusCode = (int)(System.Net.HttpStatusCode.InternalServerError);
                LogDeErrores.RegistroErrorWeb(err, DatoUsuario.idAgente, DatoUsuario.nomAgente, DatoUsuario.sIp, DatoUsuario.sHostName);
                return Json("ErrInterno");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateSeguimiento(ClsSeguimientoPol tracking)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { success = false, errores = GetElementsFromModelState(), validaciones = "" });
                }

                // Recupero el modelo
                GenericoViewModel modelComplete = (GenericoViewModel)Session["ModeloEnUso"];
                modelComplete.SeguimientoPol = tracking;
                ViewBag.EstatusTrackSource = VarProcInterfazX.arrEstSeg;

                // Guardo el modelo
                GuardaEnMemoria(modelComplete);
                ClearErrors();

                // Redirecciono
                return RedirectToAction("RefreshTracking");
            }
            catch (Exception err)
            {
                Response.StatusCode = (int)(System.Net.HttpStatusCode.InternalServerError);
                LogDeErrores.RegistroErrorWeb(err, DatoUsuario.idAgente, DatoUsuario.nomAgente, DatoUsuario.sIp, DatoUsuario.sHostName);
                return Json("ErrInterno");
            }
        }
        #endregion

        #region <-- Busqueda de Vehiculos -->
        /// <summary>
        /// Metodo usado para buscar el modelo basado en el ID
        /// </summary>
        /// <param name="id">Id del modelo</param>
        /// <returns>Descripcion del modelo</returns>
        public ActionResult SearchModelo(long id)
        {
            try
            {
                var descMd = "";
                var modelo = VarProcInterfazX.arrModelos.FirstOrDefault(veh => veh.lngClave == id);
                if (modelo != null)
                {
                    descMd = modelo.strModelo;
                }

                return Content(descMd);
            }
            catch (Exception err)
            {
                Response.StatusCode = (int)(System.Net.HttpStatusCode.InternalServerError);
                LogDeErrores.RegistroErrorWeb(err, DatoUsuario.idAgente, DatoUsuario.nomAgente, DatoUsuario.sIp, DatoUsuario.sHostName);
                return Json("ErrInterno");
            }
        }

        /// <summary>
        /// Metodo usado para levantar la ventana de busqueda de modelos
        /// </summary>
        /// <param name="ctrlModelID">Control ID a refrescar</param>
        /// <param name="ctrlDescModel">Control DEscripcion a refrescar</param>
        /// <param name="ctrlSpecific">Control modelo especifico a refrescar</param>
        /// <returns></returns>
        public ActionResult SearchCar(string ctrlModelID, string ctrlDescModel, string ctrlSpecific)
        {
            try
            {
                ctzC.ControllerContext = this.ControllerContext;
                SearchViewModel sm = new SearchViewModel();
                sm.CtrlModelID = ctrlModelID;
                sm.CtrlDescripcionModel = ctrlDescModel;
                sm.CtrlSpecificModel = ctrlSpecific;

                var marcas = (SelectList)ctzC.ObtenMarcaVehi("R", "").Data;
                sm.MarcasSource = marcas;

                var recno = marcas.FirstOrDefault(m => m.Text.Trim() == "TODAS");
                if (recno != null) sm.MarcaID = Convert.ToInt32(recno.Value);

                // Redirecciono
                return PartialView("SearchCar", sm);
            }
            catch (Exception err)
            {
                Response.StatusCode = (int)(System.Net.HttpStatusCode.InternalServerError);
                LogDeErrores.RegistroErrorWeb(err, DatoUsuario.idAgente, DatoUsuario.nomAgente, DatoUsuario.sIp, DatoUsuario.sHostName);
                return Json("ErrInterno");
            }
        }

        /// <summary>
        /// Método que retorna la lista de vehículos basado en los campos de consulta
        /// </summary>
        /// <param name="searchTerm">Valor escrito por el usuario</param>
        /// <param name="year">Año del vehículo</param>
        /// <param name="marcaID">ID de la marca</param>
        /// <param name="subMarcaID">ID SubMarca</param>
        /// <param name="pageIndex">Página de datos</param>
        /// <returns>Objeto Json</returns>
        public JsonResult GetVehiculos(string searchTerm, string modelo, string marcaDsc, string subMarkDsc, int pageIndex)
        {
            try
            {
                ManejoInformacion mi = new ManejoInformacion();
                int recordCount = 0;
                marcaDsc = (marcaDsc == null ? "" : marcaDsc.Trim().ToUpper());
                subMarkDsc = (subMarkDsc == null ? "" : subMarkDsc.Trim().ToUpper());
                modelo = (modelo == null ? "" : modelo.Trim().ToUpper());
                searchTerm = (searchTerm == null ? "" : searchTerm.Trim().ToUpper());

                var data = mi.FiltrarWeb(out recordCount, VarProcAMC.strPeriodo, modelo, marcaDsc, subMarkDsc, searchTerm, "", pageIndex);
                return Json(from g in data.Select()
                            select new
                            {
                                Clave = g["Clave"],
                                Descripcion = g["Descripción"],
                                Marca = g["Marca"],
                                SubMarca = g["SubMarca"],
                                total = recordCount,
                            }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception err)
            {
                Response.StatusCode = (int)(System.Net.HttpStatusCode.InternalServerError);
                LogDeErrores.RegistroErrorWeb(err, DatoUsuario.idAgente, DatoUsuario.nomAgente, DatoUsuario.sIp, DatoUsuario.sHostName);
                return Json("ErrInterno");
            }
        }

        /// <summary>
        /// Método que retorna la lista de años para un modelo
        /// </summary>
        /// <param name="modeloID">Id del modelo</param>
        /// <returns>Objeto Json - Lista</returns>
        public JsonResult GetModelosDeMarca(string modeloID)
        {
            try
            {
                ManejoInformacion mi = new ManejoInformacion();
                var lista = mi.ListaModelos(Convert.ToInt64(modeloID));
                return Json(lista, JsonRequestBehavior.AllowGet);
            }
            catch (Exception err)
            {
                Response.StatusCode = (int)(System.Net.HttpStatusCode.InternalServerError);
                LogDeErrores.RegistroErrorWeb(err, DatoUsuario.idAgente, DatoUsuario.nomAgente, DatoUsuario.sIp, DatoUsuario.sHostName);
                return Json("ErrInterno");
            }
        }

        /// <summary>
        /// Método que devuelve los años registrados para un modelo de carro
        /// </summary>
        /// <param name="modeloID">ID del modelo</param>
        /// <returns>Lista de años</returns>
        public JsonResult GetYearsForModel(string modeloID)
        {
            try
            {
                Dictionary<string, string> years = new Dictionary<string, string>();
                List<TypModAño> modeloAños = VarProcInterfazX.arrModAño.Where(años => años.lngModelo == Convert.ToInt64(modeloID))
                                                                       .OrderByDescending(y => y.intAño)
                                                                       .ToList();
                if (modeloID == "999")
                {
                    years = (Dictionary<string, string>)ctzC.ObtenAños("R").Data;
                }
                else
                {
                    foreach (var item in modeloAños)
                    {
                        years.Add(string.Format("{0}", item.intAño), string.Format("{0}", item.intAño));
                    }
                }
                return Json(years, JsonRequestBehavior.AllowGet);
            }
            catch (Exception err)
            {
                Response.StatusCode = (int)(System.Net.HttpStatusCode.InternalServerError);
                LogDeErrores.RegistroErrorWeb(err, DatoUsuario.idAgente, DatoUsuario.nomAgente, DatoUsuario.sIp, DatoUsuario.sHostName);
                return Json("ErrInterno");
            }
        }
        #endregion

        #region <-- Métodos Privados -->
        private void GuardaEnMemoria(ClsCotizacion ctz)
        {
            try
            {
                var gm = CentralAgentesMvc.App_Start.CastObject.Cast<GenericoViewModel>(ctz);
                GuardaEnMemoria(gm, ctz);
            }
            catch (Exception err)
            {
                Response.StatusCode = (int)(System.Net.HttpStatusCode.InternalServerError);
                LogDeErrores.RegistroErrorWeb(err, DatoUsuario.idAgente, DatoUsuario.nomAgente, DatoUsuario.sIp, DatoUsuario.sHostName);
                throw err;
            }
        }
        private void GuardaEnMemoria(GenericoViewModel gm)
        {
            try
            {
                if (gm.ModulodeTrabajo == "Cotizaciones")
                {
                    var ctz = CentralAgentesMvc.App_Start.CastObject.Cast<ClsCotizacion>(gm);
                    GuardaEnMemoria(gm, ctz);
                }
                else
                {
                    var rnv = CentralAgentesMvc.App_Start.CastObject.Cast<ClsRenovaciones>(gm);

                    // Guardo el modelo
                    Session["ModeloEnUso"] = gm;

                }
            }
            catch (Exception err)
            {
                Response.StatusCode = (int)(System.Net.HttpStatusCode.InternalServerError);
                LogDeErrores.RegistroErrorWeb(err, DatoUsuario.idAgente, DatoUsuario.nomAgente, DatoUsuario.sIp, DatoUsuario.sHostName);
                throw err;
            }
        }
        private void GuardaEnMemoria(GenericoViewModel gm, ClsCotizacion ctz)
        {
            try
            {
                // Guardo el modelo
                Session["ModeloEnUso"] = gm;


                ctz.calculos = new List<ClsCalculo>();
                Session["objACotizar"] = ctz;
            }
            catch (Exception err)
            {
                Response.StatusCode = (int)(System.Net.HttpStatusCode.InternalServerError);
                LogDeErrores.RegistroErrorWeb(err, DatoUsuario.idAgente, DatoUsuario.nomAgente, DatoUsuario.sIp, DatoUsuario.sHostName);
                throw err;
            }
        }

        private GenericoViewModel GetDataObject(string id, string modulo)
        {
            try
            {
                var logged = ((DataSet)Session["UserObj"]).Tables["catAgentes"].Rows[0]["nAgenteID"].ToString();
                GenericoViewModel model = new GenericoViewModel();

                switch (modulo.ToLower())
                {
                    case "cotizaciones":
                        {
                            #region //regresar todo action como estaba
                            string usuarioEnCotizacion = "";
                            ClsCotizacion cotizacion = ClsCotizacion.CargaCotiza(ref usuarioEnCotizacion, id, VarProcAMC.strPeriodo, logged, "Si");

                            if (logged != usuarioEnCotizacion)
                            {
                                Logear loginUsu = new Logear();
                                var nombreUsuario = loginUsu.NombrePosesionCotizacion(usuarioEnCotizacion);
                            }
                            #endregion

                            ViewBag.Title = "Detalle Cotización";
                            ViewBag.SubTitle = "cotización";
                            model = CentralAgentesMvc.App_Start.CastObject.Cast<GenericoViewModel>(cotizacion);
                            if (!string.IsNullOrEmpty(model.nSolicitud))
                            {
                                model.dataExtraPoliza = CentralAgentesMvc.App_Start.CastObject.Cast<CentralAgentesMvc.Models.DataExtraPoliza>(cotizacion.getExtraCotiza(long.Parse(model.nSolicitud),1));
                            }
                            else {
                                model.dataExtraPoliza = new Models.DataExtraPoliza();
                            }
                        }
                        break;

                    case "renovaciones":
                        {
                            ViewBag.Title = "Detalle Renovación";
                            ViewBag.SubTitle = "renovación";
                            ClsRenovaciones renovacion = ClsRenovaciones.CargaRenovacion(Convert.ToInt64(id), 0, logged, "");
                            //renovacion.SeguimientoPol.Hora = renovacion.SeguimientoPol.Hora.Substring(11);
                            ClsCotizacion cotizacion = new ClsCotizacion();
                            if (renovacion.complemento == null) renovacion.complemento = new ClsComplemento();

                            model = CentralAgentesMvc.App_Start.CastObject.Cast<GenericoViewModel>(renovacion);
                            model.dataExtraPoliza = CentralAgentesMvc.App_Start.CastObject.Cast<CentralAgentesMvc.Models.DataExtraPoliza>(cotizacion.getExtraCotiza(renovacion.solicitudID,2));
                        }
                        break;
                }

                return model;
            }
            catch (Exception err)
            {
                Response.StatusCode = (int)(System.Net.HttpStatusCode.InternalServerError);
                LogDeErrores.RegistroErrorWeb(err, DatoUsuario.idAgente, DatoUsuario.nomAgente, DatoUsuario.sIp, DatoUsuario.sHostName);
                throw err;
            }
        }

        private Dictionary<string, object> GetElementsFromModelState()
        {
            try
            {
                var errors = new Dictionary<string, object>();
                foreach (var key in ModelState.Keys)
                {
                    // Only send the errors to the client.
                    if (ModelState[key].Errors.Count > 0)
                    {
                        errors[key] = ModelState[key].Errors;
                    }
                    else
                        errors[key] = "";
                }

                return errors;
            }
            catch (Exception err)
            {
                Response.StatusCode = (int)(System.Net.HttpStatusCode.InternalServerError);
                LogDeErrores.RegistroErrorWeb(err, DatoUsuario.idAgente, DatoUsuario.nomAgente, DatoUsuario.sIp, DatoUsuario.sHostName);
                throw err;
            }
        }

        private void ClearErrors()
        {
            foreach (var key in ModelState.Keys)
            {
                ModelState[key].Errors.Clear();
            }
            foreach (var modelValue in ModelState.Values)
            {
                modelValue.Errors.Clear();
            }
        }
        #endregion

        #region  <-- Métodos srpago -->
        public JsonResult AplicaSrPago(string Referencia, string importe, string tipoTarjeta, string Nombre, string noTarjeta, string CodigSeg, string mes, string anio)
        {
            try
            {
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                try
                {
                    var modelComplete = (GenericoViewModel)Session["ModeloEnUso"];

                    switch (modelComplete.ModulodeTrabajo)
                    {
                        case "Cotizaciones":
                            ClsCotizacion cot = CentralAgentesMvc.App_Start.CastObject.Cast<ClsCotizacion>(modelComplete);
                            cot.ramo = 1;
                            cot.subRamo = "1";
                    resultado = modelComplete.AplicaSrPagoWeb("C", cot, double.Parse(importe), tipoTarjeta, Nombre, noTarjeta, CodigSeg, mes, anio);
                            break;
                        case "Renovaciones":
                            ClsRenovaciones ren = CentralAgentesMvc.App_Start.CastObject.Cast<ClsRenovaciones>(modelComplete);
                            if (ren.TarjetaDebito.Count != 0)
                            {
                                foreach(var td in ren.TarjetaDebito)
                                {
                                    td.cusuario=DatoUsuario.idAgente;
                                }
                            }
                            if (ren.TarjetCredito.Count != 0)
                            {
                                foreach (var tc in ren.TarjetCredito)
                                {
                                    tc.cusuario = DatoUsuario.idAgente;
                                }
                            }

                            ren.ramo = 1;
                            ren.subRamo = "1";
                            resultado = modelComplete.AplicaSrPagoWeb("R", ren, double.Parse(importe), tipoTarjeta, Nombre, noTarjeta, CodigSeg, mes, anio);
                            break;
                    }
                    //resultado = modelComplete.AplicaSrPagoWeb("C", cot, 2, tipoTarjeta, Nombre, noTarjeta, CodigSeg, mes, anio);
                }
                catch (Exception err)
                {
                    LogDeErrores.RegistroErrorWeb(err, DatoUsuario.idAgente, DatoUsuario.nomAgente, DatoUsuario.sIp, DatoUsuario.sHostName);
                }
                return Json(resultado, JsonRequestBehavior.AllowGet);
            }
            catch (Exception err)
            {
                Response.StatusCode = (int)(System.Net.HttpStatusCode.InternalServerError);
                LogDeErrores.RegistroErrorWeb(err, DatoUsuario.idAgente, DatoUsuario.nomAgente, DatoUsuario.sIp, DatoUsuario.sHostName);
                return Json("ErrInterno");
            }
        }
        #endregion

        #region  <-- Métodos Liberacion de cotizacion -->
        public JsonResult LiberarCot()
        {
            GenericoViewModel d = (GenericoViewModel)Session["ModeloEnUso"];
            try
            {
                // Recupero el modelo
                if (d.ModulodeTrabajo == "Cotizaciones")
                {
                    //var logged = (DataSet)Session["UserObj"];
                    //var agentID = logged.Tables["catAgentes"].Rows[0]["nAgenteID"].ToString();
                    ManejoInformacion UpdateUsuEnCotiza = new ManejoInformacion();
                    string strUsuEnCotizacion = UpdateUsuEnCotiza.RecuperaUsuCotiza(d.cotizacionID);
                    if (DatoUsuario.idAgente == strUsuEnCotizacion)
                    {
                        UpdateUsuEnCotiza.UpDateCotiUsu(d.cotizacionID);
                    }
                }
            }
            catch (Exception err)
            {
                LogDeErrores.RegistroErrorWeb(err, DatoUsuario.idAgente, DatoUsuario.nomAgente, DatoUsuario.sIp, DatoUsuario.sHostName);
            }

            return Json(d.cotizacionID, JsonRequestBehavior.AllowGet);
        }
        #endregion


    }
}