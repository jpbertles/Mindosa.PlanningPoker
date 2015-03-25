using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nancy.TinyIoc;

namespace Mindosa.PlanningPoker.Infrastructure
{
    public class TinyIocDependencyResolver : DefaultDependencyResolver
    {
        public TinyIocDependencyResolver()
        {
        }

        public override object GetService(Type serviceType)
        {
            return TinyIoCContainer.Current.CanResolve(serviceType) ? TinyIoCContainer.Current.Resolve(serviceType) : base.GetService(serviceType);
        }

        public override IEnumerable<object> GetServices(Type serviceType)
        {
            var objects = TinyIoCContainer.Current.CanResolve(serviceType) ? TinyIoCContainer.Current.ResolveAll(serviceType) : new object[] { };
            return objects.Concat(base.GetServices(serviceType));
        }
    }
}