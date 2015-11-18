using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CentralAgentesMvc.Models
{
    public class RutasPublicas
    {
        public List<rutasConfig> rutas { get; set; }

      public RutasPublicas() {
          this.rutas = new List<rutasConfig>();
          this.rutas.Add(new rutasConfig() { ruta = "/Cotizacion/CalculoCotizacion" });
          this.rutas.Add(new rutasConfig() { ruta = "/Cotizacion/ObtenMarcaVehi" });
          this.rutas.Add(new rutasConfig() { ruta = "/Cotizacion/ObtenSubMarcaVehi" });
          this.rutas.Add(new rutasConfig() { ruta = "/Cotizacion/ObtenTipoVehi" });
          this.rutas.Add(new rutasConfig() { ruta = "/Cotizacion/ShowCaptchaImage" });
          this.rutas.Add(new rutasConfig() { ruta = "/Cotizacion/ShowCaptchaImage" });
          this.rutas.Add(new rutasConfig() { ruta = "/Cotizacion/ResultadoCotizacion" });
          this.rutas.Add(new rutasConfig() { ruta = "/Cotizacion/GuardaCotExpress" });
          this.rutas.Add(new rutasConfig() { ruta = "/Cotizacion/PlanSelectedExpress" });
          this.rutas.Add(new rutasConfig() { ruta = "/Cotizacion/PagoEnLineaExpress" });      
          this.rutas.Add(new rutasConfig() { ruta = "/General/DireccionExp" });
          this.rutas.Add(new rutasConfig() { ruta = "/General/ValidatePostalCode" });
          this.rutas.Add(new rutasConfig() { ruta = "/Cotizacion/PagoEnLineaExpress" });
          this.rutas.Add(new rutasConfig() { ruta = "/General/ValidatePostalCode" });
          this.rutas.Add(new rutasConfig() { ruta = "/General/UpdateDireccion" });
          this.rutas.Add(new rutasConfig() { ruta = "/General/DireccionExp" });
          this.rutas.Add(new rutasConfig() { ruta = "/General/SeriePlacaExp" });
          this.rutas.Add(new rutasConfig() { ruta = "/General/UpdatePlacaVehiculo" });
          this.rutas.Add(new rutasConfig() { ruta = "/General/DesgloseExp" });
          this.rutas.Add(new rutasConfig() { ruta = "/General/ValidaTarjeta" });
          this.rutas.Add(new rutasConfig() { ruta = "/General/UpdateDesglosePago" });
          this.rutas.Add(new rutasConfig() { ruta = "/Cotizacion/preViewPago" });
          this.rutas.Add(new rutasConfig() { ruta = "/General/GuardarDocumento" });
          this.rutas.Add(new rutasConfig() { ruta = "/General/AplicaSrPago" });
          this.rutas.Add(new rutasConfig() { ruta = "/Cotizacion/CotizadorEnLinea" });
          this.rutas.Add(new rutasConfig() { ruta = "/Cotizacion/ServiciosEnLinea" });
          this.rutas.Add(new rutasConfig() { ruta = "/Cotizacion/GetDataService" });
          this.rutas.Add(new rutasConfig() { ruta = "/Siniestro/showSiniestro" });
          this.rutas.Add(new rutasConfig() { ruta = "/Recibos/pagosPendientes" });
          this.rutas.Add(new rutasConfig() { ruta = "/Siniestro/details" });
          this.rutas.Add(new rutasConfig() { ruta = "/Recibos/pagosPendientes" });
          this.rutas.Add(new rutasConfig() { ruta = "/Cotizacion/SendMailExpress" });
        }
    }
    public class rutasConfig {
        public String ruta { get; set; }
    }
}