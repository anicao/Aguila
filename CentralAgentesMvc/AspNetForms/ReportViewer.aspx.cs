using System;
using CrystalDecisions.CrystalReports.Engine;
using Negocio.ReportesCrystal;
using System.Linq;

namespace CentralAgentesMvc.AspNetForms
{
    public partial class ReportViewer : System.Web.UI.Page
    {
        protected void Page_Init(object sender, EventArgs e)
        {
            if (!IsPostBack)
                ConfiguraReporte();
            else
                SetReporte();
        }

        private void ConfiguraReporte()
        {
            var fIni = Request.QueryString["INICIO"];
            var fFin = Request.QueryString["FIN"];
            var agen = Request.QueryString["AGENTE"];

            ReportDocument report = new ReportDocument();
            CrReportCentral cr = new CrReportCentral();

            switch (Request.QueryString["REPORT"].ToString().ToLower())
            {
                case "liquidaciones":
                    var lr = Request.QueryString["L"];
                    if (lr == "1")
                    {
                        this.Title = "Resumen de Liquidaciones";
                        cr.ResumenLiquidaciones(report, fIni, fFin, true);
                    }
                    if (lr == "2")
                    {
                        this.Title = "Resumen de Estado de Cuentas";
                        cr.EstadoCuentaResumenAgente(report, fIni, fFin, agen, true);
                    }
                    if (lr == "3")
                    {
                        this.Title = "Estado de Cuentas Detallado";
                        cr.EstadoCuentaDetalleAgente(report, fIni, fFin, agen, true);
                    }
                    Session.Add(string.Format("rpResLiq{0}", lr), report);
                    this.CRViewer.ToolPanelView = CrystalDecisions.Web.ToolPanelViewType.None;
            CRViewer.ParameterFieldInfo = report.ParameterFields;
                    break;

                case "movimientos":
                    {
                        var nombrA = Request.QueryString["N"];
                        this.Title = "Detalle de Movimientos Emitidos";
                        cr.DetalleMovimientosAgente(report, fIni, fFin, agen, nombrA, true);
                        Session.Add("rpMovim", report);
                        this.CRViewer.ToolPanelView = CrystalDecisions.Web.ToolPanelViewType.GroupTree;
                        CRViewer.ParameterFieldInfo = report.ParameterFields;
                    }
                    break;

                case "renovaciones":
                    {
                        var nombrA = Request.QueryString["N"];
                        this.Title = "Reporte de Vencimientos";
                        cr.ReporteVencimientos(report, fIni, fFin, nombrA, true, agen);
                        this.CRViewer.ToolPanelView = CrystalDecisions.Web.ToolPanelViewType.GroupTree;
                        Session.Add("rpRenov", report);
                        CRViewer.ParameterFieldInfo = report.ParameterFields;
                    }
                    break;

                case "cobranza":
                    {
                        var nombrA = Request.QueryString["N"];
                        this.Title = "Reporte de Cobranza";
                        cr.ReporteCobranza(report, fIni, fFin, agen, nombrA, true);
                        this.CRViewer.ToolPanelView = CrystalDecisions.Web.ToolPanelViewType.None;
                        Session.Add("rpCobra", report);
                        CRViewer.ParameterFieldInfo = report.ParameterFields;
                    }
                    break;

                case "c100":
                    {
                        var polizaID = Convert.ToInt64(Request.QueryString["P"]);
                        var noEndoso = Convert.ToInt32(Request.QueryString["N"]);
                        this.Title = "Reporte Cobertura 100";
                        cr.ReporteCobertura100(report, polizaID, noEndoso, true);
                        this.CRViewer.ToolPanelView = CrystalDecisions.Web.ToolPanelViewType.None;
                        Session.Add("rpC100", report);
                        CRViewer.ParameterFieldInfo = report.ParameterFields;
                    }
                    break;

                case "psob":
                    {
                        var polizaID = Convert.ToInt64(Request.QueryString["P"]);
                        this.Title = "Reporte Seguro Obligatorio";
                        cr.ReporteSeguroObligatorio(report, polizaID, true);
                        this.CRViewer.ToolPanelView = CrystalDecisions.Web.ToolPanelViewType.None;
                        Session.Add("rpPSOB", report);
                        CRViewer.ParameterFieldInfo = report.ParameterFields;
                    }
                    break;

                case "cotizacion":
                    {
                        var cotizaID = Request.QueryString["CTZ"].ToString();
                        var montoCtz = Convert.ToInt32(Request.QueryString["MNT"]);
                        var mesesCob = Request.QueryString["MSS"];
                        var tpPagCtz = Request.QueryString["TPG"].ToString();
                        var line1Ctz = Request.QueryString["LN1"].ToString();
                        var line2Ctz = Request.QueryString["LN2"].ToString();

                        this.Title = "Cotización No.: " + cotizaID.ToString();
                        cr.CotizacionWeb(report, cotizaID, montoCtz, mesesCob, tpPagCtz, line1Ctz, line2Ctz);
                        this.CRViewer.ToolPanelView = CrystalDecisions.Web.ToolPanelViewType.None;
                        Session.Add("rpCTZ", report);
                        CRViewer.ParameterFieldInfo = report.ParameterFields;
                    }
                    break;
                case "solrenovacion":
                    {
                        Negocio.ClasesCentral.ClsRenovaciones objRenovacion = new Negocio.ClasesCentral.ClsRenovaciones();
                        long strPoliza = long.Parse(Request.QueryString["PLZ"].ToString());
                        string strOrigen = Request.QueryString["SOr"].ToString();
                        string cEra = Request.QueryString["SERA"].ToString();
                        int intasiVje = 0;
                        //Asistencia en viaje
                        if (cEra == "S")
                        {
                            intasiVje = objRenovacion.RetornaAsistViaje(strPoliza, strOrigen);
                        }

                        int intAÑOS = objRenovacion.RetornaAños(strPoliza, strOrigen);
                        var strTipoPago = Request.QueryString["STP"];
                        var strmPag1 = Request.QueryString["MP1"].ToString();
                        var strmPag2 = Request.QueryString["MP2"].ToString();
                        var strFmaPgo = Negocio.AccesosYPermisos.VarProcInterfazX.arrPago.Where(forma => forma.strCve == Request.QueryString["SFP"].ToString()).FirstOrDefault().strDesc;
                        var strERA = Request.QueryString["SERA"].ToString();
                        var strEjec = Request.QueryString["SEJC"].ToString();
                        var str6mese = (from prom in Negocio.AccesosYPermisos.VarProcInterfazX.arrPromocion where prom.nPromocionId == int.Parse(Request.QueryString["S6M"].ToString()) select prom.cNombre).FirstOrDefault();
                        var strkit = Request.QueryString["SKT"].ToString();
                        var strsolicitud = Request.QueryString["SSOL"].ToString();
                        var strPag1 = Request.QueryString["SP1"].ToString();
                        var strPag2 = Request.QueryString["SP2"].ToString();
                        str6mese = (str6mese!=null?str6mese:"");
                        this.Title = "Confirmación de Renovación: " + strPoliza.ToString();
                        cr.PrintConfirmacionRenoWeb(report, strPoliza.ToString(), strTipoPago, strmPag1, strmPag2, strFmaPgo, strERA, strEjec, str6mese, strkit, strsolicitud, intAÑOS, intasiVje, strPag1, strPag2);
                        this.CRViewer.ToolPanelView = CrystalDecisions.Web.ToolPanelViewType.None;
                        Session.Add("rpSRN", report);
                        //CRViewer.ParameterFieldInfo = report.ParameterFields;//<----------este es suprimido en este reporte
                    }
                    break;
            
            }

            CRViewer.ReportSource = report;
            CRViewer.DataBind();
        }

        private void SetReporte()
        {
            switch (Request.QueryString["REPORT"].ToString().ToLower())
            {
                case "liquidaciones":
                    var lr = Request.QueryString["L"];
                    CRViewer.ReportSource = Session[string.Format("rpResLiq{0}", lr)];
                    break;

                case "movimientos":
                    CRViewer.ReportSource = Session["rpMovim"];
                    break;

                case "renovaciones":
                    CRViewer.ReportSource = Session["rpRenov"];
                    break;

                case "cobranza":
                    CRViewer.ReportSource = Session["rpCobra"];
                    break;

                case "c100":
                    CRViewer.ReportSource = Session["rpC100"];
                    break;

                case "psob":
                    CRViewer.ReportSource = Session["rpPSOB"];
                    break;

                case "cotizacion":
                    CRViewer.ReportSource = Session["rpCTZ"];
                    break;

                case "solrenovacion":
                    CRViewer.ReportSource = Session["rpSRN"];
                    break;
            }
            CRViewer.DataBind();
        }
    }
}