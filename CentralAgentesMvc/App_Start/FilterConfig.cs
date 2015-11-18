using System.Web;
using System.Web.Mvc;
using CentralAgentesMvc.Controllers;

namespace CentralAgentesMvc
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new System.Web.Mvc.AuthorizeAttribute());
            filters.Add(new SessionExpireFilterAttribute());
            filters.Add(new ElAguilaAuthorizeAttribute());
            
        }
    }
}