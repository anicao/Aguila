using System;
using System.Linq;
using System.Data;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using System.Collections.Generic;
//==================================
using Negocio.HerramientasVarios;
using Negocio.AccesosYPermisos;
using Negocio.ClasesCentral;
using Negocio.Consultas;
//==================================
using CentralAgentesMvc.Models;
//using CaptchaMvc.Infrastructure;
//using CaptchaMvc.Attributes;
using System.IO;
using Negocio.ReportesCrystal;


namespace CentralAgentesMvc.Controllers
{
    [Authorize]
    public class CotizacionController : Controller
    {
        #region <-- Private Properties -->
        private CmbCatalogos ctlg = new CmbCatalogos();
        private ManejoInformacion MnInf = new ManejoInformacion();
        private Cotizaciones ctzc = new Cotizaciones();
        private ClsRenovaciones rnvc = new ClsRenovaciones();
        private ValidaVehiculos VldVeh = new ValidaVehiculos();
        private VarProcAMC VarProcAM = new VarProcAMC();
        #endregion

        #region <-- Action Methods -->
        /// <summary>
        /// GENERACION DE CAPTCHA
        /// </summary>
        /// <param name="random"></param>
        /// <returns></returns>
        /// 
        [Authorize]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public CaptchaImageResult ShowCaptchaImage(string random)
        {
            return new CaptchaImageResult();
        }

        // GET: /Cotizacion/Index
        /// <summary>
        /// Método de entrada a las cotizaciones. Devuelve una lista de las cotizaciones del agente conectado
        /// </summary>
        /// <returns></returns>
        /// 
        [Authorize]
        public ActionResult Index(string modulo)
        {
            try
            {
                ViewBag.Modulo = modulo;
                ViewBag.Title = (modulo == "Cotizaciones" ? "Mis Cotizaciones" : "Mis Renovaciones");
                var logged = (DataSet)Session["UserObj"];

                var rpt = new ReportViewModel();
                rpt.AgenteID = logged.Tables["catAgentes"].Rows[0]["nAgenteID"].ToString();
                rpt.AgenteName = logged.Tables["catAgentes"].Rows[0].ItemArray[5].ToString();
                rpt.AgentesSource = VarProcInterfazX.arrAgentes;
                rpt.PeriodosSource = ctlg.CargaAnosCotizacionWeb();
                rpt.PeriodosMesSource = VarProcInterfazX.arrMeses;

                return View(rpt);
            }
            catch (Exception err)
            {
                Response.StatusCode = (int)(System.Net.HttpStatusCode.InternalServerError);
                LogDeErrores.RegistroErrorWeb(err, DatoUsuario.idAgente, DatoUsuario.nomAgente, DatoUsuario.sIp, DatoUsuario.sHostName);
                return Json("ErrInterno");
            }
        }
        [Authorize]
        public ActionResult Pagar()
        {
            try
            {
                var paga = new PagarModel();
                return PartialView(paga);
            }
            catch (Exception err)
            {
                Response.StatusCode = (int)(System.Net.HttpStatusCode.InternalServerError);
                LogDeErrores.RegistroErrorWeb(err, DatoUsuario.idAgente, DatoUsuario.nomAgente, DatoUsuario.sIp, DatoUsuario.sHostName);
                return Json("ErrInterno");
            }
        }

        /// <summary>
        /// Método usado para levantar la ventana cotizacion express
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Vista de Cotizacion express</returns>
        [Authorize]
        public ActionResult CotExpress(string Tipo)
        {
            try
            {
                var CtExp = NuevaCotizacionExpress();
                return View(CtExp);
            }
            catch (Exception err)
            {
                Response.StatusCode = (int)(System.Net.HttpStatusCode.InternalServerError);
                LogDeErrores.RegistroErrorWeb(err, DatoUsuario.idAgente, DatoUsuario.nomAgente, DatoUsuario.sIp, DatoUsuario.sHostName);
                return Json("ErrInterno");
            }
        }
       
        public ActionResult CotizadorEnLinea(string nombre, string paterno, string materno, string email, string telefono, string medio)
        {
            try
            {

                if (!HttpContext.User.Identity.IsAuthenticated || Session["UserObj"]==null)
                {
                    Logear acceso = new Logear();
                    var user = acceso.LoginAgenteWeb(4, "a");
                    string name = user.Tables["catAgentes"].Rows[0]["cNombre"].ToString();
                    VarProcAMC VarProcAMC = new VarProcAMC();
                    VarProcAMC.IniciarCarga();
                    Session["UserObj"] = user;
                    Session["Periodo"] = "Periodo: " + VarProcAMC.strPeriodo;
                    Session["UserRol"] = "";

                    System.Web.Security.FormsAuthentication.SetAuthCookie(name, false);
                }

                var CtExp = NuevaCotizacionExpress();
                CtExp.cotizacion.nombres = nombre;
                CtExp.cotizacion.apellidoPaterno = paterno;
                CtExp.cotizacion.apellidoMaterno = materno;
                CtExp.cotizacion.email = email;
                CtExp.cotizacion.codigoNegro = medio.Length > 4 ? medio.Remove(4) : medio;//medios
                CtExp.Telefonos.cTel = telefono;

                return View("CotExpress", CtExp);
            }
            catch (Exception err)
            {
                Response.StatusCode = (int)(System.Net.HttpStatusCode.InternalServerError);
                LogDeErrores.RegistroErrorWeb(err, DatoUsuario.idAgente, DatoUsuario.nomAgente, DatoUsuario.sIp, DatoUsuario.sHostName);
                return Json("ErrInterno");
            }
        }
        [Authorize]
        public ActionResult PagoEnLineaExpress()
        {
            return PartialView("_PagoExpressPartial");
        }
        [Authorize]
        public ActionResult preViewPago(CotizadorExpressModel cotiza)
        {

            return PartialView("_preViewPago", cotiza);
        }
        [Authorize]
        public ActionResult PlanSelectedExpress()
        {
            try
            {
                CoberturasModel c = (CoberturasModel)Session["SelectedCoberExpress"];
                if (c == null)
                {
                    c = new CoberturasModel();
                }

                return PartialView("_PlanSelectedPartial", c);
            }
            catch (Exception err)
            {
                Response.StatusCode = (int)(System.Net.HttpStatusCode.InternalServerError);
                LogDeErrores.RegistroErrorWeb(err, DatoUsuario.idAgente, DatoUsuario.nomAgente, DatoUsuario.sIp, DatoUsuario.sHostName);
                return Json("ErrInterno");
            }
        }

        /// <summary>
        /// Método usado para enviar por correo la cotización
        /// </summary>
        /// <param name="cotizaID">NO de cotización</param>
        /// <returns>OK / ERROR</returns>
        /// 
        public JsonResult SendMailExpress(string cotizaID)
        {
            try
            {
                CrReportCentral cr = new CrReportCentral();
                bool success = cr.GeneraCotizacionWebPDF(cotizaID);

                return Json((success ? "OK" : "ERROR"), JsonRequestBehavior.AllowGet);
            }
            catch (Exception err)
            {
                Response.StatusCode = (int)(System.Net.HttpStatusCode.InternalServerError);
                LogDeErrores.RegistroErrorWeb(err, DatoUsuario.idAgente, DatoUsuario.nomAgente, DatoUsuario.sIp, DatoUsuario.sHostName);
                return Json("ErrInterno");
            }
        }

        /// <summary>
        /// Método usado para levantar la ventana con resumen de cotización
        /// </summary>
        /// <param name="id">ID de la cotización</param>
        /// <returns>Vista de Cotizar</returns>
        /// 
        [Authorize]
        public ActionResult CalculoCotizacionCede(bool ejecutaCalculo)
        {
            try
            {
                MensajesError.ErroresCalculo = new List<string>();
                ResumenCotizar rCtz = new ResumenCotizar();
                rCtz.calculos = new List<ClsCalculo>();
                rCtz.Origen = (string)Session["RsmClc"];
                GenericoViewModel model = new GenericoViewModel();
                model = (GenericoViewModel)Session["ModeloEnUso"];

                rCtz.QuiereCederComision = model.QuiereCederComision;
                rCtz.porcentajeCede = model.porcentajeCede;
                rCtz.PorcentajeComisionSource = new Dictionary<int, string>();
                for (int i = 0; i < 21; i++)
                {
                    rCtz.PorcentajeComisionSource.Add(i, i.ToString());
                }

                ClsCotizacion cot = CentralAgentesMvc.App_Start.CastObject.Cast<ClsCotizacion>(model);
                if (!ejecutaCalculo)
                {
                    DefaultCalculos(ref rCtz);

                    model.CalculoCotizacion = rCtz;
                    Session["ModeloEnUso"] = model;
                    return PartialView("_CotizarPartial", rCtz);
                }
                // Realiza el calculo
                string modulo = "C";
                if (model.porcentajecomision >= rCtz.porcentajeCede)
                {
                    //model.comisionOrig = new CedeComision();
                    if (!CedeComision.cargado)
                    {
                        CedeComision.ProcarOriginal = model.control;
                        CedeComision.primanetaOriginal = model.ObjCalculo.PrimaNetaInicial;
                        CedeComision.subtotalOriginal = model.ObjCalculo.mSubtotal;
                        CedeComision.totaloriginal = model.ObjCalculo.mTotal;
                        CedeComision.cargado = true;
                    }
                    double nvoprocar = 0;
                    double quitarprima = 0;
                    VarProcAMC objVarProcAMC = new VarProcAMC();
                    objVarProcAMC.CedeComision(CedeComision.ProcarOriginal, CedeComision.primanetaOriginal, nvoprocar, ref quitarprima, rCtz.porcentajeCede, "C", cot);
                    cot.control = Math.Round(cot.control, 2);

                    string ModulodeTrabajo = model.ModulodeTrabajo;
                    model = CentralAgentesMvc.App_Start.CastObject.Cast<GenericoViewModel>(cot);
                    model.ModulodeTrabajo = ModulodeTrabajo;
                    Calculo(ref model, modulo);
                }
                if (model.formaPago == 0) model.formaPago = 1;
                ClsCalculo ObjCalculoRes = (ClsCalculo)model.calculos[model.formaPago - 1];
                rCtz.DerechoPoliza = decimal.Parse(ObjCalculoRes.dchosPoliza.ToString());
                rCtz.ExtensionRC = decimal.Parse(ObjCalculoRes.mcurExtRC.ToString());
                rCtz.AccidentesPersonales = decimal.Parse(ObjCalculoRes.mAP.ToString());
                rCtz.Recargos = decimal.Parse(ObjCalculoRes.RecPagFrac.ToString());
                rCtz.SubTotal = decimal.Parse(ObjCalculoRes.mSubtotal.ToString());
                rCtz.IVA = ObjCalculoRes.IVA;
                rCtz.PorcentajeIVA = ObjCalculoRes.PorcentajeIVA;
                rCtz.TotalCotizacion = decimal.Parse(ObjCalculoRes.mTotal.ToString());

                rCtz.mPago1de = (model.formaPago == 1 ? ObjCalculoRes.mTotal : ObjCalculoRes.mPago1de);
                rCtz.Descripcion2Pago = (model.formaPago == 3 ? "3" : ObjCalculoRes.nPago2de.ToString().Trim()) + " Pago(s) de.:";
                rCtz.mPago2de = (model.formaPago == 3 ? ObjCalculoRes.mPago1de : ObjCalculoRes.mPago2de);

                rCtz.OpcionesPagoInfo = new List<ClsOpcionesPago>();
                rCtz.VehiculosInfo = new List<ClsVehiculo>();

                rCtz.OpcionesPagoInfo.Add(new ClsOpcionesPago()
                {
                    OpcionPagoID = 1,
                    DescripcionRubro = "Pago Inicial:",
                    PagoContado = string.Format("{0:C2}", ((ClsCalculo)model.calculos[0]).mTotal),
                    PagoSemestral = string.Format("{0:C2}", ((ClsCalculo)model.calculos[1]).mPago1de),
                    PagoTrimestral = string.Format("{0:C2}", ((ClsCalculo)model.calculos[2]).mPago1de),
                    PagoMensual = string.Format("{0:C2}", ((ClsCalculo)model.calculos[3]).mPago1de)
                });

                rCtz.OpcionesPagoInfo.Add(new ClsOpcionesPago()
                {
                    OpcionPagoID = 2,
                    DescripcionRubro = "Pagos Subsecuentes:",
                    PagoContado = string.Format("{0}", ""),
                    PagoSemestral = string.Format("{0:C2}", ((ClsCalculo)model.calculos[1]).mPago2de),
                    PagoTrimestral = string.Format("{0:C2}", ((ClsCalculo)model.calculos[2]).mPago1de),
                    PagoMensual = string.Format("{0:C2}", ((ClsCalculo)model.calculos[3]).mPago2de)
                });

                rCtz.OpcionesPagoInfo.Add(new ClsOpcionesPago()
                {
                    OpcionPagoID = 3,
                    DescripcionRubro = "Número de Pagos Subsecuentes:",
                    PagoContado = string.Format("{0}", ""),
                    PagoSemestral = string.Format("{0}", ((ClsCalculo)model.calculos[1]).nPago2de),
                    PagoTrimestral = string.Format("3"),
                    PagoMensual = string.Format("{0}", ((ClsCalculo)model.calculos[3]).nPago2de)
                });

                rCtz.OpcionesPagoInfo.Add(new ClsOpcionesPago()
                {
                    OpcionPagoID = 4,
                    DescripcionRubro = "<strong><i>Total:</i></strong>",
                    PagoContado = string.Format("<strong><i>{0:C2}</i></strong>", ((ClsCalculo)model.calculos[0]).mTotal),
                    PagoSemestral = string.Format("<strong><i>{0:C2}</i></strong>", ((ClsCalculo)model.calculos[1]).mTotal),
                    PagoTrimestral = string.Format("<strong><i>{0:C2}</i></strong>", ((ClsCalculo)model.calculos[2]).mTotal),
                    PagoMensual = string.Format("<strong><i>{0:C2}</i></strong>", ((ClsCalculo)model.calculos[3]).mTotal)
                });

                foreach (ClsVehiculo vehiculoCal in model.ObjCalculo.vehiculos)
                {
                    rCtz.VehiculosInfo.Add(new ClsVehiculo()
                    {
                        subRamo = vehiculoCal.subRamo,
                        nNumVehi = vehiculoCal.nNumVehi,
                        tpoEspecifAuto = vehiculoCal.DescripcionTipo.Trim(),
                        mSUVA = vehiculoCal.mSUVA,
                        mRespCivil = vehiculoCal.mRespCivil,
                        mAyDLegal = vehiculoCal.mAyDLegal,
                        mDañosMateria = vehiculoCal.mDañosMateria,
                        mRoboTot = vehiculoCal.mRoboTot,
                        mEquiEspec = vehiculoCal.mEquiEspec,
                        mAsisViaje = vehiculoCal.mAsisViaje,
                        mVehiSustitu = vehiculoCal.mVehiSustitu,
                        mTotCoberturas = vehiculoCal.mTotCoberturas,
                        mSA = vehiculoCal.mSA,
                        mcurExtRC = vehiculoCal.mcurExtRC,
                        mSubtotal = vehiculoCal.mSubtotal,
                        PorcentajeIVA = vehiculoCal.PorcentajeIVA,
                        mTotal = vehiculoCal.mTotal,
                    });
                }

                if (MensajesError.ErroresCalculo != null)
                {
                    if (MensajesError.ErroresCalculo.Count() > 0)
                        rCtz.WarningCalculoInfo = MensajesError.ErroresCalculo;
                }

                model.CalculoCotizacion = rCtz;
                Session["ModeloEnUso"] = model;
                return PartialView("_CotizarPartial", rCtz);
            }
            catch (Exception err)
            {
                Response.StatusCode = (int)(System.Net.HttpStatusCode.InternalServerError);
                LogDeErrores.RegistroErrorWeb(err, DatoUsuario.idAgente, DatoUsuario.nomAgente, DatoUsuario.sIp, DatoUsuario.sHostName);
                return Json("ErrInterno");
            }
        }

        /// <summary>
        /// Método usado para levantar la ventana con resumen de cotización
        /// </summary>
        /// <param name="id">ID de la cotización</param>
        /// <returns>Vista de Cotizar</returns>
        /// 
        [Authorize]
        public ActionResult CalculoCotizacionCedida(bool ejecutaCalculo)
        {
            try
            {
                MensajesError.ErroresCalculo = new List<string>();
                ResumenCotizar rCtz = new ResumenCotizar();
                rCtz.calculos = new List<ClsCalculo>();
                rCtz.Origen = (string)Session["RsmClc"];
                GenericoViewModel model = new GenericoViewModel();
                model = (GenericoViewModel)Session["ModeloEnUso"];
                if (model.nQuitarPorPrima != 0)
                {
                    rCtz.QuiereCederComision = model.QuiereCederComision = false;
                    rCtz.porcentajeCede = model.nQuitarPorPrima;
                    //model.porcentajecomision = model.nQuitarPorPrima;
                }
                rCtz.PorcentajeComisionSource = new Dictionary<int, string>();
                for (int i = 0; i < 21; i++)
                {
                    rCtz.PorcentajeComisionSource.Add(i, i.ToString());
                }

                //rCtz.PorcentajeComisionSource = model.PorcentajeComisionSource;
                if (!ejecutaCalculo)
                {
                    DefaultCalculos(ref rCtz);

                    model.CalculoCotizacion = rCtz;
                    Session["ModeloEnUso"] = model;
                    return PartialView("_CotizarPartial", rCtz);
                }
                // Realiza el calculo
                string modulo = "C";
                Calculo(ref model, modulo);
                if (!CedeComision.cargado)
                {
                    CedeComision.ProcarOriginal = model.control;
                    CedeComision.primanetaOriginal = model.ObjCalculo.PrimaNetaInicial;
                    CedeComision.subtotalOriginal = model.ObjCalculo.mSubtotal;
                    CedeComision.totaloriginal = model.ObjCalculo.mTotal;
                    CedeComision.cargado = true;
                }

                ClsCotizacion cot = CentralAgentesMvc.App_Start.CastObject.Cast<ClsCotizacion>(model);
                rCtz.QuiereCederComision = model.QuiereCederComision = true;
                double nvoprocar = 0;
                double quitarprima = 0;
                VarProcAMC objVarProcAMC = new VarProcAMC();
                objVarProcAMC.CedeComision(CedeComision.ProcarOriginal, CedeComision.primanetaOriginal, nvoprocar, ref quitarprima, rCtz.porcentajeCede, "C", cot);
                cot.control = Math.Round(cot.control, 2);
                string ModulodeTrabajo = model.ModulodeTrabajo;
                model = CentralAgentesMvc.App_Start.CastObject.Cast<GenericoViewModel>(cot);
                model.ModulodeTrabajo = ModulodeTrabajo;

                Calculo(ref model, modulo);
                if (model.formaPago == 0) model.formaPago = 1;
                ClsCalculo ObjCalculoRes = (ClsCalculo)model.calculos[model.formaPago - 1];
                rCtz.DerechoPoliza = decimal.Parse(ObjCalculoRes.dchosPoliza.ToString());
                rCtz.ExtensionRC = decimal.Parse(ObjCalculoRes.mcurExtRC.ToString());
                rCtz.AccidentesPersonales = decimal.Parse(ObjCalculoRes.mAP.ToString());
                rCtz.Recargos = decimal.Parse(ObjCalculoRes.RecPagFrac.ToString());
                rCtz.SubTotal = decimal.Parse(ObjCalculoRes.mSubtotal.ToString());
                rCtz.IVA = ObjCalculoRes.IVA;
                rCtz.PorcentajeIVA = ObjCalculoRes.PorcentajeIVA;
                rCtz.TotalCotizacion = decimal.Parse(ObjCalculoRes.mTotal.ToString());

                rCtz.mPago1de = (model.formaPago == 1 ? ObjCalculoRes.mTotal : ObjCalculoRes.mPago1de);
                rCtz.Descripcion2Pago = (model.formaPago == 3 ? "3" : ObjCalculoRes.nPago2de.ToString().Trim()) + " Pago(s) de.:";
                rCtz.mPago2de = (model.formaPago == 3 ? ObjCalculoRes.mPago1de : ObjCalculoRes.mPago2de);

                rCtz.OpcionesPagoInfo = new List<ClsOpcionesPago>();
                rCtz.VehiculosInfo = new List<ClsVehiculo>();

                rCtz.OpcionesPagoInfo.Add(new ClsOpcionesPago()
                {
                    OpcionPagoID = 1,
                    DescripcionRubro = "Pago Inicial:",
                    PagoContado = string.Format("{0:C2}", ((ClsCalculo)model.calculos[0]).mTotal),
                    PagoSemestral = string.Format("{0:C2}", ((ClsCalculo)model.calculos[1]).mPago1de),
                    PagoTrimestral = string.Format("{0:C2}", ((ClsCalculo)model.calculos[2]).mPago1de),
                    PagoMensual = string.Format("{0:C2}", ((ClsCalculo)model.calculos[3]).mPago1de)
                });

                rCtz.OpcionesPagoInfo.Add(new ClsOpcionesPago()
                {
                    OpcionPagoID = 2,
                    DescripcionRubro = "Pagos Subsecuentes:",
                    PagoContado = string.Format("{0}", ""),
                    PagoSemestral = string.Format("{0:C2}", ((ClsCalculo)model.calculos[1]).mPago2de),
                    PagoTrimestral = string.Format("{0:C2}", ((ClsCalculo)model.calculos[2]).mPago1de),
                    PagoMensual = string.Format("{0:C2}", ((ClsCalculo)model.calculos[3]).mPago2de)
                });

                rCtz.OpcionesPagoInfo.Add(new ClsOpcionesPago()
                {
                    OpcionPagoID = 3,
                    DescripcionRubro = "Número de Pagos Subsecuentes:",
                    PagoContado = string.Format("{0}", ""),
                    PagoSemestral = string.Format("{0}", ((ClsCalculo)model.calculos[1]).nPago2de),
                    PagoTrimestral = string.Format("3"),
                    PagoMensual = string.Format("{0}", ((ClsCalculo)model.calculos[3]).nPago2de)
                });

                rCtz.OpcionesPagoInfo.Add(new ClsOpcionesPago()
                {
                    OpcionPagoID = 4,
                    DescripcionRubro = "<strong><i>Total:</i></strong>",
                    PagoContado = string.Format("<strong><i>{0:C2}</i></strong>", ((ClsCalculo)model.calculos[0]).mTotal),
                    PagoSemestral = string.Format("<strong><i>{0:C2}</i></strong>", ((ClsCalculo)model.calculos[1]).mTotal),
                    PagoTrimestral = string.Format("<strong><i>{0:C2}</i></strong>", ((ClsCalculo)model.calculos[2]).mTotal),
                    PagoMensual = string.Format("<strong><i>{0:C2}</i></strong>", ((ClsCalculo)model.calculos[3]).mTotal)
                });

                foreach (ClsVehiculo vehiculoCal in model.ObjCalculo.vehiculos)
                {
                    rCtz.VehiculosInfo.Add(new ClsVehiculo()
                    {
                        subRamo = vehiculoCal.subRamo,
                        nNumVehi = vehiculoCal.nNumVehi,
                        tpoEspecifAuto = vehiculoCal.DescripcionTipo.Trim(),
                        mSUVA = vehiculoCal.mSUVA,
                        mRespCivil = vehiculoCal.mRespCivil,
                        mAyDLegal = vehiculoCal.mAyDLegal,
                        mDañosMateria = vehiculoCal.mDañosMateria,
                        mRoboTot = vehiculoCal.mRoboTot,
                        mEquiEspec = vehiculoCal.mEquiEspec,
                        mAsisViaje = vehiculoCal.mAsisViaje,
                        mVehiSustitu = vehiculoCal.mVehiSustitu,
                        mTotCoberturas = vehiculoCal.mTotCoberturas,
                        mSA = vehiculoCal.mSA,
                        mcurExtRC = vehiculoCal.mcurExtRC,
                        mSubtotal = vehiculoCal.mSubtotal,
                        PorcentajeIVA = vehiculoCal.PorcentajeIVA,
                        mTotal = vehiculoCal.mTotal,
                    });
                }

                if (MensajesError.ErroresCalculo != null)
                {
                    if (MensajesError.ErroresCalculo.Count() > 0)
                        rCtz.WarningCalculoInfo = MensajesError.ErroresCalculo;
                }

                model.CalculoCotizacion = rCtz;
                Session["ModeloEnUso"] = model;
                return PartialView("_CotizarPartial", rCtz);
            }
            catch (Exception err)
            {
                Response.StatusCode = (int)(System.Net.HttpStatusCode.InternalServerError);
                LogDeErrores.RegistroErrorWeb(err, DatoUsuario.idAgente, DatoUsuario.nomAgente, DatoUsuario.sIp, DatoUsuario.sHostName);
                return Json("ErrInterno");
            }
        }

        /// <summary>
        /// Método usado para levantar la ventana con resumen de cotización
        /// </summary>
        /// <param name="id">ID de la cotización</param>
        /// <returns>Vista de Cotizar</returns>
        /// 
        [Authorize]
        public ActionResult CalculoCotizacion(bool ejecutaCalculo)
        {
            try
            {
                MensajesError.ErroresCalculo = new List<string>();
                ResumenCotizar rCtz = new ResumenCotizar();
                rCtz.calculos = new List<ClsCalculo>();
                rCtz.Origen = (string)Session["RsmClc"];
                GenericoViewModel model = new GenericoViewModel();
                model = (GenericoViewModel)Session["ModeloEnUso"];
                rCtz.QuiereCederComision = model.QuiereCederComision;
                rCtz.porcentajeCede = model.porcentajeCede;
                rCtz.PorcentajeComisionSource = new Dictionary<int, string>();

                rCtz.WarningCalculoInfo = new List<string>();
                for (int i = 0; i < 21; i++)
                {
                    rCtz.PorcentajeComisionSource.Add(i, i.ToString());
                }
                //rCtz.PorcentajeComisionSource = model.PorcentajeComisionSource;
                rCtz.cotizacionCom = model;
                rCtz.cotizacionCom.chkCedeComision = model.chkCedeComision == null ? "N" : model.chkCedeComision;

                ClsCotizacion cot = CentralAgentesMvc.App_Start.CastObject.Cast<ClsCotizacion>(model);
                if (!ejecutaCalculo)
                {
                    DefaultCalculos(ref rCtz);

                    model.CalculoCotizacion = rCtz;
                    Session["ModeloEnUso"] = model;
                    return PartialView("_CotizarPartial", rCtz);
                }
                // Realiza el calculo
                string modulo = "C";
                Calculo(ref model, modulo);
                if (MensajesError.ErroresCalculo.Count() > 0)
                {
                    rCtz.WarningCalculoInfo = MensajesError.ErroresCalculo;
                }
                else
                {
                    if (model.formaPago == 0) model.formaPago = 1;
                    ClsCalculo ObjCalculoRes = (ClsCalculo)model.calculos[model.formaPago - 1];
                    rCtz.DerechoPoliza = decimal.Parse(ObjCalculoRes.dchosPoliza.ToString());
                    rCtz.ExtensionRC = decimal.Parse(ObjCalculoRes.mcurExtRC.ToString());
                    rCtz.AccidentesPersonales = decimal.Parse(ObjCalculoRes.mAP.ToString());
                    rCtz.Recargos = decimal.Parse(ObjCalculoRes.RecPagFrac.ToString());
                    rCtz.SubTotal = decimal.Parse(ObjCalculoRes.mSubtotal.ToString());
                    rCtz.IVA = ObjCalculoRes.IVA;
                    rCtz.PorcentajeIVA = ObjCalculoRes.PorcentajeIVA;
                    rCtz.TotalCotizacion = decimal.Parse(ObjCalculoRes.mTotal.ToString());

                    rCtz.mPago1de = (model.formaPago == 1 ? ObjCalculoRes.mTotal : ObjCalculoRes.mPago1de);
                    rCtz.Descripcion2Pago = (model.formaPago == 3 ? "3" : ObjCalculoRes.nPago2de.ToString().Trim()) + " Pago(s) de.:";
                    rCtz.mPago2de = (model.formaPago == 3 ? ObjCalculoRes.mPago1de : ObjCalculoRes.mPago2de);

                    rCtz.OpcionesPagoInfo = new List<ClsOpcionesPago>();
                    rCtz.VehiculosInfo = new List<ClsVehiculo>();

                    rCtz.OpcionesPagoInfo.Add(new ClsOpcionesPago()
                    {
                        OpcionPagoID = 1,
                        DescripcionRubro = "Pago Inicial:",
                        PagoContado = string.Format("{0:C2}", ((ClsCalculo)model.calculos[0]).mTotal),
                        PagoSemestral = string.Format("{0:C2}", ((ClsCalculo)model.calculos[1]).mPago1de),
                        PagoTrimestral = string.Format("{0:C2}", ((ClsCalculo)model.calculos[2]).mPago1de),
                        PagoMensual = string.Format("{0:C2}", ((ClsCalculo)model.calculos[3]).mPago1de)
                    });

                    rCtz.OpcionesPagoInfo.Add(new ClsOpcionesPago()
                    {
                        OpcionPagoID = 2,
                        DescripcionRubro = "Pagos Subsecuentes:",
                        PagoContado = string.Format("{0}", ""),
                        PagoSemestral = string.Format("{0:C2}", ((ClsCalculo)model.calculos[1]).mPago2de),
                        PagoTrimestral = string.Format("{0:C2}", ((ClsCalculo)model.calculos[2]).mPago1de),
                        PagoMensual = string.Format("{0:C2}", ((ClsCalculo)model.calculos[3]).mPago2de)
                    });

                    rCtz.OpcionesPagoInfo.Add(new ClsOpcionesPago()
                    {
                        OpcionPagoID = 3,
                        DescripcionRubro = "Número de Pagos Subsecuentes:",
                        PagoContado = string.Format("{0}", ""),
                        PagoSemestral = string.Format("{0}", ((ClsCalculo)model.calculos[1]).nPago2de),
                        PagoTrimestral = string.Format("3"),
                        PagoMensual = string.Format("{0}", ((ClsCalculo)model.calculos[3]).nPago2de)
                    });

                    rCtz.OpcionesPagoInfo.Add(new ClsOpcionesPago()
                    {
                        OpcionPagoID = 4,
                        DescripcionRubro = "<strong><i>Total:</i></strong>",
                        PagoContado = string.Format("<strong><i>{0:C2}</i></strong>", ((ClsCalculo)model.calculos[0]).mTotal),
                        PagoSemestral = string.Format("<strong><i>{0:C2}</i></strong>", ((ClsCalculo)model.calculos[1]).mTotal),
                        PagoTrimestral = string.Format("<strong><i>{0:C2}</i></strong>", ((ClsCalculo)model.calculos[2]).mTotal),
                        PagoMensual = string.Format("<strong><i>{0:C2}</i></strong>", ((ClsCalculo)model.calculos[3]).mTotal)
                    });

                    foreach (ClsVehiculo vehiculoCal in model.ObjCalculo.vehiculos)
                    {
                        rCtz.VehiculosInfo.Add(new ClsVehiculo()
                        {
                            subRamo = vehiculoCal.subRamo,
                            nNumVehi = vehiculoCal.nNumVehi,
                            tpoEspecifAuto = vehiculoCal.DescripcionTipo.Trim(),
                            mSUVA = vehiculoCal.mSUVA,
                            mRespCivil = vehiculoCal.mRespCivil,
                            mAyDLegal = vehiculoCal.mAyDLegal,
                            mDañosMateria = vehiculoCal.mDañosMateria,
                            mRoboTot = vehiculoCal.mRoboTot,
                            mEquiEspec = vehiculoCal.mEquiEspec,
                            mAsisViaje = vehiculoCal.mAsisViaje,
                            mVehiSustitu = vehiculoCal.mVehiSustitu,
                            mTotCoberturas = vehiculoCal.mTotCoberturas,
                            mSA = vehiculoCal.mSA,
                            mcurExtRC = vehiculoCal.mcurExtRC,
                            mSubtotal = vehiculoCal.mSubtotal,
                            PorcentajeIVA = vehiculoCal.PorcentajeIVA,
                            mTotal = vehiculoCal.mTotal,
                        });
                    }
                }

                model.CalculoCotizacion = rCtz;
                Session["ModeloEnUso"] = model;
                return PartialView("_CotizarPartial", rCtz);
            }
            catch (Exception err)
            {
                Response.StatusCode = (int)(System.Net.HttpStatusCode.InternalServerError);
                LogDeErrores.RegistroErrorWeb(err, DatoUsuario.idAgente, DatoUsuario.nomAgente, DatoUsuario.sIp, DatoUsuario.sHostName);
                return Json("ErrInterno");
            }
        }

        /// <summary>
        /// Método para refrescar el calculo de los vehiculos
        /// </summary>
        /// <returns>Json con los datos</returns>
        public JsonResult RefreshCalculoVehiculos()
        {
            try
            {
                GenericoViewModel d = (GenericoViewModel)Session["ModeloEnUso"];
                return Json(d.CalculoCotizacion.VehiculosInfo, JsonRequestBehavior.AllowGet);
            }
            catch (Exception err)
            {
                Response.StatusCode = (int)(System.Net.HttpStatusCode.InternalServerError);
                LogDeErrores.RegistroErrorWeb(err, DatoUsuario.idAgente, DatoUsuario.nomAgente, DatoUsuario.sIp, DatoUsuario.sHostName);
                return Json("ErrInterno");
            }
        }

        /// <summary>
        /// Método para refrescar las formas de pago generadas por el calculo
        /// </summary>
        /// <returns>Json con los datos</returns>
        public JsonResult RefreshCalculoFormasPagos()
        {
            try
            {
                GenericoViewModel d = (GenericoViewModel)Session["ModeloEnUso"];
                return Json(d.CalculoCotizacion.OpcionesPagoInfo, JsonRequestBehavior.AllowGet);
            }
            catch (Exception err)
            {
                Response.StatusCode = (int)(System.Net.HttpStatusCode.InternalServerError);
                LogDeErrores.RegistroErrorWeb(err, DatoUsuario.idAgente, DatoUsuario.nomAgente, DatoUsuario.sIp, DatoUsuario.sHostName);
                return Json("ErrInterno");
            }
        }

        /// <summary>
        /// Método para recuperar los errores generados por el calculo
        /// </summary>
        /// <returns>Json con la lista de errores</returns>
        public JsonResult GetErroresCalculo()
        {
            try
            {
                GenericoViewModel d = (GenericoViewModel)Session["ModeloEnUso"];
                if(d.CalculoCotizacion.WarningCalculoInfo == null)
                    d.CalculoCotizacion.WarningCalculoInfo = new List<string>();
                return Json(d.CalculoCotizacion.WarningCalculoInfo, JsonRequestBehavior.AllowGet);
            }
            catch (Exception err)
            {
                Response.StatusCode = (int)(System.Net.HttpStatusCode.InternalServerError);
                LogDeErrores.RegistroErrorWeb(err, DatoUsuario.idAgente, DatoUsuario.nomAgente, DatoUsuario.sIp, DatoUsuario.sHostName);
                return Json("ErrInterno");
            }
        }
        #endregion

        #region <-- Servicios en Linea -->
        public ActionResult ServiciosEnLinea(string poliza, string solicitud)
        {
            try
            {
                var rpt = new ServiciosModel()
                {
                    PolizaID = poliza,
                    SolicitudID = solicitud,
                };

                return View(rpt);
            }
            catch (Exception err)
            {
                Response.StatusCode = (int)(System.Net.HttpStatusCode.InternalServerError);
                LogDeErrores.RegistroErrorWeb(err, DatoUsuario.idAgente, DatoUsuario.nomAgente, DatoUsuario.sIp, DatoUsuario.sHostName);
                return Json("ErrInterno");
            }
        }

        /// <summary>
        /// Método que devuelve las facturas par auna poliza y solicitud
        /// </summary>
        /// <param name="polizaID">Número de Póliza</param>
        /// <param name="solicitudID">Número de Solicitud</param>
        /// <param name="offset">Número de página a devolver</param>
        /// <param name="limit">Cantidad de registros a devolver</param>
        /// <returns>Json con resultados</returns>
        public ActionResult GetDataService(string polizaID, string solicitudID, int offset, int limit)
        {
            try
            {
                if (string.IsNullOrEmpty(polizaID)) polizaID = "0";
                if (string.IsNullOrEmpty(solicitudID)) solicitudID = "0";
                if (offset == 0) offset = 1;
                if (limit == 0) limit = 10;

                ManejoInformacion info = new ManejoInformacion();
                var data = info.FacturasEnLinea(polizaID, solicitudID, offset, limit);
                var totalRows = Convert.ToInt32(data.Tables["Pager"].Rows[0]["RecordCount"]);
                DataTable vigencias = new DataTable();
                List<PolizaFacturacion> facturacion = new List<PolizaFacturacion>();
                var List = (from g in data.Tables[0].Select()
                            select new PolizaFacturacion
                            {
                                nPoliza = long.Parse(g["nPoliza"].ToString()),
                                dfExpedicion = String.Format("{0:dd/MM/yyyy}", g["dfExpedicion"]),
                                cNombreA = g["cNombreA"].ToString(),
                                nEndoso = int.Parse(g["nEndoso"].ToString()),
                                nRecibo = int.Parse(g["nRecibo"].ToString()),
                                Fecha_Emision = String.Format("{0:dd/MM/yyyy}", g["Fecha_Emision"]),
                                Serie = g["Serie"].ToString(),
                                Folio = g["Folio"].ToString(),
                                LnkPDF = g["LnkPDF"].ToString(),
                                LnkXML = g["LnkXML"].ToString(),
                                Lnk100 = g["Lnk100"].ToString(),
                                Css100 = g["Css100"].ToString(),
                                LnkSOB = g["LnkSOB"].ToString(),
                                dFIniVig = String.Format("{0:dd/MM/yyyy}", g["dFIniVig"]),
                                dFFinVig = String.Format("{0:dd/MM/yyyy}", g["dFFinVig"])
                            });

                facturacion = List.ToList();
                return PartialView("_GridFacturacion", facturacion);
            }
            catch (Exception err)
            {
                Response.StatusCode = (int)(System.Net.HttpStatusCode.InternalServerError);
                LogDeErrores.RegistroErrorWeb(err, DatoUsuario.idAgente, DatoUsuario.nomAgente, DatoUsuario.sIp, DatoUsuario.sHostName);
                return Json("ErrInterno");
            }

        }
        #endregion

        #region <-- JSON Methods -->
        /// <summary>
        /// metodo para calculos de cotizacion express
        /// </summary>
        /// <param name="model">El modelo del la cotizacion</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult ResultadoCotizacion(CotizaExpressModel model)
        {
            try
            {
                if (model.OrigenCotExp == "Clientes")
                {
                    if (model.CaptchaText == HttpContext.Session["captchastring"].ToString())
                        ViewBag.Message = "CAPTCHA correcto!";
                    else
                    {
                        ViewBag.Message = "CAPTCHA incorrecto!";

                        ModelState["CaptchaText"].Errors.Add("CAPTCHA incorrecto!");
                    }
                }

                if (!ModelState.IsValid)
                {
                    return Json(new { success = false, errores = GetElementsFromModelState(), validaciones = "", view = model });
                }

                model.conductor.sexo = (model.EsHombre ? 0 : 1);
                model.conductor.nNumCond = 1;
                model.conductor.nombre = model.cotizacion.apellidoPaterno + " " + model.cotizacion.apellidoMaterno + " " + model.cotizacion.nombres;
                model.cotizacion.sexo = model.conductor.sexo;
                model.cotizacion.titulo = "C.";
                model.cotizacion.nombreAsegurado = model.conductor.nombre;

                model.cotizacion.conductores.Add(model.conductor);
                model.cotizacion.meses = 12;
                model.cotizacion.cobertura = 12;
                model.cotizacion.cEra = "N";
                model.cotizacion.conductorRestringido = 1;
                model.cotizacion.oficinaID = 1;
                model.cotizacion.agente = 4;
                model.cotizacion.clienteID = 1;
                var client = VarProcInterfazX.arrClientes.First(c => c.lngClave == 1);
                model.cotizacion.ClienteInfo = new ClsCliente()
                {
                    ClienteID = client.lngClave,
                    RazonSocial = client.strDescrip,
                };
                model.cotizacion.nomTitular = client.strDescrip;

                model.vehiculo.nNumVehi = 1;
                model.vehiculo.conducAsign = model.conductor.nNumCond = 1;
                model.vehiculo.cCOBER100SN = "";
                if (model.cotizacion.telefono.Count == 0)
                {
                    if (model.Telefonos == null)
                    {
                        model.Telefonos = new ClsTelefono();
                        model.Telefonos.cTel = "0";
                        model.Telefonos.cLada = "0";
                        model.Telefonos.cCelular = "0";
                    }
                    model.cotizacion.telefono.Add(model.Telefonos);
                    model.cotizacion.telefono[0].cLada = "55";
                    model.cotizacion.telefono[0].cCelular = "5555555555";
                    model.Telefonos.cHistorial = "";
                    model.Telefonos.cTpo1 = "";
                }

                if (model.vehiculo.subRamo == "L")
                {
                    model.vehiculo.modelo = 999;
                    model.vehiculo.DescripcionTipo = VarProcInterfazX.arrModelos.FirstOrDefault(t => t.lngClave == model.vehiculo.modelo).strModelo;
                    model.vehiculo.VIN = model.tipoVeh;
                }
                else
                {
                    model.vehiculo.modelo = int.Parse(model.tipoVeh);
                    model.vehiculo.DescripcionTipo = VarProcInterfazX.arrModelos.FirstOrDefault(t => t.lngClave == model.vehiculo.modelo).strModelo;
                }
                model.vehiculo.tpoEspecifAuto = model.vehiculo.DescripcionTipo;
                model.vehiculo.codigoPostal = model.cotizacion.codigoPostal;
                model.cotizacion.vehiculos.Add(model.vehiculo);
                model.cotizacion.numConductores = model.cotizacion.conductores.Count;
                model.cotizacion.numVehiculos = model.cotizacion.vehiculos.Count;
                model.cotizacion.meses = 12;
                model.cotizacion.cFormaPago = "T";
                model.cotizacion.tipoPago = "C";
                // poner aqui el valor que depende de donde vienen dirigidos

                string modulo = "C";
                ResumenCotizar rCtz = new ResumenCotizar();
                TempData.Keep("RsmClc");
                modulo = "C";
                GenericoViewModel ModeloGenerico = new GenericoViewModel();
                ModeloGenerico = CentralAgentesMvc.App_Start.CastObject.Cast<GenericoViewModel>(model.cotizacion);
                DataSet TpCober = MnInf.CargaTipoCotizaciones(model.OrigenCotExp);
                DataTable tblCoberturas = new DataTable("Cotizaciones");
                tblCoberturas.Columns.Add("TipoCobertura", typeof(string));
                tblCoberturas.Columns.Add("Contado", typeof(string));
                tblCoberturas.Columns.Add("Semestral", typeof(string));
                tblCoberturas.Columns.Add("Trimestral", typeof(string));
                tblCoberturas.Columns.Add("Mensual", typeof(string));
                switch (model.OrigenCotExp)
                {
                    #region clientes
                    case "Clientes":
                        CotizaExpressModel.coberturas = new List<CoberturasModel>();
                        model.vehiculo.sumAsegFija = "S";

                        int pvtCalculos = -1;

                        ModeloGenerico.tblCotizacionesCober = new DataTable("CotizacionesCober");
                        ModeloGenerico.tblCotizacionesCober.Columns.Add("nOrdenCobertura", typeof(string));
                        ModeloGenerico.tblCotizacionesCober.Columns.Add("Cobertura", typeof(string));
                        ModeloGenerico.tblCotizacionesCober.Columns.Add("nFormaPago", typeof(string));
                        ModeloGenerico.tblCotizacionesCober.Columns.Add("desFormaPago", typeof(string));
                        ModeloGenerico.tblCotizacionesCober.Columns.Add("PagoIni", typeof(string));
                        ModeloGenerico.tblCotizacionesCober.Columns.Add("PagoSub", typeof(string));
                        ModeloGenerico.tblCotizacionesCober.Columns.Add("Total", typeof(string));

                        foreach (DataRow cobertura in TpCober.Tables[0].Rows)
                        {
                            CoberturasModel CalculoCober = new CoberturasModel();
                            CalculoCober.OrdenCobertura = int.Parse(cobertura["OrdenCobertura"].ToString());
                            CalculoCober.Nombre = cobertura["Nombre"].ToString();
                            CalculoCober.Descripcion = cobertura["Descripcion"].ToString();
                            CalculoCober.DM = int.Parse(cobertura["DM"].ToString());
                            CalculoCober.RT = int.Parse(cobertura["RT"].ToString());
                            CalculoCober.Cober100 = cobertura["Cober100"].ToString();
                            CalculoCober.Terssa = cobertura["Terssa"].ToString();
                            CalculoCober.ExtensionRC = cobertura["ExtensionRC"].ToString();
                            CalculoCober.GarageCasa = cobertura["GarageCasa"].ToString();
                            CalculoCober.GarageTrabajo = cobertura["GarageTrabajo"].ToString();
                            CalculoCober.Usotrabajo = cobertura["Usotrabajo"].ToString();
                            CalculoCober.RC = int.Parse(cobertura["RC"].ToString());
                            CalculoCober.RCcatastrofica = int.Parse(cobertura["RCcatastrofica"].ToString());
                            CalculoCober.AseLegal = cobertura["AseLegal"].ToString();
                            CalculoCober.SumaAseg = cobertura["SumaAseg"].ToString();
                            CalculoCober.VehSus = cobertura["VehSus"].ToString();
                            CalculoCober.Aviaje = cobertura["Aviaje"].ToString();
                            CalculoCober.BajoRiesgo = cobertura["BajoRiesgo"].ToString();
                            CalculoCober.MasAutos = cobertura["MasAutos"].ToString();
                            CalculoCober.Procar = int.Parse(cobertura["Procar"].ToString());
                            CalculoCober.Campaña = int.Parse(cobertura["Campaña"].ToString());
                            CalculoCober.DetalleCobertura = cobertura["DetalleCobertura"].ToString();
                            CalculoCober.GM = int.Parse(cobertura["GastosMedicos"].ToString());
                            CalculoCober.ExDedu = cobertura["ExenDeducible"].ToString();

                            ModeloGenerico.vehiculos[0].ExcenDedu = CalculoCober.ExDedu;
                            ModeloGenerico.vehiculos[0].coberGtoMed = CalculoCober.GM;
                            ModeloGenerico.vehiculos[0].deducDañMat = CalculoCober.DM;
                            ModeloGenerico.vehiculos[0].deducRobTot = CalculoCober.RT;
                            ModeloGenerico.vehiculos[0].cober100 = CalculoCober.Cober100;
                            ModeloGenerico.tersa = CalculoCober.Terssa == "N" ? 0 : 1;
                            ModeloGenerico.vehiculos[0].estacionaCasa = CalculoCober.GarageCasa;
                            ModeloGenerico.vehiculos[0].estacionaTrab = CalculoCober.GarageTrabajo;
                            ModeloGenerico.vehiculos[0].usoTrabajo = CalculoCober.Usotrabajo;
                            ModeloGenerico.vehiculos[0].coberRepCiv = CalculoCober.RC;
                            ModeloGenerico.vehiculos[0].coberRepCivCat = CalculoCober.RCcatastrofica;
                            ModeloGenerico.vehiculos[0].proliber = CalculoCober.AseLegal;
                            ModeloGenerico.vehiculos[0].sumAsegFija = CalculoCober.SumaAseg;
                            ModeloGenerico.vehiculos[0].vehSus = CalculoCober.VehSus;
                            ModeloGenerico.vehiculos[0].asistenciaViaje = CalculoCober.Aviaje;
                            ModeloGenerico.vehiculos[0].EsConductorBajoRiesgo = CalculoCober.BajoRiesgo;
                            ModeloGenerico.vehiculos[0].puertas = 4;
                            ModeloGenerico.vehiculos[0].cilindros = 4;
                            ModeloGenerico.vehiculos[0].validaSerie = "S";
                            ModeloGenerico.vehiculos[0].SUVA = "S";
                            ModeloGenerico.complemento.telefono = ModeloGenerico.telefono[0].cTel;
                            ModeloGenerico.conductorRestringido = CalculoCober.BajoRiesgo == "S" ? 0 : 1;
                            ModeloGenerico.conductores[0].extRespCivil = CalculoCober.Cober100 == "S" ? 1 : 0;
                            ModeloGenerico.cobertura100 = CalculoCober.Cober100 == "S" ? 1 : 0;
                            ModeloGenerico.masAutos = CalculoCober.MasAutos;
                            ModeloGenerico.control = CalculoCober.Procar;
                            //CtExp.cotizacion.campaña = 1549;
                            ValidaParaCotizara(ModeloGenerico);

                            Calculo(ref ModeloGenerico, modulo);
                            CotizaExpressModel.coberturas.Add(CalculoCober);
                            DataRow fila = tblCoberturas.NewRow();

                            fila["TipoCobertura"] = "<div class='col-xs-4 text-right'>" +
                                "<a id='popoverH" + CalculoCober.OrdenCobertura + "'" +
                                "   class='btn btn-sm btn-primary qtip-content'" +
                                "   role='button' onmouseover=\"tooltips(\'popoverH" + CalculoCober.OrdenCobertura + "\',\'" + CalculoCober.DetalleCobertura + "\',\'" + CalculoCober.Nombre + "\')\" > ?" +
                                "</a>" +
                                "</div><strong>" + CalculoCober.Nombre + "</strong>" + "</br>" + CalculoCober.Descripcion;

                            pvtCalculos++;
                            fila["Contado"] = "<input name='savecoti' value='" + CalculoCober.OrdenCobertura + "|1' type='radio'> <strong>" +
                                                    string.Format("{0:C2}", ModeloGenerico.calculos[pvtCalculos].mTotal) + "</strong> </br> Pago único";

                            /**********************************************************************************/
                            DataRow cotCoberFila = ModeloGenerico.tblCotizacionesCober.NewRow();
                            cotCoberFila["nOrdenCobertura"] = CalculoCober.OrdenCobertura;
                            cotCoberFila["Cobertura"] = CalculoCober.Nombre;
                            cotCoberFila["nFormaPago"] = "1";
                            cotCoberFila["desFormaPago"] = "Contado";
                            cotCoberFila["PagoIni"] = ModeloGenerico.calculos[pvtCalculos].mTotal;
                            cotCoberFila["PagoSub"] = "Pago único";
                            cotCoberFila["Total"] = ModeloGenerico.calculos[pvtCalculos].mTotal;
                            ModeloGenerico.tblCotizacionesCober.Rows.Add(cotCoberFila);
                            /***********************************************************************************/
                            pvtCalculos++;
                            fila["Semestral"] = "<input name='savecoti' value='" + CalculoCober.OrdenCobertura + "|2' type='radio'> <strong>" +
                                                    string.Format("{0:C2}", ModeloGenerico.calculos[pvtCalculos].mTotal) + "</strong></br> primer pago de " +
                                                    string.Format("{0:C2}", ModeloGenerico.calculos[pvtCalculos].mPago1de) + " y " +
                                                    ModeloGenerico.calculos[pvtCalculos].nPago2de + " pagos de " +
                                                    string.Format("{0:C2}", ModeloGenerico.calculos[pvtCalculos].mPago2de);
                            /************************************************************************************/
                            DataRow cotCoberFila2 = ModeloGenerico.tblCotizacionesCober.NewRow();
                            cotCoberFila2["nOrdenCobertura"] = CalculoCober.OrdenCobertura;
                            cotCoberFila2["Cobertura"] = CalculoCober.Nombre;
                            cotCoberFila2["nFormaPago"] = "2";
                            cotCoberFila2["desFormaPago"] = "Semestral";
                            cotCoberFila2["PagoIni"] = ModeloGenerico.calculos[pvtCalculos].mPago1de;
                            cotCoberFila2["PagoSub"] = ModeloGenerico.calculos[pvtCalculos].nPago2de + " pagos de " +
                                                                    ModeloGenerico.calculos[pvtCalculos].mPago2de;
                            cotCoberFila2["Total"] = ModeloGenerico.calculos[pvtCalculos].mTotal;
                            ModeloGenerico.tblCotizacionesCober.Rows.Add(cotCoberFila2);
                            /******************************************************************************/
                            pvtCalculos++;
                            fila["Trimestral"] = "<input name='savecoti' value='" + CalculoCober.OrdenCobertura + "|3' type='radio'> <strong>" +
                                                    string.Format("{0:C2}", ModeloGenerico.calculos[pvtCalculos].mTotal) + "</strong></br> cuatro pagos de  " +
                                                    string.Format("{0:C2}", ModeloGenerico.calculos[pvtCalculos].mPago1de);
                            /**********************************************************************************/
                            DataRow cotCoberFila3 = ModeloGenerico.tblCotizacionesCober.NewRow();
                            cotCoberFila3["nOrdenCobertura"] = CalculoCober.OrdenCobertura;
                            cotCoberFila3["Cobertura"] = CalculoCober.Nombre;
                            cotCoberFila3["nFormaPago"] = "3";
                            cotCoberFila3["desFormaPago"] = "Trimestral";
                            cotCoberFila3["PagoIni"] = ModeloGenerico.calculos[pvtCalculos].mPago1de;
                            cotCoberFila3["PagoSub"] = "cuatro pagos de  " +
                                                                    ModeloGenerico.calculos[pvtCalculos].mPago1de;
                            cotCoberFila3["Total"] = ModeloGenerico.calculos[pvtCalculos].mTotal;
                            ModeloGenerico.tblCotizacionesCober.Rows.Add(cotCoberFila3);
                            /**********************************************************************************/
                            pvtCalculos++;
                            fila["Mensual"] = "<input name='savecoti' value='" + CalculoCober.OrdenCobertura + "|4' type='radio'" + (CalculoCober.OrdenCobertura == 1 ? "checked='checked'" : "") + "> <strong>" +
                                                    string.Format("{0:C2}", ModeloGenerico.calculos[pvtCalculos].mTotal) + "</strong></br> primer pago  de " +
                                                    string.Format("{0:C2}", ModeloGenerico.calculos[pvtCalculos].mPago1de) + " y " +
                                                    ModeloGenerico.calculos[pvtCalculos].nPago2de + " pagos de  " +
                                                    string.Format("{0:C2}", ModeloGenerico.calculos[pvtCalculos].mPago2de);
                            /**********************************************************************************/
                            DataRow cotCoberFila4 = ModeloGenerico.tblCotizacionesCober.NewRow();
                            cotCoberFila4["nOrdenCobertura"] = CalculoCober.OrdenCobertura;
                            cotCoberFila4["Cobertura"] = CalculoCober.Nombre;
                            cotCoberFila4["nFormaPago"] = "4";
                            cotCoberFila4["desFormaPago"] = "Mensual";
                            cotCoberFila4["PagoIni"] = ModeloGenerico.calculos[pvtCalculos].mPago1de;
                            cotCoberFila4["PagoSub"] = ModeloGenerico.calculos[pvtCalculos].nPago2de + " pagos de  " +
                                                                ModeloGenerico.calculos[pvtCalculos].mPago2de;
                            cotCoberFila4["Total"] = ModeloGenerico.calculos[pvtCalculos].mTotal;
                            ModeloGenerico.tblCotizacionesCober.Rows.Add(cotCoberFila4);
                            /**************************************************************************************/
                            tblCoberturas.Rows.Add(fila);
                        }
                        ModeloGenerico.ObjCalculo = ModeloGenerico.calculos[1];
                        //var responsa                                        = VarProcInterfazX.arrAgentes.Where(a => a.lngAgente == long.Parse(agentID)).First().cresponsa;
                        //ModeloGenerico.responsable              = responsa == "" ? "XX" : responsa;
                        ModeloGenerico.agente = 0;
                        ModeloGenerico.formaPago = 4;
                        ModeloGenerico.complemento.primaNeta = ModeloGenerico.ObjCalculo.primaNeta;
                        ModeloGenerico.complemento.interes = "COB";
                        CoberturasModel cbrSeleccionada = CotizaExpressModel.coberturas[0];
                        Dictionary<string, object> resultado = new Dictionary<string, object>();

                        ModeloGenerico.cEra = VarProcInterfazX.arrPago[1].strEra;

                        ModeloGenerico.vehiculos = ModeloGenerico.calculos[0].vehiculos;

                        ModeloGenerico.vehiculos[0].ExcenDedu = cbrSeleccionada.ExDedu;
                        ModeloGenerico.vehiculos[0].coberGtoMed = cbrSeleccionada.GM;
                        ModeloGenerico.vehiculos[0].deducDañMat = cbrSeleccionada.DM;
                        ModeloGenerico.vehiculos[0].deducRobTot = cbrSeleccionada.RT;
                        ModeloGenerico.vehiculos[0].cober100 = cbrSeleccionada.Cober100;
                        ModeloGenerico.tersa = cbrSeleccionada.Terssa == "N" ? 0 : 1;
                        ModeloGenerico.vehiculos[0].estacionaCasa = cbrSeleccionada.GarageCasa;
                        ModeloGenerico.vehiculos[0].estacionaTrab = cbrSeleccionada.GarageTrabajo;
                        ModeloGenerico.vehiculos[0].usoTrabajo = cbrSeleccionada.Usotrabajo;
                        ModeloGenerico.vehiculos[0].coberRepCiv = cbrSeleccionada.RC;
                        ModeloGenerico.vehiculos[0].coberRepCivCat = cbrSeleccionada.RCcatastrofica;
                        ModeloGenerico.vehiculos[0].proliber = cbrSeleccionada.AseLegal;
                        ModeloGenerico.vehiculos[0].sumAsegFija = cbrSeleccionada.SumaAseg;
                        ModeloGenerico.vehiculos[0].vehSus = cbrSeleccionada.VehSus;
                        ModeloGenerico.vehiculos[0].asistenciaViaje = cbrSeleccionada.Aviaje;
                        ModeloGenerico.vehiculos[0].EsConductorBajoRiesgo = cbrSeleccionada.BajoRiesgo;
                        ModeloGenerico.conductorRestringido = cbrSeleccionada.BajoRiesgo == "S" ? 0 : 1;
                        ModeloGenerico.masAutos = cbrSeleccionada.MasAutos;
                        ModeloGenerico.control = cbrSeleccionada.Procar;

                        ModeloGenerico.conductores[0].extRespCivil = cbrSeleccionada.ExtensionRC == "S" ? 1 : 0;
                        ModeloGenerico.cobertura100 = cbrSeleccionada.Cober100 == "S" ? 1 : 0;

                        ModeloGenerico.conductores[0].DescripcionExRespCivil = ModeloGenerico.conductores[0].extRespCivil == 1 ? "Sí" : "No";
                        ModeloGenerico.estatus = "H";
                        ModeloGenerico.cVendida = "I";
                        ModeloGenerico.responsable = "XX";
                        ModeloGenerico.fechaCotizacion = DateTime.Now;
                        ModeloGenerico.inicioVigencia = DateTime.Now.ToShortDateString();
                        ModeloGenerico.finVigencia = DateTime.Now.AddYears(1).ToShortDateString();
                        ModeloGenerico.persona = "F";
                        ModeloGenerico.contraEntrega = "";
                        ModeloGenerico.entregaPol = "N";
                        ModeloGenerico.entregaComen = " ";
                        ModeloGenerico.campaña = 1549;
                        ModeloGenerico.subRamo = "1";
                        ModeloGenerico.ramo = 1;
                        ModeloGenerico.comentariosV = "Cotización generada en línea por cotizador exprés";
                        ModeloGenerico.observaciones = "";
                        CotizaExpressModel.CotComp = CentralAgentesMvc.App_Start.CastObject.Cast<ClsCotizacion>(ModeloGenerico);
                        resultado = CentralAgentesMvc.App_Start.CastObject.Cast<ClsCotizacion>(ModeloGenerico).GuardaCotizacionWeb(CentralAgentesMvc.App_Start.CastObject.Cast<ClsCotizacion>(ModeloGenerico), "A", "");
                        ModeloGenerico.cotizacionID = int.Parse(resultado["ID"].ToString());
                        ModeloGenerico.ModulodeTrabajo = "Cotizaciones";
                        ViewBag.idCotizacion = ModeloGenerico.cotizacionID;
                        //CotizaExpressModel.CotComp = CentralAgentesMvc.App_Start.CastObject.Cast<ClsCotizacion>(ModeloGenerico);

                        break;
                    #endregion

                    #region Agentes
                    case "Agentes":
                        ModeloGenerico.vehiculos[0].sumAsegFija = model.vehiculo.sumAsegFija.Substring(0, 1) == "V" ? "F" : model.vehiculo.sumAsegFija.Substring(0, 1);
                        CotizaExpressModel.coberturas = new List<CoberturasModel>();
                        pvtCalculos = -1;
                        foreach (DataRow cobertura in TpCober.Tables[0].Rows)
                        {
                            CoberturasModel CalculoCober = new CoberturasModel();
                            CalculoCober.OrdenCobertura = int.Parse(cobertura["OrdenCobertura"].ToString());
                            CalculoCober.Nombre = cobertura["Nombre"].ToString();
                            CalculoCober.Descripcion = cobertura["Descripcion"].ToString();

                            ModeloGenerico.vehiculos[0].ExcenDedu = CalculoCober.ExDedu = CalculoCober.ExDedu = cobertura["ExenDeducible"].ToString();
                            ModeloGenerico.vehiculos[0].coberGtoMed = CalculoCober.GM = int.Parse(cobertura["GastosMedicos"].ToString());
                            ModeloGenerico.vehiculos[0].deducDañMat = CalculoCober.DM = int.Parse(cobertura["DM"].ToString());
                            ModeloGenerico.vehiculos[0].deducRobTot = CalculoCober.RT = int.Parse(cobertura["RT"].ToString());
                            ModeloGenerico.vehiculos[0].cober100 = CalculoCober.Cober100 = cobertura["Cober100"].ToString();
                            ModeloGenerico.vehiculos[0].estacionaTrab = CalculoCober.GarageTrabajo = cobertura["GarageTrabajo"].ToString();
                            ModeloGenerico.vehiculos[0].estacionaCasa = CalculoCober.GarageCasa = cobertura["GarageCasa"].ToString();
                            //ModeloGenerico.vehiculos[0].usoTrabajo = CalculoCober.Usotrabajo = cobertura["Usotrabajo"].ToString();
                            ModeloGenerico.masAutos = CalculoCober.MasAutos = cobertura["MasAutos"].ToString();
                            CalculoCober.Procar = int.Parse(cobertura["Procar"].ToString());
                            ModeloGenerico.cobertura100 = CalculoCober.Cober100 == "S" ? 1 : 0;
                            if (ModeloGenerico.cobertura100 == 0)
                            {
                                ModeloGenerico.conductores[0].extRespCivil = 0;
                                ModeloGenerico.vehiculos[0].cober100 = "N";
                            }
                            ValidaParaCotizara(ModeloGenerico);
                            Calculo(ref ModeloGenerico, modulo);
                            CotizaExpressModel.coberturas.Add(CalculoCober);
                            DataRow fila = tblCoberturas.NewRow();
                            fila["TipoCobertura"] = "<strong>" + CalculoCober.Nombre + "</strong>" + "</br>" + CalculoCober.Descripcion;

                            pvtCalculos++;
                            fila["Contado"] = "<input name='savecoti' value='" + CalculoCober.OrdenCobertura + "|1' type='radio'> <strong>" +
                                                    string.Format("{0:C2}", ModeloGenerico.calculos[pvtCalculos].mTotal) + "</strong> </br> Pago único";
                            pvtCalculos++;
                            fila["Semestral"] = "<input name='savecoti' value='" + CalculoCober.OrdenCobertura + "|2' type='radio'> <strong>" +
                                                    string.Format("{0:C2}", ModeloGenerico.calculos[pvtCalculos].mTotal) + "</strong></br> primer pago de " +
                                                    string.Format("{0:C2}", ModeloGenerico.calculos[pvtCalculos].mPago1de) + " y " +
                                                    ModeloGenerico.calculos[pvtCalculos].nPago2de + " pagos de " +
                                                    string.Format("{0:C2}", ModeloGenerico.calculos[pvtCalculos].mPago2de);
                            pvtCalculos++;
                            fila["Trimestral"] = "<input name='savecoti' value='" + CalculoCober.OrdenCobertura + "|3' type='radio'> <strong>" +
                                                    string.Format("{0:C2}", ModeloGenerico.calculos[pvtCalculos].mTotal) + "</strong></br> cuatro pagos de  " +
                                                    string.Format("{0:C2}", ModeloGenerico.calculos[pvtCalculos].mPago1de);
                            pvtCalculos++;
                            fila["Mensual"] = "<input name='savecoti' value='" + CalculoCober.OrdenCobertura + "|4' type='radio'> <strong>" +
                                                    string.Format("{0:C2}", ModeloGenerico.calculos[pvtCalculos].mTotal) + "</strong></br> primer pago  de " +
                                                    string.Format("{0:C2}", ModeloGenerico.calculos[pvtCalculos].mPago1de) + " y " +
                                                    ModeloGenerico.calculos[pvtCalculos].nPago2de + " pagos de  " +
                                                    string.Format("{0:C2}", ModeloGenerico.calculos[pvtCalculos].mPago2de);
                            tblCoberturas.Rows.Add(fila);
                        }
                        break;
                    #endregion

                    #region Servidores
                    case "Servidores":
                        modulo = "C";


                        CotizaExpressModel.coberturas = new List<CoberturasModel>();
                        pvtCalculos = -1;
                        foreach (DataRow cobertura in TpCober.Tables[0].Rows)
                        {
                            CoberturasModel CalculoCober = new CoberturasModel();
                            CalculoCober.OrdenCobertura = int.Parse(cobertura["OrdenCobertura"].ToString());
                            CalculoCober.Nombre = cobertura["Nombre"].ToString();
                            CalculoCober.Descripcion = cobertura["Descripcion"].ToString();
                            ModeloGenerico.vehiculos[0].cober100 = CalculoCober.Cober100 = cobertura["Cober100"].ToString();
                            ModeloGenerico.vehiculos[0].deducDañMat = CalculoCober.DM = int.Parse(cobertura["DM"].ToString());
                            model.cotizacion.vehiculos[0].deducRobTot = CalculoCober.RT = int.Parse(cobertura["RT"].ToString());
                            CalculoCober.Terssa = cobertura["Terssa"].ToString();
                            CalculoCober.ExtensionRC = cobertura["ExtensionRC"].ToString();
                            Calculo(ref ModeloGenerico, modulo);
                            CotizaExpressModel.coberturas.Add(CalculoCober);
                            DataRow fila = tblCoberturas.NewRow();
                            fila["TipoCobertura"] = "<strong>" + CalculoCober.Nombre + "</strong>" + "</br>" + CalculoCober.Descripcion;

                            pvtCalculos++;
                            fila["Contado"] = "<input name='savecoti' value='" + CalculoCober.OrdenCobertura + "|1' type='radio'> <strong>" +
                                                    ModeloGenerico.calculos[pvtCalculos].mTotal + "</strong> </br>  Pago único";
                            pvtCalculos++;
                            fila["Semestral"] = "<input name='savecoti' value='" + CalculoCober.OrdenCobertura + "|2' type='radio'> <strong>" +
                                                    ModeloGenerico.calculos[pvtCalculos].mTotal + "</strong></br> primer pago de " +
                                                    ModeloGenerico.calculos[pvtCalculos].mPago1de + " y " +
                                                    ModeloGenerico.calculos[pvtCalculos].nPago2de + " pagos de " +
                                                    ModeloGenerico.calculos[pvtCalculos].mPago2de;
                            pvtCalculos++;
                            fila["Trimestral"] = "<input name='savecoti' value='" + CalculoCober.OrdenCobertura + "|3' type='radio'> <strong>" +
                                                    ModeloGenerico.calculos[pvtCalculos].mTotal + "</strong></br> cuatro pagos de  " +
                                                    ModeloGenerico.calculos[pvtCalculos].mPago1de;
                            pvtCalculos++;
                            fila["Mensual"] = "<input name='savecoti' value='" + CalculoCober.OrdenCobertura + "|4' type='radio'> <strong>" +
                                                    ModeloGenerico.calculos[pvtCalculos].mTotal + "</strong></br> primer pago  de " +
                                                    ModeloGenerico.calculos[pvtCalculos].mPago1de + " y " +
                                                    ModeloGenerico.calculos[pvtCalculos].nPago2de + " pagos de  " +
                                                    ModeloGenerico.calculos[pvtCalculos].mPago2de;
                            tblCoberturas.Rows.Add(fila);
                        }
                        break;
                    #endregion

                    #region Prestadinero
                    case "Prestadinero":
                        modulo = "C";


                        CotizaExpressModel.coberturas = new List<CoberturasModel>();
                        pvtCalculos = -1;
                        foreach (DataRow cobertura in TpCober.Tables[0].Rows)
                        {
                            CoberturasModel CalculoCober = new CoberturasModel();
                            CalculoCober.OrdenCobertura = int.Parse(cobertura["OrdenCobertura"].ToString());
                            CalculoCober.Nombre = cobertura["Nombre"].ToString();
                            CalculoCober.Descripcion = cobertura["Descripcion"].ToString();
                            ModeloGenerico.vehiculos[0].cober100 = CalculoCober.Cober100 = cobertura["Cober100"].ToString();
                            ModeloGenerico.vehiculos[0].deducDañMat = CalculoCober.DM = int.Parse(cobertura["DM"].ToString());
                            model.cotizacion.vehiculos[0].deducRobTot = CalculoCober.RT = int.Parse(cobertura["RT"].ToString());
                            CalculoCober.Terssa = cobertura["Terssa"].ToString();
                            CalculoCober.ExtensionRC = cobertura["ExtensionRC"].ToString();
                            Calculo(ref ModeloGenerico, modulo);
                            CotizaExpressModel.coberturas.Add(CalculoCober);
                            DataRow fila = tblCoberturas.NewRow();
                            fila["TipoCobertura"] = "<strong>" + CalculoCober.Nombre + "</strong>" + "</br>" + CalculoCober.Descripcion;

                            pvtCalculos++;
                            fila["Contado"] = "<input name='savecoti' value='" + CalculoCober.OrdenCobertura + "|1' type='radio'> <strong>" +
                                                    ModeloGenerico.calculos[pvtCalculos].mTotal + "</strong> </br>  Pago único";
                            pvtCalculos++;
                            fila["Semestral"] = "<input name='savecoti' value='" + CalculoCober.OrdenCobertura + "|2' type='radio'> <strong>" +
                                                    ModeloGenerico.calculos[pvtCalculos].mTotal + "</strong></br> primer pago de " +
                                                    ModeloGenerico.calculos[pvtCalculos].mPago1de + " y " +
                                                    ModeloGenerico.calculos[pvtCalculos].nPago2de + " pagos de " +
                                                    ModeloGenerico.calculos[pvtCalculos].mPago2de;
                            pvtCalculos++;
                            fila["Trimestral"] = "<input name='savecoti' value='" + CalculoCober.OrdenCobertura + "|3' type='radio'> <strong>" +
                                                    ModeloGenerico.calculos[pvtCalculos].mTotal + "</strong></br> cuatro pagos de  " +
                                                    ModeloGenerico.calculos[pvtCalculos].mPago1de;
                            pvtCalculos++;
                            fila["Mensual"] = "<input name='savecoti' value='" + CalculoCober.OrdenCobertura + "|4' type='radio'> <strong>" +
                                                    ModeloGenerico.calculos[pvtCalculos].mTotal + "</strong></br> primer pago  de " +
                                                    ModeloGenerico.calculos[pvtCalculos].mPago1de + " y " +
                                                    ModeloGenerico.calculos[pvtCalculos].nPago2de + " pagos de  " +
                                                    ModeloGenerico.calculos[pvtCalculos].mPago2de;
                            tblCoberturas.Rows.Add(fila);
                        }
                        break;
                    #endregion
                }

                Session["ModeloEnUso"] = ModeloGenerico;
                CotizaExpressModel.CotComp = new ClsCotizacion();
                CotizaExpressModel.CotComp = CentralAgentesMvc.App_Start.CastObject.Cast<ClsCotizacion>(ModeloGenerico);

                int totalRows = 0;

                var data = tblCoberturas;

                ClearErrors();
                return Json(from g in data.Select()
                            select new
                            {
                                //chkSelect = false,
                                TipoCobertura = g["TipoCobertura"],
                                Contado = g["Contado"],
                                Semestral = g["Semestral"],
                                Trimestral = g["Trimestral"],
                                Mensual = g["Mensual"],
                                IdCoti = "Cotización No: " + ModeloGenerico.cotizacionID,
                                total = totalRows,
                            }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception err)
            {
                Response.StatusCode = (int)(System.Net.HttpStatusCode.InternalServerError);
                LogDeErrores.RegistroErrorWeb(err, DatoUsuario.idAgente, DatoUsuario.nomAgente, DatoUsuario.sIp, DatoUsuario.sHostName);
                return Json("ErrInterno");
            }
        }

        private void ValidaParaCotizara(GenericoViewModel modeloEvalua)
        {
            //VldVeh.validaMarca(modeloEvalua.vehiculos[0].modelo, modeloEvalua.vehiculos[0].DescripcionTipo);
            //ValidaVehiculos(modeloEvalua.tipo);
            //ValidaVehiculos(modeloEvalua.formaPago.ToString());
            //modeloEvalua.vehiculos[0].ValidaConductorMenorDe25(
        }

        [HttpPost]
        public JsonResult GuardaCotUsuario(CotizaExpressModel model)
        {
            try
            {
                int posicio = 0;
                switch (int.Parse(model.radio.Split('|')[0]))
                {
                    case 1:
                        posicio = -1 + int.Parse(model.radio.Split('|')[1]);
                        break;
                    case 2:
                        posicio = 3 + int.Parse(model.radio.Split('|')[1]);
                        break;
                    case 3:
                        posicio = 7 + int.Parse(model.radio.Split('|')[1]);
                        break;
                }
                CotizaExpressModel.CotComp.ObjCalculo = CotizaExpressModel.CotComp.calculos[posicio];

                return Json(model, JsonRequestBehavior.AllowGet);
            }
            catch (Exception err)
            {
                Response.StatusCode = (int)(System.Net.HttpStatusCode.InternalServerError);
                LogDeErrores.RegistroErrorWeb(err, DatoUsuario.idAgente, DatoUsuario.nomAgente, DatoUsuario.sIp, DatoUsuario.sHostName);
                return Json("ErrInterno");
            }
        }

        [HttpPost]
        public JsonResult GuardaCotExpress(CotizaExpressModel model)
        {
            try
            {
                //var logged = (DataSet)Session["UserObj"];
                var agentID = DatoUsuario.idAgente;
                int posicio = 0;
                var cot = (GenericoViewModel)Session["ModeloEnUso"];
                switch (int.Parse(model.radio.Split('|')[0]))
                {
                    case 1:
                        posicio = -1 + int.Parse(model.radio.Split('|')[1]);
                        break;
                    case 2:
                        posicio = 3 + int.Parse(model.radio.Split('|')[1]);
                        break;
                    case 3:
                        posicio = 7 + int.Parse(model.radio.Split('|')[1]);
                        break;
                }
                CotizaExpressModel.CotComp = CentralAgentesMvc.App_Start.CastObject.Cast<ClsCotizacion>(cot);
                CotizaExpressModel.CotComp.ObjCalculo = CotizaExpressModel.CotComp.calculos[posicio];
                CotizaExpressModel.CotComp.vehiculos = CotizaExpressModel.CotComp.calculos[posicio].vehiculos;

                CotizaExpressModel.CotComp.vehiculos[0].validaSerie = "S";
                var responsa = VarProcInterfazX.arrAgentes.Where(a => a.lngAgente == long.Parse(agentID)).First().cresponsa;

                CotizaExpressModel.CotComp.agente = int.Parse(agentID);
                CotizaExpressModel.CotComp.formaPago = int.Parse(model.radio.Split('|')[1]);
                CotizaExpressModel.CotComp.complemento.primaNeta = CotizaExpressModel.CotComp.ObjCalculo.primaNeta;
                CotizaExpressModel.CotComp.complemento.interes = "COB";
                CoberturasModel cbrSeleccionada = CotizaExpressModel.coberturas[int.Parse(model.radio.Split('|')[0]) - 1];
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                CotizaExpressModel.CotComp.cEra = VarProcInterfazX.arrPago[int.Parse(model.radio.Split('|')[1]) - 1].strEra;

                switch (model.OrigenCotExp)
                {
                    #region Clientes
                    case "Clientes":

                        CotizaExpressModel.CotComp.responsable = CotizaExpressModel.CotComp.obtenerResponsable(CotizaExpressModel.CotComp.cotizacionID);
                        CotizaExpressModel.CotComp.vehiculos[0].ExcenDedu = cbrSeleccionada.ExDedu;
                        CotizaExpressModel.CotComp.vehiculos[0].coberGtoMed = cbrSeleccionada.GM;
                        CotizaExpressModel.CotComp.vehiculos[0].deducDañMat = cbrSeleccionada.DM;
                        CotizaExpressModel.CotComp.vehiculos[0].deducRobTot = cbrSeleccionada.RT;
                        CotizaExpressModel.CotComp.vehiculos[0].cober100 = cbrSeleccionada.Cober100;
                        CotizaExpressModel.CotComp.tersa = cbrSeleccionada.Terssa == "N" ? 0 : 1;
                        CotizaExpressModel.CotComp.vehiculos[0].ExcenDedu = cbrSeleccionada.ExtensionRC;
                        CotizaExpressModel.CotComp.vehiculos[0].estacionaCasa = cbrSeleccionada.GarageCasa;
                        CotizaExpressModel.CotComp.vehiculos[0].estacionaTrab = cbrSeleccionada.GarageTrabajo;
                        CotizaExpressModel.CotComp.vehiculos[0].usoTrabajo = cbrSeleccionada.Usotrabajo;
                        CotizaExpressModel.CotComp.vehiculos[0].coberRepCiv = cbrSeleccionada.RC;
                        CotizaExpressModel.CotComp.vehiculos[0].coberRepCivCat = cbrSeleccionada.RCcatastrofica;
                        CotizaExpressModel.CotComp.vehiculos[0].proliber = cbrSeleccionada.AseLegal;
                        CotizaExpressModel.CotComp.vehiculos[0].sumAsegFija = cbrSeleccionada.SumaAseg;
                        CotizaExpressModel.CotComp.vehiculos[0].vehSus = cbrSeleccionada.VehSus;
                        CotizaExpressModel.CotComp.vehiculos[0].asistenciaViaje = cbrSeleccionada.Aviaje;
                        CotizaExpressModel.CotComp.vehiculos[0].EsConductorBajoRiesgo = cbrSeleccionada.BajoRiesgo;
                        CotizaExpressModel.CotComp.conductorRestringido = cbrSeleccionada.BajoRiesgo == "S" ? 0 : 1;


                        CotizaExpressModel.CotComp.vehiculos[0].SUVA = "";
                        CotizaExpressModel.CotComp.vehiculos[0].autorizaSerie = "";
                        CotizaExpressModel.CotComp.vehiculos[0].placas = "";
                        CotizaExpressModel.CotComp.vehiculos[0].tpoAlarma = "";
                        CotizaExpressModel.CotComp.vehiculos[0].descEqEsp = "";
                        CotizaExpressModel.CotComp.vehiculos[0].ccNCI = "";
                        CotizaExpressModel.CotComp.vehiculos[0].fecEBC = "";
                        CotizaExpressModel.CotComp.vehiculos[0].numeroSerie = "";
                        CotizaExpressModel.CotComp.vehiculos[0].pagEBC = "";
                        CotizaExpressModel.CotComp.vehiculos[0].VIN = CotizaExpressModel.CotComp.vehiculos[0].VIN == null ? "" : CotizaExpressModel.CotComp.vehiculos[0].VIN;

                        CotizaExpressModel.CotComp.vehiculos[0].finVigencia = CotizaExpressModel.CotComp.finVigencia;
                        CotizaExpressModel.CotComp.vehiculos[0].inicioVigencia = CotizaExpressModel.CotComp.inicioVigencia;
                        CotizaExpressModel.CotComp.masAutos = cbrSeleccionada.MasAutos;
                        CotizaExpressModel.CotComp.vehiculos[0].procarinicial = cbrSeleccionada.Procar;
                        CotizaExpressModel.CotComp.conductores[0].extRespCivil = cbrSeleccionada.ExtensionRC == "S" ? 1 : 0;
                        CotizaExpressModel.CotComp.cobertura100 = cbrSeleccionada.Cober100 == "S" ? 1 : 0;
                        CotizaExpressModel.CotComp.emailOtro = "";
                        CotizaExpressModel.CotComp.RFC = "";
                        CotizaExpressModel.CotComp.obsTarj = "";
                        CotizaExpressModel.CotComp.nSolicitud = "";
                        CotizaExpressModel.CotComp.nSolicitud_Ant = "";
                        CotizaExpressModel.CotComp.autorizaProcar = "";
                        CotizaExpressModel.CotComp.cFinanciamiento = "";
                        CotizaExpressModel.CotComp.cNombreFinanciamiento = "";
                        CotizaExpressModel.CotComp.cEndosoCancela = "";
                        CotizaExpressModel.CotComp.recomienda = "";
                        CotizaExpressModel.CotComp.telefono[0].cOficina = "";
                        CotizaExpressModel.CotComp.telefono[0].cExtension = "";
                        CotizaExpressModel.CotComp.telefono[0].cFax = "";
                        CotizaExpressModel.CotComp.telefono[0].cRadio = "";
                        CotizaExpressModel.CotComp.telefono[0].cRadioID = "";
                        CotizaExpressModel.CotComp.telefono[0].cOtro1 = "";

                        if (CotizaExpressModel.CotComp.Update(CotizaExpressModel.CotComp, "", VarProcAMC.strPeriodo, "", CotizaExpressModel.CotComp.ObjCalculo.mPago1de, CotizaExpressModel.CotComp.ObjCalculo.mPago2de, CotizaExpressModel.CotComp.ObjCalculo.mTotal))
                        {
                            resultado["ID"] = CotizaExpressModel.CotComp.cotizacionID;
                            //Negocio.ReportesCrystal.CrReportCentral cr = new Negocio.ReportesCrystal.CrReportCentral();
                            //cr.GeneraCotizacionWebPDF(resultado["ID"].ToString());
                            var gm = CentralAgentesMvc.App_Start.CastObject.Cast<GenericoViewModel>(CotizaExpressModel.CotComp);
                            gm.ModulodeTrabajo = "Cotizaciones";
                            Session["ModeloEnUso"] = gm;
                            // Guardo la cobertura seleccionada
                            Session["SelectedCoberExpress"] = cbrSeleccionada;
                        }
                        break;
                    #endregion

                    #region Agentes
                    case "Agentes":
                        CotizaExpressModel.CotComp.estatus = "P";
                        CotizaExpressModel.CotComp.responsable = CotizaExpressModel.CotComp.ResponsableAgentes(int.Parse(agentID.ToString()));
                        CotizaExpressModel.CotComp.vehiculos[0].deducDañMat = cbrSeleccionada.DM;
                        CotizaExpressModel.CotComp.vehiculos[0].deducRobTot = cbrSeleccionada.RT;
                        CotizaExpressModel.CotComp.vehiculos[0].cober100 = cbrSeleccionada.Cober100;
                        CotizaExpressModel.CotComp.conductores[0].extRespCivil = cbrSeleccionada.Cober100 == "S" ? 1 : 0;
                        CotizaExpressModel.CotComp.conductores[0].DescripcionExRespCivil = CotizaExpressModel.CotComp.conductores[0].extRespCivil == 1 ? "Sí" : "No";
                        CotizaExpressModel.CotComp.vehiculos[0].estacionaCasa = cbrSeleccionada.GarageCasa;
                        CotizaExpressModel.CotComp.vehiculos[0].estacionaTrab = cbrSeleccionada.GarageTrabajo;
                        //CotizaExpressModel.CotComp.vehiculos[0].ExcenDedu                 = cbrSeleccionada.ExtensionRC;
                        //CotizaExpressModel.CotComp.vehiculos[0].usoTrabajo                = cbrSeleccionada.Usotrabajo;
                        //CotizaExpressModel.CotComp.vehiculos[0].mcurExtRC                 = cbrSeleccionada.RC;
                        //CotizaExpressModel.CotComp.vehiculos[0].coberRepCivCat            = cbrSeleccionada.RCcatastrofica;
                        //CotizaExpressModel.CotComp.vehiculos[0].proliber                  = cbrSeleccionada.AseLegal;
                        //CotizaExpressModel.CotComp.vehiculos[0].vehSus                    = cbrSeleccionada.VehSus;
                        //CotizaExpressModel.CotComp.vehiculos[0].asistenciaViaje           = cbrSeleccionada.Aviaje;
                        //CotizaExpressModel.CotComp.vehiculos[0].conductorRestringido      = cbrSeleccionada.VehSus;
                        CotizaExpressModel.CotComp.masAutos = cbrSeleccionada.MasAutos;
                        CotizaExpressModel.CotComp.cobertura100 = cbrSeleccionada.Cober100 == "S" ? 1 : 0;

                        CotizaExpressModel.CotComp.fechaCotizacion = DateTime.Now;

                        resultado = CotizaExpressModel.CotComp.GuardaCotizacionWeb(CotizaExpressModel.CotComp, "A", agentID);
                        //resultado = CotizaExpressModel.CotComp.GuardaCotizacionWeb(CotizaExpressModel.CotComp, "A", "");
                        break;
                    #endregion

                    #region Servidores
                    case "Servidores":

                        break;
                    #endregion

                    #region Prestadinero
                    case "Prestadinero":

                        break;
                    #endregion
                }
                return Json(resultado["ID"].ToString(), JsonRequestBehavior.AllowGet);
            }
            catch (Exception err)
            {
                Response.StatusCode = (int)(System.Net.HttpStatusCode.InternalServerError);
                LogDeErrores.RegistroErrorWeb(err, DatoUsuario.idAgente, DatoUsuario.nomAgente, DatoUsuario.sIp, DatoUsuario.sHostName);
                return Json("ErrInterno");
            }
        }

        /// <summary>
        /// Método que devuelve las cotizaciones del agente conectado. Si es administrador
        /// devolverá las cotizaciones del agente que se seleccione del combo
        /// </summary>
        /// <param name="agenteID">ID del agente</param>
        /// <param name="periodoID">Año del periodo</param>
        /// <param name="periodoID">Mes del periodo</param>
        /// <param name="offset">Número de página de datos a devolver</param>
        /// <param name="limit">Cantidad de registros a devolver</param>
        /// <returns>Objeto Json con los resultados de la consulta</returns>
        public JsonResult GetCotizaciones(string agenteID, string periodoID, string periodoMesID, int offset, int limit)
        {
            try
            {
                if (string.IsNullOrEmpty(agenteID)) agenteID = "0";
                if (string.IsNullOrEmpty(periodoID)) periodoID = "0";
                if (string.IsNullOrEmpty(periodoMesID)) periodoMesID = "0";
                if (offset == 0) offset = 1;
                if (limit == 0) limit = 10;

                var data = ctzc.GetCotizacionesByAgente(agenteID, periodoID, periodoMesID, offset, limit);
                var totalRows = Convert.ToInt32(data.Tables["Pager"].Rows[0]["RecordCount"]);

                return Json(from g in data.Tables[0].Select()
                            select new
                            {
                                chkSelect = false,
                                nCotizaID = g["nCotizaID"],
                                Cliente = g["Cliente"],
                                InicioVigencia = String.Format("{0:dd/MM/yyyy}", g["InicioVigencia"]),
                                Telefono = g["Telefono"],
                                Observaciones = g["Observaciones"],
                                FechaRegistro = String.Format("{0:dd/MM/yyyy}", g["FechaRegistro"]),
                                SolicitudID = g["nSolicitudId"],
                                EstatusID = g["cEstatus"],
                                DescripEstatus = g["DescripcionEstatus"],
                                FechaExpedicion = String.Format("{0:dd/MM/yyyy}", g["FechaExpedicion"]),
                                total = totalRows,
                            }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception err)
            {
                Response.StatusCode = (int)(System.Net.HttpStatusCode.InternalServerError);
                LogDeErrores.RegistroErrorWeb(err, DatoUsuario.idAgente, DatoUsuario.nomAgente, DatoUsuario.sIp, DatoUsuario.sHostName);
                return Json("ErrInterno",JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Método que devuelve las renovaciones del agente conectado. Si es administrador
        /// devolverá las renovaciones del agente que se seleccione del combo
        /// </summary>
        /// <param name="agenteID">ID del agente</param>
        /// <param name="periodoID">Año del periodo</param>
        /// <param name="offset">Número de página de datos a devolver</param>
        /// <param name="limit">Cantidad de registros a devolver</param>
        /// <returns>Objeto Json con los resultados de la consulta</returns>
        public JsonResult GetRenovaciones(string agenteID, string periodoID, string periodoMesID, int offset, int limit)
        {
            try
            {
                if (string.IsNullOrEmpty(agenteID)) agenteID = "0";
                if (string.IsNullOrEmpty(periodoID)) periodoID = "0";
                if (string.IsNullOrEmpty(periodoMesID)) periodoMesID = "0";
                if (offset == 0) offset = 1;
                if (limit == 0) limit = 10;

                var data = rnvc.GetRenovacionesByAgente(agenteID, periodoID, periodoMesID, offset, limit);
                var totalRows = Convert.ToInt32(data.Tables["Pager"].Rows[0]["RecordCount"]);

                return Json(from g in data.Tables[0].Select()
                            select new
                            {
                                chkSelect = false,
                                nPolizaID = g["nPoliza"],
                                Cliente = g["Cliente"],
                                NombreAsegurado = g["Nombre"],
                                InicioVigencia = String.Format("{0:dd/MM/yyyy}", g["InicioVigencia"]),
                                FinVigencia = String.Format("{0:dd/MM/yyyy}", g["FinVigencia"]),
                                Observaciones = g["Observaciones"],
                                Emision = String.Format("{0:dd/MM/yyyy}", g["FechaExpedicion"]),
                                NumVehiculos = g["nNumVehi"],
                                NumConductores = g["nNumCond"],
                                ConductorRestr = g["cCondRest"],
                                Ejecutivo = g["cResponsa"],
                                CampainID = g["nCampID"],
                                ClienteID = g["nClienteID"],
                                EstatusID = g["nEstatusID"],
                                DescripEstatus = g["DescripcionEstatus"],
                                total = totalRows,
                                era = g["cEra"].ToString()
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
        /// Método usado para buscar información de cotizaciones por Número o Nombre del Cliente
        /// </summary>
        /// <param name="agenteID">ID del agente</param>
        /// <param name="offset">Número de página de datos a devolver</param>
        /// <param name="limit">Cantidad de registros a devolver</param>
        /// <param name="search">Texto a Buscar</param>
        /// <returns>Objeto Json con los resultados de la consulta</returns>
        public JsonResult SearchCotizaciones(string agenteID, int offset, int limit, string search)
        {
            try
            {
                if (string.IsNullOrEmpty(agenteID)) agenteID = "0";
                if (string.IsNullOrEmpty(search)) search = "";
                if (offset == 0) offset = 1;
                if (limit == 0) limit = 10;

                var data = ctzc.SearchCotizacionesByAgente(agenteID, offset, limit, search);
                var totalRows = Convert.ToInt32(data.Tables["Pager"].Rows[0]["RecordCount"]);

                return Json(from g in data.Tables[0].Select()
                            select new
                            {
                                chkSelect = false,
                                nCotizaID = g["nCotizaID"],
                                Cliente = g["Cliente"],
                                InicioVigencia = String.Format("{0:dd/MM/yyyy}", g["InicioVigencia"]),
                                Telefono = g["Telefono"],
                                Observaciones = g["Observaciones"],
                                FechaRegistro = String.Format("{0:dd/MM/yyyy}", g["FechaRegistro"]),
                                SolicitudID = g["nSolicitudId"],
                                EstatusID = g["cEstatus"],
                                DescripEstatus = g["DescripcionEstatus"],
                                FechaExpedicion = String.Format("{0:dd/MM/yyyy}", g["FechaExpedicion"]),
                                total = totalRows,
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
        /// Método usado para buscar información de renovaciones por Número o Nombre del Cliente
        /// </summary>
        /// <param name="agenteID">ID del agente</param>
        /// <param name="offset">Número de página de datos a devolver</param>
        /// <param name="limit">Cantidad de registros a devolver</param>
        /// <param name="search">Texto a Buscar</param>
        /// <returns>Objeto Json con los resultados de la consulta</returns>
        public JsonResult SearchRenovaciones(string agenteID, int offset, int limit, string search)
        {
            try
            {
                if (string.IsNullOrEmpty(agenteID)) agenteID = "0";
                if (string.IsNullOrEmpty(search)) search = "";
                if (offset == 0) offset = 1;
                if (limit == 0) limit = 10;

                var data = rnvc.SearchRenovacionesByAgente(agenteID, offset, limit, search);
                var totalRows = Convert.ToInt32(data.Tables["Pager"].Rows[0]["RecordCount"]);

                return Json(from g in data.Tables[0].Select()
                            select new
                            {
                                chkSelect = false,
                                nPolizaID = g["nPoliza"],
                                Cliente = g["Cliente"],
                                NombreAsegurado = g["Nombre"],
                                InicioVigencia = String.Format("{0:dd/MM/yyyy}", g["InicioVigencia"]),
                                FinVigencia = String.Format("{0:dd/MM/yyyy}", g["FinVigencia"]),
                                Observaciones = g["Observaciones"],
                                Emision = String.Format("{0:dd/MM/yyyy}", g["FechaExpedicion"]),
                                NumVehiculos = g["nNumVehi"],
                                NumConductores = g["nNumCond"],
                                ConductorRestr = g["cCondRest"],
                                Ejecutivo = g["cResponsa"],
                                CampainID = g["nCampID"],
                                ClienteID = g["nClienteID"],
                                EstatusID = g["nEstatusID"],
                                DescripEstatus = g["DescripcionEstatus"],
                                total = totalRows,
                            }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception err)
            {
                Response.StatusCode = (int)(System.Net.HttpStatusCode.InternalServerError);
                LogDeErrores.RegistroErrorWeb(err, DatoUsuario.idAgente, DatoUsuario.nomAgente, DatoUsuario.sIp, DatoUsuario.sHostName);
                return Json("ErrInterno");
            }
        }

        public JsonResult ObtenAños(string SubRamo)
        {
            try
            {
                DropDownList ddAño = new DropDownList();
                Dictionary<string, string> Años = new Dictionary<string, string>();
                int año = (SubRamo != "L" ? 1960 : 1994);
                int ultimoAño = (SubRamo != "L" ? DateTime.Now.Year + 1 : 2007);
                ddAño.Items.Clear();
                Años.Clear();
                while (ultimoAño >= año)
                {
                    Años.Add(string.Format("{0}", ultimoAño), string.Format("{0}", ultimoAño));
                    ultimoAño--;
                }
                return Json(new Dictionary<string, string>(Años));
            }
            catch (Exception err)
            {
                Response.StatusCode = (int)(System.Net.HttpStatusCode.InternalServerError);
                LogDeErrores.RegistroErrorWeb(err, DatoUsuario.idAgente, DatoUsuario.nomAgente, DatoUsuario.sIp, DatoUsuario.sHostName);
                return Json("ErrInterno");
            }
        }

        public JsonResult ObtenMarcaVehi(string subRamo, string straño)
        {
            try
            {
                DropDownList ddMarca = new DropDownList();
                if (straño != "0")
                {
                    if (subRamo == "L")
                    {
                        ctlg.cargaCmbMarcaVehiWeb(ddMarca, straño);
                    }
                    else
                    {
                        if (straño == "")
                            ctlg.CargaCmbMarcaWeb(ddMarca);
                        else
                            ctlg.CargaCmbMarcaWeb(ddMarca, int.Parse(straño));
                    }
                }
                var marca = ddMarca.Items;

                return Json(new SelectList(marca, "Value", "Text"));
            }
            catch (Exception err)
            {
                Response.StatusCode = (int)(System.Net.HttpStatusCode.InternalServerError);
                LogDeErrores.RegistroErrorWeb(err, DatoUsuario.idAgente, DatoUsuario.nomAgente, DatoUsuario.sIp, DatoUsuario.sHostName);
                return Json("ErrInterno");
            }
        }

        public JsonResult ObtenSubMarcaVehi(string subRamo, string straño, string nMarcaId)
        {
            try
            {
                int marcaID;
                DropDownList ddSubMarca = new DropDownList();


                if (subRamo == "L" && straño != "" && nMarcaId != "")
                {
                    //Cargo las SubMarcas
                    CmbCatalogos llenaCombos = new CmbCatalogos();
                    llenaCombos.cargaCmbSubMarcaVehiWeb(ddSubMarca, straño, nMarcaId);
                }
                else
                {
                    if (int.TryParse(nMarcaId, out marcaID))
                    {
                        if (straño == "")
                            ctlg.CargaCmbSubMarcaWeb(ddSubMarca, int.Parse(nMarcaId));
                        else
                            ctlg.CargaCmbSubMarcaWeb(ddSubMarca, int.Parse(nMarcaId), int.Parse(straño));

                    }
                }
                var subMarca = ddSubMarca.Items;

                return Json(new SelectList(subMarca, "Value", "Text"));
            }
            catch (Exception err)
            {
                Response.StatusCode = (int)(System.Net.HttpStatusCode.InternalServerError);
                LogDeErrores.RegistroErrorWeb(err, DatoUsuario.idAgente, DatoUsuario.nomAgente, DatoUsuario.sIp, DatoUsuario.sHostName);
                return Json("ErrInterno");
            }
        }

        public JsonResult ObtenTipoVehi(string subRamo, string straño, string nMarcaId, string SubMarca, string Marca)
        {
            try
            {
                MensajesError.tipo = "T";
                DropDownList ddTipo = new DropDownList();
                if (subRamo == "L" && straño != "" && nMarcaId != "" && SubMarca != "")
                {
                    //Cargo las SubMarcas
                    CmbCatalogos llenaCombos = new CmbCatalogos();
                    llenaCombos.cargaCmbTipoVehiWeb(ddTipo, straño, nMarcaId, SubMarca);
                }
                else
                {
                    ddTipo.DataSource = MnInf.FiltrarWebCmb(VarProcAMC.strPeriodo, straño, Marca, SubMarca); ;
                    ddTipo.DataTextField = "Descripcion".Trim();
                    ddTipo.DataValueField = "Clave";
                    ddTipo.DataBind();
                    //CmbSubMarca.Items.Insert(0, "<TODAS>");
                    ddTipo.ClearSelection();
                }
                var tipoVeh = ddTipo.Items;
                return Json(new SelectList(tipoVeh, "Value", "Text"));
            }
            catch (Exception err)
            {
                Response.StatusCode = (int)(System.Net.HttpStatusCode.InternalServerError);
                LogDeErrores.RegistroErrorWeb(err, DatoUsuario.idAgente, DatoUsuario.nomAgente, DatoUsuario.sIp, DatoUsuario.sHostName);
                return Json("ErrInterno");
            }
        }
        #endregion

        #region <-- Metodos privados -->
        private CotizaExpressModel NuevaCotizacionExpress()
        {
            try
            {
                DropDownList ddSubRamo = new DropDownList();
                Dictionary<string, string> Subramos = new Dictionary<string, string>();
                Subramos.Add("R", "Residente");
                Subramos.Add("L", "Legalizado");
                ddSubRamo.DataSource = Subramos;
                ddSubRamo.DataBind();

                DropDownList ddAño = new DropDownList();
                Dictionary<string, string> Años = new Dictionary<string, string>();
                int año = 1960;
                int añoahora = DateTime.Now.AddYears(1).Year;
                ddAño.Items.Clear();
                Años.Clear();

                Años.Add(string.Format("{0}", "0"), string.Format("{0}", "Años(Modelo)"));
                while (añoahora >= año)
                {
                    Años.Add(string.Format("{0}", añoahora), string.Format("{0}", añoahora));
                    añoahora--;
                }

                DropDownList ddEdoCivil = new DropDownList();
                ctlg.CargaCmbEdoCivilWeb(ddEdoCivil);

                DropDownList ddRC = new DropDownList();
                ctlg.CargaCmbRCWeb(ddRC);

                DropDownList ddGM = new DropDownList();
                ctlg.CargaCmbGMWeb(ddGM);

                DropDownList ddAsisteViaje = new DropDownList();
                ctlg.CargaCmbAsisteViajeWeb(ddAsisteViaje);

                DropDownList ddProliber = new DropDownList();
                ctlg.CargaCmbConSIoNOWeb(ddProliber, 1);

                DropDownList ddSAF = new DropDownList();
                ctlg.CargaCmbSAFWeb(ddSAF);

                DropDownList ddVehSus = new DropDownList();
                ctlg.CargaCmbVehSusWeb(ddVehSus);

                DropDownList ddExDeduc = new DropDownList();
                ctlg.CargaCmbConSIoNOWeb(ddExDeduc, 1);

                DropDownList ddRCCat = new DropDownList();
                ctlg.CargaCmbRCCatWeb(ddRCCat);

                DropDownList ddCampañas = new DropDownList();
                ctlg.CargaCmbCampañasWeb(ddCampañas);

                Dictionary<string, string> procar = new Dictionary<string, string>();
                int procar_ = 1;
                procar.Clear();

                while (procar_ <= 30)
                {
                    procar.Add(string.Format("{0}", procar_), string.Format("{0}", procar_));
                    procar_++;
                }

                DropDownList ddTersa = new DropDownList();
                ctlg.CargaCmbConSIoNOWeb(ddTersa, 1);

                DropDownList ddlRiesgo = new DropDownList();
                ctlg.CargaCmbCondRestWeb(ddlRiesgo);
                Dictionary<int, string> riesgosEx = new Dictionary<int, string>();
                for (int i = 0; i < ddlRiesgo.Items.Count; i++)
                {
                    riesgosEx.Add(i, ddlRiesgo.Items[i].Text);
                }

                var CtExp = new CotizaExpressModel();
                CtExp.cotizacion = new ClsCotizacion();
                CtExp.Telefonos = new ClsTelefono();
                CtExp.vehiculo = new ClsVehiculo();
                CtExp.conductor = new ClsConductor();
                CtExp.arrSubRamo = Subramos;
                CtExp.ArrAños = Años;
                CtExp.Procar = procar;
                CtExp.EdoCivil = new SelectList(ddEdoCivil.Items);
                CtExp.RiesgoConductorSourceEx = riesgosEx;
                CtExp.RC = new SelectList(ddRC.Items);
                CtExp.GM = new SelectList(ddGM.Items);
                CtExp.AsisteViaje = new SelectList(ddAsisteViaje.Items);
                CtExp.Proliber = new SelectList(ddProliber.Items);
                CtExp.SAF = new SelectList(ddSAF.Items);
                CtExp.VehSus = new SelectList(ddVehSus.Items);
                CtExp.ExDeduc = new SelectList(ddExDeduc.Items);
                CtExp.RCCat = new SelectList(ddRCCat.Items);
                CtExp.Campañas = new SelectList(ddCampañas.Items);
                CtExp.Tersa = new SelectList(ddTersa.Items);
                CtExp.EsHombre = true;
                CtExp.conductor.nombre = "x";
                CtExp.conductor.sexo = (CtExp.EsHombre ? 0 : 1);
                CtExp.conductor.DescripcionSexo = (CtExp.EsHombre ? "Masculino" : "Femenino");
                CtExp.conductor.estadoCivil = "S";
                CtExp.conductor.DescripcionEdoCivil = "Soltero(a)";
                CtExp.cotizacion.conductorRestringido = 1;
                CtExp.cotizacion.campaña = 1549;
                CtExp.cotizacion.titulo = "C.";
                CtExp.cotizacion.tipoPago = "C";
                CtExp.cotizacion.numVehiculos = CtExp.vehiculo.numVehiculos = 1;
                CtExp.cotizacion.numConductores = CtExp.vehiculo.numConductores = 1;
                CtExp.cotizacion.cVendida = "I";
                CtExp.cotizacion.VendidaPor = "Internet";
                CtExp.cotizacion.cVendida = CtExp.cotizacion.cVendida;
                CtExp.cotizacion.cAP = "N";
                CtExp.vehiculo.cAP = "N";
                CtExp.vehiculo.modelo = 1;
                CtExp.vehiculo.valorAuto = 1;
                CtExp.vehiculo.tpoEspecifAuto = "X";
                CtExp.vehiculo.DescripcionTipo = "X";
                CtExp.vehiculo.subRamo = "R";
                CtExp.vehiculo.proliber = "";
                CtExp.vehiculo.sumAsegFija = "";
                CtExp.vehiculo.asistenciaViaje = "";
                CtExp.vehiculo.subRamo = "";
                CtExp.vehiculo.codigoPostal = "";
                CtExp.vehiculo.titulo = "X";
                CtExp.vehiculo.apellidoPaterno = "X";
                CtExp.vehiculo.apellidoMaterno = "X";
                CtExp.vehiculo.nombres = "X";
                CtExp.vehiculo.codigoPostal = "2";
                var session = (DataSet)Session["UserObj"];
                if (session.Tables[0].Rows[0]["cNombre"].ToString().Trim().ToString().Equals("COTIZADOR EN LINEA"))
                {
                    CtExp.OrigenCotExp = "Clientes";
                }
                else
                {
                    if (Request.QueryString["Tipo"] == null)
                        CtExp.OrigenCotExp = "Agentes";
                    else
                        CtExp.OrigenCotExp = Request.QueryString["Tipo"];

                    DataSet TpCober = TpCober = MnInf.CargaDefaultCotizaciones(CtExp.OrigenCotExp);
                    CtExp.AplicaTERSSA = TpCober.Tables[0].Rows[0]["Terssa"].ToString() == "S" ? true : false;
                    CtExp.vehiculo.coberGtoMed = int.Parse(TpCober.Tables[0].Rows[0]["Descripcion"].ToString().Split('-')[1]);
                    CtExp.vehiculo.coberRepCiv = int.Parse(TpCober.Tables[0].Rows[0]["RC"].ToString());
                    CtExp.vehiculo.coberRepCivCat = int.Parse(TpCober.Tables[0].Rows[0]["RCcatastrofica"].ToString());
                    CtExp.vehiculo.campaña = DatoUsuario.campaña;
                    CtExp.cotizacion.campaña = DatoUsuario.campaña;
                    CtExp.vehiculo.ExcenDedu = TpCober.Tables[0].Rows[0]["ExtensionRC"].ToString();
                    CtExp.vehiculo.proliber = TpCober.Tables[0].Rows[0]["AseLegal"].ToString();
                    CtExp.vehiculo.sumAsegFija = TpCober.Tables[0].Rows[0]["SumaAseg"].ToString();
                    CtExp.vehiculo.vehSus = TpCober.Tables[0].Rows[0]["VehSus"].ToString();
                    CtExp.vehiculo.asistenciaViaje = TpCober.Tables[0].Rows[0]["Aviaje"].ToString();
                    CtExp.vehiculo.EsConductorBajoRiesgo = TpCober.Tables[0].Rows[0]["BajoRiesgo"].ToString();
                    CtExp.cotizacion.control = int.Parse(TpCober.Tables[0].Rows[0]["Procar"].ToString());

                }
                if (CtExp.AplicaTERSSA)
                {
                    CtExp.cotizacion.tersa = 1;
                    CtExp.DescripcionTersa = "Sí";
                }
                else
                {
                    CtExp.cotizacion.tersa = 0;
                    CtExp.DescripcionTersa = "No";
                }
                if (CtExp.OrigenCotExp != "Clientes")
                {
                    CtExp.CaptchaText = "x";
                }
                return CtExp;
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        public void Calculo(ref GenericoViewModel model, string Tipo)
        {
            try
            {
                model.conductores.Remove(model.conductores.FirstOrDefault(x => x.nombre == "-- Seleccione un conductor asignado --"));
                switch (Tipo)
                {
                    case "C":
                        /************************************************************/
                        if (model.vehiculos.Count == model.numVehiculos && model.conductores.Count == model.numConductores)
                        {
                            var logged = (DataSet)Session["UserObj"];
                            ClsComplemento Modi = new ClsComplemento();
                            VarProcAMC objVarProcAMC = new VarProcAMC();
                            model.periodo = VarProcAMC.strPeriodo;
                            double curDerechoEndoso = 0;
                            if (!string.IsNullOrEmpty(model.ModulodeTrabajo))
                            {
                                model.calculos = new List<ClsCalculo>();
                            }
                            //rCtz.cotizacion.CalculaCotizacion(rCtz.cotizacion, logged.Tables["catAgentes"].Rows[0]["nAgenteID"].ToString(), MnInf.ConvierteCobertura100(rCtz.cotizacion, "C"), MnInf.ConvierteTersa(rCtz.cotizacion, "C"), false);
                            //MnInf.LlenarlvCot(FrmDetalleGeneral.objCotizacion, strOrigen, "lvCot");
                            if (model.calculos == null)
                                model.calculos = new List<ClsCalculo>();
                            else
                            {
                                if (model.nQuitarPorPrima != 0)
                                {
                                    model.calculos.Clear();
                                }
                            }

                            for (int i = 1; i < 5; )
                            {
                                ClsCalculo ObjCalculo = new ClsCalculo();
                                int contCober = 0;
                                int contCond = 0;
                                int cobertura100 = VldVeh.ValidaConvierteCobertura100(ref contCober, ref contCond, model.cobertura100, model.vehiculos, model.conductores, "C");
                                int contTerssa = 0;
                                int terssa = VldVeh.ValidaConvierteTerssa(ref contTerssa, model.tersa, model.vehiculos, "C");
                                MnInf.LlenaObjetoCalculo(CentralAgentesMvc.App_Start.CastObject.Cast<ClsCotizacion>(model), "C", ref ObjCalculo, i);
                                if (objVarProcAMC.CalculaPrimas(ref ObjCalculo, ref curDerechoEndoso, VarProcAMC.strPeriodo, 0, "A", true, 0, 0, contCober, contTerssa, "") == true)
                                {
                                    model.ObjCalculo = ObjCalculo;
                                    model.complemento.precioAguila = ObjCalculo.mTotal;
                                }
                                else
                                {
                                    //MessageBox.Show("Error en cálculo");
                                    //return;
                                }
                                model.calculos.Add(ObjCalculo);
                                i++;
                            }
                            if (model.formaPago > 0)
                            {
                                model.ObjCalculo = model.calculos[model.formaPago - 1];
                                objVarProcAMC = new VarProcAMC();
                                model.RevisaCedeComision(CentralAgentesMvc.App_Start.CastObject.Cast<ClsCotizacion>(model), model.QuiereCederComision, model.ObjCalculo.primaNeta, "C");
                                model.vehiculos = model.ObjCalculo.vehiculos;
                            }
                        }
                        else
                        {
                            if (model.vehiculos.Count != model.numVehiculos)
                                MensajesError.ErroresCalculo.Add("No se han capturado todos los vehiculos especificados.");

                            if (model.conductores.Count != model.numConductores)
                                MensajesError.ErroresCalculo.Add("No se han capturado todos los conductores especificados.");
                        }
                        /************************************************************************/
                        break;
                    case "R":
                        break;
                }
            }
            catch (Exception err)
            {
                Response.StatusCode = (int)(System.Net.HttpStatusCode.InternalServerError);
                LogDeErrores.RegistroErrorWeb(err, DatoUsuario.idAgente, DatoUsuario.nomAgente, DatoUsuario.sIp, DatoUsuario.sHostName);
                throw err;
            }
        }

        private void DefaultCalculos(ref ResumenCotizar rCtz)
        {
            try
            {
                rCtz.Descripcion2Pago = "1 Pago(s) de:";
                rCtz.VehiculosInfo = new List<ClsVehiculo>();
                rCtz.OpcionesPagoInfo = new List<ClsOpcionesPago>();

                rCtz.OpcionesPagoInfo.Add(new ClsOpcionesPago()
                {
                    OpcionPagoID = 1,
                    DescripcionRubro = "Pago Inicial:",
                    PagoContado = string.Format("{0:C2}", 0),
                    PagoSemestral = string.Format("{0:C2}", 0),
                    PagoTrimestral = string.Format("{0:C2}", 0),
                    PagoMensual = string.Format("{0:C2}", 0)
                });

                rCtz.OpcionesPagoInfo.Add(new ClsOpcionesPago()
                {
                    OpcionPagoID = 2,
                    DescripcionRubro = "Pagos Subsecuentes:",
                    PagoContado = string.Format("{0}", ""),
                    PagoSemestral = string.Format("{0:C2}", 0),
                    PagoTrimestral = string.Format("{0:C2}", 0),
                    PagoMensual = string.Format("{0:C2}", 0),
                });

                rCtz.OpcionesPagoInfo.Add(new ClsOpcionesPago()
                {
                    OpcionPagoID = 3,
                    DescripcionRubro = "Número de Pagos Subsecuentes:",
                    PagoContado = string.Format("{0}", ""),
                    PagoSemestral = string.Format("{0:C2}", 0),
                    PagoTrimestral = string.Format("3"),
                    PagoMensual = string.Format("{0:C2}", 0),
                });

                rCtz.OpcionesPagoInfo.Add(new ClsOpcionesPago()
                {
                    OpcionPagoID = 4,
                    DescripcionRubro = "<strong><i>Total:</i></strong>",
                    PagoContado = string.Format("<strong><i>{0:C2}</i></strong>", 0),
                    PagoSemestral = string.Format("<strong><i>{0:C2}</i></strong>", 0),
                    PagoTrimestral = string.Format("<strong><i>{0:C2}</i></strong>", 0),
                    PagoMensual = string.Format("<strong><i>{0:C2}</i></strong>", 0)
                });
            }
            catch (Exception err) {
                throw err;
            }
        }

        #endregion

        #region     <-- Métodos Privados -->

        private void GuardaEnMemoria(ClsCotizacion ctz)
        {
            try
            {
                var gm = CentralAgentesMvc.App_Start.CastObject.Cast<GenericoViewModel>(ctz);
                GuardaEnMemoria(gm, ctz);
            }
            catch (Exception err) {
                throw err;
            }
        }
        private void GuardaEnMemoria(GenericoViewModel gm)
        {
            try
            {
                var ctz = CentralAgentesMvc.App_Start.CastObject.Cast<ClsCotizacion>(gm);
                GuardaEnMemoria(gm, ctz);
            }
            catch (Exception err)
            {
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
    }
}
