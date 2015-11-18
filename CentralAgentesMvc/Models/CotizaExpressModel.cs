using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Negocio.ClasesCentral;
using Negocio.AccesosYPermisos;
//==============================
using System.Web.Mvc;
using System.ComponentModel;
using System;

namespace CentralAgentesMvc.Models
{
    [Serializable]
    public class CotizaExpressModel
    {
        [Required]
        public string CaptchaText { get; set; }
        public string OrigenCotExp { get; set; }
        public string JsFunction { get; set; }
        [System.Web.Script.Serialization.ScriptIgnore]
        public ClsCotizacion cotizacion{get; set;}
        public ClsTelefono Telefonos { get; set; }
        public ClsVehiculo vehiculo { get; set; }
        public ClsConductor conductor { get; set; }
        public Dictionary<string, string> arrSubRamo { get; set; }
        public Dictionary<string, string> ArrAños { get; set; }
        public SelectList Marcas { get; set; }
        public SelectList subMarcas { get; set; }
        public IEnumerable<SelectListItem> tipoVehiculo { get; set; }
        public SelectListItem vehiculoClaveDesc { get; set; }
        [Required(ErrorMessage = "Debe filtrar su vehiculo")]
        public string marca_ { get; set; }
        [Required(ErrorMessage = "Debe filtrar su vehiculo")]
        public string Sbmarca_ { get; set; }
        [Required(ErrorMessage="Debe filtrar su vehiculo")]
        public string tipoVeh { get; set; }
        [Required(ErrorMessage = "Agregar Edad")]
        public string edad { get; set; }
        public bool EsHombre { get; set; }
        public SelectList EdoCivil { get; set; }
        public Dictionary<int, string> RiesgoConductorSourceEx { get; set; }
        public SelectList RC { get; set; }
        public SelectList GM { get; set; }
        public SelectList AsisteViaje { get; set; }
        public SelectList Proliber { get; set; }
        public SelectList SAF { get; set; }
        public SelectList VehSus { get; set; }
        public SelectList ExDeduc { get; set; }
        public SelectList RCCat { get; set; }
        public SelectList Campañas { get; set; }
        public Dictionary<string, string> Procar { get; set; }        
        public SelectList Tersa { get; set; }
        public string TersaEx { get; set; }
        public string DescripcionTersa { get; set; }
        public int tersa_ { get; set; }
        public bool AplicaTERSSA { get; set; }
        public decimal SubTotalPrimas { get; set; }
        public decimal SubTotalServicios { get; set; }
        public decimal DerechoPoliza { get; set; }
        public decimal ExtensionRC { get; set; }
        public decimal AccidentesPersonales { get; set; }
        public decimal Recargos { get; set; }
        public decimal SubTotal { get; set; }
        public decimal IVA { get; set; }
        public decimal TotalCotizacion { get; set; }
        public static List<CoberturasModel> coberturas { get; set; }
        public List<ClsOpcionesPago> OpcionesPagoInfo { get; set; }
        public List<ClsCalculo> calculos { get; set; }
        public static ClsCotizacion CotComp { get; set; }
        public string Origen { get; set; }
        public string radio { get; set; }

    }
    public class CoberturasModel
    {
   
      public int GM{ get; set; }
      public string ExDedu{ get; set; }
      //public Cotizador { get; set; }
      public int OrdenCobertura { get; set; }
      public string Nombre { get; set; }
      public string Descripcion { get; set; }
      public int DM { get; set; }
      public int RT { get; set; }
      public string Cober100 { get; set; }
      public string Terssa { get; set; }
      public string ExtensionRC { get; set; }
      public string GarageCasa { get; set; }
      public string GarageTrabajo { get; set; }
      public string Usotrabajo { get; set; }
      public int RC { get; set; }
      public int RCcatastrofica { get; set; }
      public string AseLegal { get; set; }
      public string SumaAseg { get; set; }
      public string VehSus { get; set; }
      public string Aviaje { get; set; }
      public string BajoRiesgo { get; set; }
      public string MasAutos { get; set; }
      public int Procar { get; set; }
      public int Campaña { get; set; }
      public string DetalleCobertura { get; set; }
    }
    public class CedeComision
    {
        public static double ProcarOriginal { set; get; }
        public static double primanetaOriginal { set; get; }
        public static double subtotalOriginal { set; get; }
        public static double totaloriginal { set; get; }
        public static bool cargado { get; set; }
    }

    public class CaptchaImageResult : ActionResult
    {

        public string GetCaptchaString(int length)
        {
            int intZero = '0';
            int intNine = '9';
            int intA = 'A';
            int intZ = 'Z';
            int intI = 'I';
            int intO = 'O';
            int intCount = 0;
            int intRandomNumber = 0;
            string strCaptchaString = "";

            Random random = new Random(System.DateTime.Now.Millisecond);

            while (intCount < length)
            {
                intRandomNumber = random.Next(intZero, intZ);
                if (((intRandomNumber >= intZero) && (intRandomNumber <= intNine) || (intRandomNumber >= intA) && (intRandomNumber <= intZ) && (intRandomNumber != intI && intRandomNumber != intO)))
                {
                    strCaptchaString = strCaptchaString + (char)intRandomNumber;
                    intCount = intCount + 1;
                }
            }
            return strCaptchaString;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(100, 30);
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bmp);
            g.Clear(System.Drawing.Color.Navy);
            string randomString = GetCaptchaString(6);
            context.HttpContext.Session["captchastring"] = randomString;
            #region
            //add noise , if dont want any noisy , then make it false.
            bool noisy = false;
            if (noisy)
            {
                var rand = new Random((int)DateTime.Now.Ticks);
                int i, r, x, y;
                var pen = new System.Drawing.Pen(System.Drawing.Color.Yellow);
                for (i = 1; i < 10; i++)
                {
                    pen.Color = System.Drawing.Color.FromArgb(
                    (rand.Next(0, 255)),
                    (rand.Next(0, 255)),
                    (rand.Next(0, 255)));

                    r = rand.Next(0, (130 / 3));
                    x = rand.Next(0, 130);
                    y = rand.Next(0, 30);

                    int m = x - r;
                    int n = y - r;
                    g.DrawEllipse(pen, m, n, r, r);
                }
            }
            //end noise
            #endregion
            g.DrawString(randomString, new System.Drawing.Font("Courier", 16), new System.Drawing.SolidBrush(System.Drawing.Color.WhiteSmoke), 2, 2);
            System.Web.HttpResponseBase response = context.HttpContext.Response;
            response.ContentType = "image/jpeg";
            bmp.Save(response.OutputStream, System.Drawing.Imaging.ImageFormat.Jpeg);
            bmp.Dispose();

        }
    }
}