using System.Web;
using System.Web.Http;
using System.Web.UI.WebControls;
using ExcelConverter.Areas.HelpPage;
using ExcelConverter.Repositories.Implementations;
using ExcelConverter.Repositories.Interfaces;
using Microsoft.Practices.Unity;
using RouteParameter = System.Web.Http.RouteParameter;

namespace ExcelConverter
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            //idea based on http://www.asp.net/web-api/overview/advanced/dependency-injection
            var container = new UnityContainer();
            container.RegisterType<IExcelRepository, SQLiteRepository>(new HierarchicalLifetimeManager());
            config.DependencyResolver = new UnityResolver(container);
            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            //config.SetDocumentationProvider(new XmlDocumentationProvider(
            //    HttpContext.Current.Server.MapPath("~/App_Data/ExcelConverter.XML")));
        }
    }
}
