using System;
using System.Linq;
using System.Data;
using System.Web.Mvc;
using System.Collections.Generic;
//==================================
using Negocio.HerramientasVarios;
using Negocio.AccesosYPermisos;
using Negocio.ClasesCentral;
//==================================
using CentralAgentesMvc.Models;

namespace CentralAgentesMvc.Controllers
{
    [Authorize]
    public class ReportsController : Controller
    {
        CmbCatalogos ctlg = new CmbCatalogos();
        ManejoInformacion info = new ManejoInformacion();

        #region <-- Reportes de Agentes -->
        // GET: Reports/Agentes
        public ActionResult Agentes(string rptID)
        {
            try
            {
                var logged = (DataSet)Session["UserObj"];

                var rpt = new ReportViewModel();
                rpt.AgenteID = logged.Tables["catAgentes"].Rows[0]["nAgenteID"].ToString();
                rpt.AgenteName = logged.Tables["catAgentes"].Rows[0].ItemArray[5].ToString();
                rpt.AgentesSource = VarProcInterfazX.arrAgentes;

                rpt.ReportID = rptID;
                rpt.ReportName = (rptID == "0" ? "Liquidaciones"
                                : rptID == "1" ? "Movimientos"
                                : rptID == "2" ? "Renovaciones" : "Cobranza");
                rpt.FechaDesde = "";
                rpt.FechaHasta = "";

                return PartialView(rpt);
            }
            catch (Exception err)
            {
                Response.StatusCode = (int)(System.Net.HttpStatusCode.InternalServerError);
                LogDeErrores.RegistroErrorWeb(err, DatoUsuario.idAgente, DatoUsuario.nomAgente, DatoUsuario.sIp, DatoUsuario.sHostName);
                return Json("ErrInterno");
            }
        }
        #endregion

        #region <-- Consulta de Facturas -->
        // GET: Reports/Facturas
        public ActionResult Facturas()
        {
            try
            {
                var logged = (DataSet)Session["UserObj"];

                var rpt = new ReportViewModel();
                rpt.AgenteID = logged.Tables["catAgentes"].Rows[0]["nAgenteID"].ToString();
                rpt.AgenteName = logged.Tables["catAgentes"].Rows[0].ItemArray[5].ToString();
                rpt.AgentesSource = VarProcInterfazX.arrAgentes;
                rpt.FechaDesde = "";
                rpt.FechaHasta = "";
                rpt.PolizaID = "";
                rpt.ClienteID = "";
                rpt.NombreAsegurado = "";

                return View(rpt);
            }
            catch (Exception err)
            {
                Response.StatusCode = (int)(System.Net.HttpStatusCode.InternalServerError);
                LogDeErrores.RegistroErrorWeb(err, DatoUsuario.idAgente, DatoUsuario.nomAgente, DatoUsuario.sIp, DatoUsuario.sHostName);
                return Json("ErrInterno");
            }
        }

        public JsonResult ClientsList(string query)
        {
            try
            {
                return Json(from g in VarProcInterfazX.arrClientes.Where(c => c.strDescrip.ToLower().StartsWith(query.ToLower()))
                                                                                          .OrderBy(c => c.strDescrip)
                                                                                          .Take(15)
                            select new { id = g.lngClave, name = g.strDescrip }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception err)
            {
                Response.StatusCode = (int)(System.Net.HttpStatusCode.InternalServerError);
                LogDeErrores.RegistroErrorWeb(err, DatoUsuario.idAgente, DatoUsuario.nomAgente, DatoUsuario.sIp, DatoUsuario.sHostName);
                return Json("ErrInterno");
            }
        }

        

        public JsonResult GetInvoices(string fechaInicio, string fechaFinal, string agenteID, string polizaID, string clienteID, int offset, int limit)
        {
            try
            {
                
                List<object> prmtrs = new List<object>();
                prmtrs.Add(fechaInicio);
                prmtrs.Add(fechaFinal);
                prmtrs.Add(agenteID);
                prmtrs.Add(polizaID);
                prmtrs.Add(clienteID);
                prmtrs.Add(offset);
                prmtrs.Add(limit);
                var data = info.ConsultaFacturasWeb(prmtrs);
                var totalRows = Convert.ToInt32(data.Tables["Pager"].Rows[0]["RecordCount"]);

                return Json(from g in data.Tables[0].Select()
                            select new
                            {
                                nPoliza = g["nPoliza"],
                                dfExpedicion = String.Format("{0:dd/MM/yyyy}", g["dfExpedicion"]),
                                cNombreA = g["cNombreA"],
                                nEndoso = g["nEndoso"],
                                nRecibo = g["nRecibo"],
                                Fecha_Emision = String.Format("{0:dd/MM/yyyy}", g["Fecha_Emision"]),
                                Serie = g["Serie"],
                                Folio = g["Folio"],
                                LnkPDF = g["LnkPDF"],
                                LnkXML = g["LnkXML"],
                                Lnk100 = g["Lnk100"],
                                Css100 = g["Css100"],
                                LnkSOB = g["LnkSOB"],
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
        #endregion
    }
}
