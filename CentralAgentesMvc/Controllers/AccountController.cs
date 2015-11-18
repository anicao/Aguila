using System;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.Web.Security;
using System.Collections.Generic;
using WebMatrix.WebData;
using System.Threading.Tasks;
//===================================
using CentralAgentesMvc.Models;
using Negocio.AccesosYPermisos;
using Negocio.ClasesCentral;
using Negocio.HerramientasVarios;
//===================================


namespace CentralAgentesMvc.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        #region <-- Login / Logout Methods -->
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            try
            {
                if (!Request.RawUrl.Contains("CotExpress") && !Request.RawUrl.Contains("CotizadorEnLinea") && !Request.RawUrl.Contains("ServiciosEnLinea"))
                {
                    Session["UserObj"] = null;
                    Session["UserRol"] = "";
                    Session["Periodo"] = "";

                    ViewBag.ReturnUrl = returnUrl;
                    return View();
                }
                else
                {
                    ViewBag.ReturnUrl = "Cotizar";
                    LoginViewModel UsuarioExt = new LoginViewModel();
                    UsuarioExt.UserName = "4";
                    UsuarioExt.Password = "a";
                    Login(UsuarioExt, Request.RawUrl.Remove(0,Request.RawUrl.IndexOf("=/")+1));
                    return Redirect(Request.RawUrl.Remove(0,Request.RawUrl.IndexOf("=/")+1));
                }
            }
            catch (Exception err)
            {
                Response.StatusCode = (int)(System.Net.HttpStatusCode.InternalServerError);
                LogDeErrores.RegistroErrorWeb(err, DatoUsuario.idAgente, DatoUsuario.nomAgente, DatoUsuario.sIp, DatoUsuario.sHostName);
                return Json("ErrInterno");
            }
        }

        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            try
            {
                Negocio.HerramientasVarios.MensajesError.tipo = "T";
                Negocio.HerramientasVarios.LogDeErrores.soloWeb = "I";
                Boolean singIng = SignIn(model.UserName, model.Password, persistCookie: model.RememberMe);
                if (!model.UserName.Equals("2362") && !model.Password.Equals("2362"))
                {
                    if (model.UserName.Equals(model.Password) && singIng)
                    {
                        Session["UserObj"] = null;
                        Session["UserRol"] = "";
                        Session["Periodo"] = "";
                        Session["ChangePassword"] = null;
                        FormsAuthentication.SignOut();
                        Session.Abandon();
                        ModelState.AddModelError("", "Aún no tiene acceso al portal de agentes, por favor comunicarse con su ejecutiva de servicio.");
                        return View(model);
                    }
                }
                if (ModelState.IsValid && singIng)
                {
                    var slogged = (DataSet)Session["UserObj"];
                    DatoUsuario.idAgente = slogged.Tables["catAgentes"].Rows[0]["nAgenteID"].ToString();
                    DatoUsuario.nomAgente = slogged.Tables["catAgentes"].Rows[0]["cNombre"].ToString().Trim();
                    DatoUsuario.sHostName = Dns.GetHostName().ToUpper(); ;
                    DatoUsuario.sIp = ControllerContext.HttpContext.Request.UserHostAddress;
                    DatoUsuario.campaña = int.Parse(slogged.Tables["catAgentes"].Rows[0]["campaña"].ToString().Trim());

                    Negocio.HerramientasVarios.LogDeErrores.agenteWeb = DatoUsuario.idAgente;
                    return RedirectToLocal(returnUrl);
                }

                ModelState.AddModelError("", "El usuario o contraseña proporcionado son incorrectos");
                return View(model);
            }
            catch (Exception err)
            {
                Response.StatusCode = (int)(System.Net.HttpStatusCode.InternalServerError);
                LogDeErrores.RegistroErrorWeb(err, DatoUsuario.idAgente, DatoUsuario.nomAgente, DatoUsuario.sIp, DatoUsuario.sHostName);
                return Json("ErrInterno");
            }
        }

        // POST: /Account/LogOff
        public ActionResult LogOff()
        {
            Session["UserObj"] = null;
            Session["UserRol"] = "";
            Session["Periodo"] = "";

            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult ResetPassword(string userID)
        {
            // Envio el correo
            LoginViewModel model = new LoginViewModel();
             ClsAgente agt = new ClsAgente();
             if (agt.UpdateAgenteWeb(Convert.ToInt32(userID), "agente" + userID, ""))
             {
                 ModelState.AddModelError("", "Se restablecio su contraseña correctamente");
                 return Content("OK");
             }
             return Content("OK");
        }
        #endregion

        #region <-- UserProfile / ResetPassword Methods -->
        // GET: /Account/ResetPassword
        public ActionResult UserProfile()
        {
            try
            {
                var logged = (DataSet)Session["UserObj"];
                var profile = new UserProfileViewModel()
                {
                    AgenteID = logged.Tables["catAgentes"].Rows[0]["nAgenteID"].ToString(),
                    AgenteName = logged.Tables["catAgentes"].Rows[0]["cNombre"].ToString(),
                    Credencial = logged.Tables["catAgentes"].Rows[0]["cCredencial"].ToString(),
                    Email = logged.Tables["catAgentes"].Rows[0]["Email"].ToString(),
                    ActualPassword = logged.Tables["catAgentes"].Rows[0]["cPasww"].ToString().Trim(),

                    OldPassword = "",
                    PasswordHash = "",
                    ConfirmPassword = "",
                };

                return PartialView(profile);
            }
            catch (Exception err)
            {
                Response.StatusCode = (int)(System.Net.HttpStatusCode.InternalServerError);
                LogDeErrores.RegistroErrorWeb(err, DatoUsuario.idAgente, DatoUsuario.nomAgente, DatoUsuario.sIp, DatoUsuario.sHostName);
                return Json("ErrInterno");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UserProfile(UserProfileViewModel model)
        {
            try
            {
                if (model.CaptchaText == HttpContext.Session["captchastring"].ToString())
                {
                    ViewBag.Message = "CAPTCHA correcto!";
                }
                else
                {
                    ViewBag.Message = "CAPTCHA incorrecto!";
                    ModelState["CaptchaText"].Errors.Add("CAPTCHA incorrecto!");
                }

                if (!ModelState.IsValid)
                {
                    var errors = new Dictionary<string, object>();
                    foreach (var key in ModelState.Keys)
                    {
                        if (ModelState[key].Errors.Count > 0)
                        {
                            errors[key] = ModelState[key].Errors;
                        }
                        else
                            errors[key] = "";
                    }

                    ModelState.Clear();
                    return Json(new { success = false, errores = errors, validaciones = "", view = model });
                }


                ModelState.Clear();

                // Guardo el cambio
                ClsAgente agt = new ClsAgente();
                if (agt.UpdateAgenteWeb(Convert.ToInt32(model.AgenteID), model.ConfirmPassword, model.Email))
                {
                    var logged = (DataSet)Session["UserObj"];
                    logged.Tables["catAgentes"].Rows[0]["Email"] = model.Email;
                    logged.Tables["catAgentes"].Rows[0]["cPasww"] = model.ConfirmPassword;

                    Session["UserObj"] = logged;
                    Session["ChangePassword"] = null;
                    //*cambio aqui
                }

                return PartialView(model);
            }
            catch (Exception err)
            {
                Response.StatusCode = (int)(System.Net.HttpStatusCode.InternalServerError);
                LogDeErrores.RegistroErrorWeb(err, DatoUsuario.idAgente, DatoUsuario.nomAgente, DatoUsuario.sIp, DatoUsuario.sHostName);
                return Json("ErrInterno");
            }
        }
        #endregion

        #region <-- Private Methods -->
        private void ClearStateError(string key)
        {
            ModelState state = ModelState[key];
            if (state != null)
            {
                state.Errors.Clear();
            }
        }
        #endregion

        #region <-- Helpers -->
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
        }

        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion

        #region <-- Helper IAuthenticationManager -->
        private bool SignIn(string userName, string password, bool persistCookie = false)
        {
            FormsAuthentication.SignOut();
            bool success = false;
            Logear acceso = new Logear();

            var userNameOld = (userName.ToLower() == "test" ? "0" : userName);
            var passwordOld = (password == "Test2015k" ? "0" : password);

            var user = acceso.LoginAgenteWeb(int.Parse(userNameOld), passwordOld);

            if (user != null)
            {
                string rol = "";
                string name = user.Tables["catAgentes"].Rows[0]["cNombre"].ToString();
                if (user.Tables["catAgentes"].Rows[0]["nAgenteID"].ToString() == "0")
                {
                    rol = "A";
                    name = (userName.ToLower() == "test" ? "Usuario Test" : "Nancy Martinez");
                }

                VarProcAMC ProcAMC = new VarProcAMC();
                if (!ProcAMC.IniciarCarga())
                {
                    Session["UserObj"] = null;
                    ModelState.AddModelError("", "Error en inicio de la carga de variables");
                }
                else
                {
                    VarProcCentral proCentral = new VarProcCentral();
                    proCentral.IniciaDatos(VarProcAMC.strPeriodo);
                    AccesoSistema accesos = new AccesoSistema();
                    accesos.P_AccesosSistema("03", "E");

                    Session["UserObj"] = user;
                    Session["Periodo"] = "Periodo: " + VarProcAMC.strPeriodo;
                    Session["UserRol"] = rol;
                    if (password.Equals("agente" + userName))
                    {
                        Session["ChangePassword"] = 1;
                    }
                    else {
                        Session["ChangePassword"] = 0;
                    }


                    FormsAuthentication.SetAuthCookie(name, false);
                    success = true;
                }
            }

            return success;
        }
        #endregion
    }
}
