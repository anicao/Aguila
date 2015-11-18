using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CentralAgentesMvc.Models;
using Negocio.ClasesCentral;
using System.Data;
using Negocio.HerramientasVarios;

namespace CentralAgentesMvc.Controllers
{
    public class RecibosController : Controller
    {
        //
        // GET: /Pagos/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult pagosPendientes(string polizaID, int offset, int limit) {
            try
            {
                if (string.IsNullOrEmpty(polizaID)) polizaID = "0";

                if (offset == 0) offset = 1;
                if (limit == 0) limit = 10;
                ManejoInformacion info = new ManejoInformacion();
                var data = info.getPolizasPendientesSl(polizaID, offset, limit);
                var totalRows = Convert.ToInt32(data.Tables["Pager"].Rows[0]["RecordCount"]);
                DataTable vigencias = new DataTable();
                List<PagosModel> pagosPendientes = new List<PagosModel>();
                var list = (from g in data.Tables[0].Select()
                            select new PagosModel
                            {
                                endoso = int.Parse(g["endoso"].ToString()),
                                recibo = int.Parse(g["recibo"].ToString()),
                                vigenciaInicio = String.Format("{0:dd/MM/yyyy}", g["vigenciaInicio"]),
                                vigenciafin = String.Format("{0:dd/MM/yyyy}", g["vigenciaFin"]),
                                total = String.Format("{0:c3}", g["total"]),

                            }
               );
                pagosPendientes = list.ToList();
                return PartialView("_GridpagosPendientes", pagosPendientes);
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
