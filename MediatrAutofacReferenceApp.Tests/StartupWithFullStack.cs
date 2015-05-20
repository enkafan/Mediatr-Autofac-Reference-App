using System.Web.Http;
using Autofac;
using MediatrAutofacReferenceApp.Web;
using Owin;

namespace MediatrAutofacReferenceApp.Tests
{
    public class StartupWithFullStack
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            // wire up the http configuration
            var config = new HttpConfiguration();
            WebApiConfig.Register(config);

            // configure autofac
            var builder = new ContainerBuilder();
            var container = DependencyInjection.Configure(builder, config);

            // wire up owin
            appBuilder.UseAutofacMiddleware(container);
            appBuilder.UseAutofacWebApi(config);
            appBuilder.UseWebApi(config);
        }
    }
}