using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Mindosa.PlanningPoker.Hubs;
using Mindosa.PlanningPoker.Infrastructure.Repositories;
using Owin;

[assembly: OwinStartup(typeof(Mindosa.PlanningPoker.Startup))]
namespace Mindosa.PlanningPoker
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);

            /*
             * The TinyIocDependencyResolver apparently can't resolve the PlanningHub
             * Largely pulled from http://stackoverflow.com/questions/13817794/signalr-nancyfx-integration
             * I also tried using a Bootstrapper as in the example, but it doesn't seem to matter.
             * TinyIoCContainer.CanResolve(typeof(PlanningHub)) returns false...so no dice
             * */
            GlobalHost.DependencyResolver.Register(typeof(PlanningHub), () => new PlanningHub(new GroupRepository()));

            var hubConfiguration = new HubConfiguration {EnableDetailedErrors = true, EnableJSONP = true};
            app.MapSignalR(hubConfiguration).UseNancy();
        }
    }
}