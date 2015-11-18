using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//================================
using Negocio.ReportesCrystal;

namespace EnviaCotizacionMail
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Hora Inicio envio de cotizaciones: {0}", DateTime.Now.TimeOfDay.ToString());
                Console.WriteLine("*** Enviando cotizaciones automáticas ***");

                CrReportCentral cr = new CrReportCentral();
                cr.GeneraCotizacionWebPDF(args[0].ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + ex.StackTrace);
            }
            finally
            {
                Console.WriteLine();
                Console.WriteLine("*** Cotizaciones enviadas ***");
                Console.WriteLine("Hora Final envio de cotizaciones: {0}", DateTime.Now.TimeOfDay.ToString());

            }
        }
    }
}
