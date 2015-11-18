using System;
using System.Linq;
using System.Data;
using System.Web.Mvc;
using System.Collections.Generic;
//==================================
using Negocio.ClasesCentral;
//==================================
using CentralAgentesMvc.Models;

namespace CentralAgentesMvc.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        ManejoInformacion info = new ManejoInformacion();

        #region <-- Action Methods -->
        // GET: /Home/
        public ActionResult Index()
        {
            int NumCotizacionesMes = 0;
            int NumCotizacionesAño = 0;
            decimal MontoPolPrimNetaMes = 0;
            decimal MontoPolPrimNetaAño = 0;

            int NumRenovacionesMes = 0;
            int NumRenovacionesAño = 0;
            decimal MontoRenPolPrimNetaMes = 0;
            decimal MontoRenPolPrimNetaAño = 0;

            int NumEndososMes = 0;
            int NumEndososAño = 0;
            decimal MtoEndososMes = 0;
            decimal MtoEndososAño = 0;

            string sUltimoPagoCom = "";


            var logged = (DataSet)Session["UserObj"];
            var agentID = logged.Tables["catAgentes"].Rows[0]["nAgenteID"].ToString();

            if (Convert.ToInt32(agentID) != 0 & Convert.ToInt32(agentID) != 4)
            {
                var data = info.ConsultaInformacionEncabezadoWeb(agentID);

                NumCotizacionesMes = Convert.ToInt32(data.Tables["InformacionEncabezado"].Rows[0]["NumCotizacionesMes"]);
                NumCotizacionesAño = Convert.ToInt32(data.Tables["InformacionEncabezado"].Rows[0]["NumCotizacionesAño"]);
                MontoPolPrimNetaMes = Convert.ToDecimal(data.Tables["InformacionEncabezado"].Rows[0]["MontoPolPrimNetaMes"]);
                MontoPolPrimNetaAño = Convert.ToDecimal(data.Tables["InformacionEncabezado"].Rows[0]["MontoPolPrimNetaAño"]);

                NumRenovacionesMes = Convert.ToInt32(data.Tables["InformacionEncabezado"].Rows[0]["NumRenovacionesMes"]);
                NumRenovacionesAño = Convert.ToInt32(data.Tables["InformacionEncabezado"].Rows[0]["NumRenovacionesAño"]);
                MontoRenPolPrimNetaMes = Convert.ToDecimal(data.Tables["InformacionEncabezado"].Rows[0]["MontoRenPolPrimNetaMes"]);
                MontoRenPolPrimNetaAño = Convert.ToDecimal(data.Tables["InformacionEncabezado"].Rows[0]["MontoRenPolPrimNetaAño"]);

                NumEndososMes = Convert.ToInt32(data.Tables["InformacionEncabezado"].Rows[0]["NumEndososMes"]);
                NumEndososAño = Convert.ToInt32(data.Tables["InformacionEncabezado"].Rows[0]["NumEndososAño"]);
                MtoEndososMes = Convert.ToDecimal(data.Tables["InformacionEncabezado"].Rows[0]["MontoEndososMes"]);
                MtoEndososAño = Convert.ToDecimal(data.Tables["InformacionEncabezado"].Rows[0]["MontoEndososAño"]);

                sUltimoPagoCom = Convert.ToString(data.Tables["InformacionEncabezado"].Rows[0]["UltimoPago"]);
            }
            if (Convert.ToInt32(agentID) == 4)
            {
                Response.Redirect("http://www.elaguila.com.mx");
            }

            HomeViewModel home = new HomeViewModel()
            {
                CotizacionesMes = NumCotizacionesMes,
                CotizacionesAño = NumCotizacionesAño,
                PolPrimNetaMes = MontoPolPrimNetaMes,
                PolPrimNetaAño = MontoPolPrimNetaAño,

                RenovacionesMes = NumRenovacionesMes,
                RenovacionesAño = NumRenovacionesAño,
                RenPolPrimNetaMes = MontoRenPolPrimNetaMes,
                RenPolPrimNetaAño = MontoRenPolPrimNetaAño,

                EndososMes = NumEndososMes,
                EndososAño = NumEndososAño,
                MontoEndososMes = MtoEndososMes,
                MontoEndososAño = MtoEndososAño,

                PrimaTotalMes = (MontoPolPrimNetaMes + MontoRenPolPrimNetaMes + MtoEndososMes),
                PrimaTotalAño = (MontoPolPrimNetaAño + MontoRenPolPrimNetaAño + MtoEndososAño),

                UltimoPagoCom = sUltimoPagoCom
            };

            return View(home);
        }
        public PartialViewResult GraphPage()
        {
            return PartialView();
        }
        public PartialViewResult NotifyCarousel()
        {
            return PartialView();
        }

        public PartialViewResult Welcome()
        {
            return PartialView();
        }

        public ActionResult EnDesarrollo()
        {
            return PartialView();
        }
        #endregion

        #region <-- JSON Methods -->

        public JsonResult GetDataGraph()
        {
            List<HomeViewModel> grph = new List<HomeViewModel>();
            grph.Add(new HomeViewModel()
            {
                ID = 1,
                DiaMes = "04-05",
                CountCotiza = 40,
                CountPoliza = 32,
            });
            grph.Add(new HomeViewModel()
            {
                ID = 1,
                DiaMes = "05-05",
                CountCotiza = 19,
                CountPoliza = 17,
            });
            grph.Add(new HomeViewModel()
            {
                ID = 1,
                DiaMes = "06-05",
                CountCotiza = 23,
                CountPoliza = 23,
            });
            grph.Add(new HomeViewModel()
            {
                ID = 1,
                DiaMes = "07-05",
                CountCotiza = 14,
                CountPoliza = 5,
            });
            grph.Add(new HomeViewModel()
            {
                ID = 1,
                DiaMes = "08-05",
                CountCotiza = 20,
                CountPoliza = 10,
            });
            grph.Add(new HomeViewModel()
            {
                ID = 1,
                DiaMes = "09-05",
                CountCotiza = 16,
                CountPoliza = 14,
            });
            grph.Add(new HomeViewModel()
            {
                ID = 1,
                DiaMes = "10-05",
                CountCotiza = 26,
                CountPoliza = 26,
            });
            grph.Add(new HomeViewModel()
            {
                ID = 1,
                DiaMes = "11-05",
                CountCotiza = 20,
                CountPoliza = 10,
            });

            return Json(from g in grph
                        select new
                        {
                            Tick = g.DiaMes,
                            Cotz = g.CountCotiza,
                            Polz = g.CountPoliza,
                        }, JsonRequestBehavior.AllowGet);
        }

        #endregion

    }
}
