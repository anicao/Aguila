using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Negocio.ClasesCentral;
using System.Data;
using CentralAgentesMvc.Models;
using Negocio.HerramientasVarios;

namespace CentralAgentesMvc.Controllers
{
    public class SiniestroController : Controller
    {
        //
        // GET: /Siniestro/

        public ActionResult showSiniestro(string polizaID, string siniestroID, int offset, int limit)
        {
            try
            {
                if (string.IsNullOrEmpty(polizaID)) polizaID = "0";

                if (offset == 0) offset = 1;
                if (limit == 0) limit = 10;

                ManejoInformacion info = new ManejoInformacion();
                var data = info.getSiniestrosSl(polizaID, siniestroID, offset, limit);
                var totalRows = Convert.ToInt32(data.Tables["Pager"].Rows[0]["RecordCount"]);
                DataTable vigencias = new DataTable();
                List<SiniestroModel> listSi = new List<SiniestroModel>();

                var list = (from g in data.Tables[0].Select()
                            select new SiniestroModel
                            {
                                siniestroId = g["nSiniestroID"].ToString(),
                                fecha = String.Format("{0:dd/MM/yyyy}", g["dFSiniestro"]),
                                tipoSiniestro = g["cDescrip"].ToString(),
                                auto = g["cTpoEspAut"].ToString(),
                                chofer = g["cConductor"].ToString(),
                                detalle = g["cInforme"].ToString(),
                                NoSerie = g["cNumSerie"].ToString(),
                                Placas = g["cplacas"].ToString()
                            }
               );
                listSi = list.ToList();
                return PartialView("showSiniestro", listSi);
            }
            catch (Exception err)
            {
                Response.StatusCode = (int)(System.Net.HttpStatusCode.InternalServerError);
                LogDeErrores.RegistroErrorWeb(err, DatoUsuario.idAgente, DatoUsuario.nomAgente, DatoUsuario.sIp, DatoUsuario.sHostName);
                return Json("ErrInterno");
            }
        }

        public ActionResult details(SiniestroModel si){
            try
            {
                ManejoInformacion info = new ManejoInformacion();
                var data = info.getSiniestrosSl(si.polizaId, si.siniestroId, 1, 10);
                List<SiniestroModel> listSi = new List<SiniestroModel>();
                var list = (from g in data.Tables[0].Select()
                            select new SiniestroModel
                            {
                                siniestroId = g["nSiniestroID"].ToString(),
                                fecha = String.Format("{0:dd/MM/yyyy}", g["dFSiniestro"]),
                                tipoSiniestro = g["cDescrip"].ToString(),
                                auto = g["cTpoEspAut"].ToString(),
                                chofer = g["cConductor"].ToString(),
                                detalle = g["cInforme"].ToString(),
                                NoSerie = g["cNumSerie"].ToString(),
                                Placas = g["cplacas"].ToString(),
                                taller = g["taller"].ToString(),
                                fechaRecepcion = String.Format("{0:dd/MM/yyyy}",g["fechaRecepcion"]),
                                promesaEntrega = String.Format("{0:dd/MM/yyyy}", g["promesaEntrega"]),
                                direccion = g["direccion"].ToString(),
                                telefonos = g["telefonos"].ToString()
                            }
                           );
                si = list.First();

                return PartialView("_siniestroDetails", si);
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
