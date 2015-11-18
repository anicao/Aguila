using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using CentralAgentesMvc.Models;

namespace CentralAgentesMvc.Controllers
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class SessionExpireFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpContext ctx = HttpContext.Current;

           

                // If the browser session or authentication session has expired...
            if (ctx.Session["UserObj"] == null || !filterContext.HttpContext.Request.IsAuthenticated)
            {
                if (!filterContext.ActionDescriptor.ActionName.Equals("Login"))
                {
                    if (filterContext.ActionDescriptor.ActionName.Equals("ResetPassword"))
                    {
                        base.OnActionExecuting(filterContext);
                        return;
                    }
                    if (!filterContext.ActionDescriptor.ControllerDescriptor.ControllerName.Equals("Cotizacion"))
                    {
                        if (filterContext.HttpContext.Request.IsAjaxRequest())
                        {
                            // For AJAX requests, we're overriding the returned JSON result with a simple string,
                            // indicating to the calling JavaScript code that a redirect should be performed.
                            filterContext.Result = new JsonResult { Data = "_Logon_" };
                        }
                        else
                        {
                            // For round-trip posts, we're forcing a redirect to Home/TimeoutRedirect/, which
                            // simply displays a temporary 5 second notification that they have timed out, and
                            // will, in turn, redirect to the logon page.
                            filterContext.Result = new RedirectResult("~/Account/Login");
                        }
                    }
                    else
                    {

                        if (HttpContext.Current.Session["UserObj"] == null)
                        {
                            filterContext.Result = new RedirectResult("~/Account/Login?returnUrl=" + filterContext.HttpContext.Request.RawUrl.ToString());
                        }

                    }
                }
               
            }
            else {
                var session = (System.Data.DataSet)ctx.Session["UserObj"];
                if (session.Tables[0].Rows[0]["cNombre"].ToString().Trim() == "COTIZADOR EN LINEA") {
                    RutasPublicas rt = new RutasPublicas();
                    if (ctx.Request.QueryString["ReturnUrl"] != null)
                        filterContext.Result = new RedirectResult(ctx.Request.QueryString["ReturnUrl"]);
                    else
                    {
                        var ruta = rt.rutas.Find(x => ctx.Request.RawUrl.ToString().Contains(x.ruta));

                        if (ruta == null)
                        {
                            ctx.Session.RemoveAll();
                            ctx.Session.Abandon();
                            filterContext.Result = new RedirectResult("https://www.elaguila.com.mx/");
                        }
                    }
                }
            }
            base.OnActionExecuting(filterContext);
        }
    }
}