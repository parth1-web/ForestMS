using Autofac;
using LE.Common.Provider;
using System;
using System.Linq;

namespace LE.Web.Autofac.Modules
{
    public class AutofacModule : global::Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // Controllers with [FromServices]-style public properties (BaseController
            // repositories) need property injection. Register every controller in
            // this assembly instead of maintaining the hand-written list (which missed
            // several BaseController-derived controllers and double-registered the
            // abstract BaseController itself).
            var assembly = typeof(AutofacModule).Assembly;
            builder.RegisterAssemblyTypes(assembly)
                   .Where(t => t.Name.EndsWith("Controller", StringComparison.Ordinal) && !t.IsAbstract)
                   .PropertiesAutowired();

            builder.RegisterType<DbConnectionProvider>().As<IConnectionProvider>();
        }
    }
}
