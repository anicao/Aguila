using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Negocio.ClasesCentral;
using Negocio.HerramientasVarios;
using CentralAgentesMvc.Models;

namespace CentralAgentesMvc.Controllers
{
    public class EndososController : Controller
    {
        //
        // GET: /Endosos/
        ManejoInformacion info = new ManejoInformacion();
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult GetEndosos(string fechaInicio, string fechaFinal, string agenteID, string polizaID, string clienteID, int offset, int limit)
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
                var data = info.getEndososWeb(prmtrs);
                var totalRows = Convert.ToInt32(data.Tables["Pager"].Rows[0]["RecordCount"]);

                var rest = from g in data.Tables[0].Select()
                           select new
                           {
                               Poliza = g["npoliza"],
                               Endoso = g["nEndoso"],
                               recibo = g["nrecibo"],
                               NombreA = g["cNombreA"],
                               Expedicion = String.Format("{0:dd/MM/yyyy}", g["dfExpedicion"]),
                               Solicitud = g["nSolicitudID"],
                               FechaInicio = String.Format("{0:dd/MM/yyyy}", g["dFIniVig"]),
                               FechaFin = String.Format("{0:dd/MM/yyyy}", g["dFFinVig"]),

                               total = totalRows,
                           };
                return Json(rest, JsonRequestBehavior.AllowGet);
            }
            catch (Exception err)
            {
                Response.StatusCode = (int)(System.Net.HttpStatusCode.InternalServerError);
                LogDeErrores.RegistroErrorWeb(err, DatoUsuario.idAgente, DatoUsuario.nomAgente, DatoUsuario.sIp, DatoUsuario.sHostName);
                return Json("ErrInterno");
            }
        }

    }
}
