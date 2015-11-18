using System.Web.Mvc;
using System.Web;
using System.Web.Security;

namespace CentralAgentesMvc.Controllers
{
    public class ElAguilaAuthorizeAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);
            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                if (!filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true) &&
                  !filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true))
                {
                    filterContext.Result = new RedirectResult("~/Account/Login");
                    return;
                }
            }

            if (filterContext.Result is HttpUnauthorizedResult)
            {
                filterContext.Result = new RedirectResult("~/Account/AccessDenied");
            }           
        }
    }
}
