using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using Microsoft.Owin;
using Owin;
using Swashbuckle.Application;
using Taxes.App_Start;

[assembly: OwinStartup(typeof(Taxes.Startup))]

namespace Taxes {
    public class Startup {
        public void Configuration(IAppBuilder app) {
            var config = new HttpConfiguration();

            WebApiConfig.Register(config);

            config.Services.Replace(typeof(IExceptionHandler), new CustomExceptionHandler());

            config.EnableSwagger(c => {
                c.SingleApiVersion("v1", "Taxes API");
            }).EnableSwaggerUi("help/{*assetPath}");

            app.UseWebApi(config);
        }
    }
}
