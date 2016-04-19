using System.Web.Http;
using WebActivatorEx;
using jajs.API;
using Swashbuckle.Application;

[assembly: PreApplicationStartMethod(typeof(SwaggerConfig), "Register")]

namespace jajs.API
{
	public class SwaggerConfig
	{
		public static void Register()
		{
			var thisAssembly = typeof(SwaggerConfig).Assembly;

			GlobalConfiguration.Configuration
                .EnableSwagger(c => { c.SingleApiVersion("v1", "jajs.API");
                c.IncludeXmlComments(string.Format(@"{0}\bin\jajs.API.XML",           
                           System.AppDomain.CurrentDomain.BaseDirectory)); })
				.EnableSwaggerUi(c => { });
		}
	}
}
