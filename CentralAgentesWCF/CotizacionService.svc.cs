using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace CentralAgentesWCF
{
    public class CotizacionService : ICotizacionService
    {
        public bool SendCotizacionByMail(string cotizacionID)
        {
            var path = System.Web.Hosting.HostingEnvironment.MapPath("~/Reportes/");
            Negocio.ReportesCrystal.CrReportCentral ctz = new Negocio.ReportesCrystal.CrReportCentral();
            return ctz.GeneraCotizacionWebPDF(cotizacionID, path);
        }
    }
}
