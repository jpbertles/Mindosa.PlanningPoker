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
            //TODO: FUGLY...do something here
            GlobalHost.DependencyResolver.Register(typeof(PlanningHub), () => new PlanningHub(new GroupRepository()));

            app.MapSignalR().UseNancy();
        }
    }
}