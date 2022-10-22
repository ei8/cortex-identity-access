using CQRSlite.Commands;
using CQRSlite.Routing;
using ei8.Cortex.IdentityAccess.Application;
using ei8.Cortex.IdentityAccess.Application.AccessRequest;
using ei8.Cortex.IdentityAccess.Domain.Model;
using ei8.Cortex.IdentityAccess.Port.Adapter.IO.Persistence.SQLite;
using ei8.Cortex.IdentityAccess.Port.Adapter.IO.Process.Services;
using Nancy;
using Nancy.TinyIoc;
using neurUL.Common.Http;
using System;

namespace ei8.Cortex.IdentityAccess.Port.Adapter.In.Api
{
    public class CustomBootstrapper : DefaultNancyBootstrapper
    {
        public CustomBootstrapper()
        {
        }

        protected override void ConfigureRequestContainer(TinyIoCContainer container, NancyContext context)
        {
            base.ConfigureRequestContainer(container, context);

            container.Register<IPermitApplicationService, PermitApplicationService>();
            container.Register<INeuronPermitRepository, NeuronPermitRepository>();
            container.Register<ISettingsService, SettingsService>();

            // create a singleton instance which will be reused for all calls in current request
            var ipb = new Router();
            container.Register<ICommandSender, Router>(ipb);
            container.Register<IHandlerRegistrar, Router>(ipb);

            container.Register<AccessRequestCommandHandlers>();

            var ticl = new TinyIoCServiceLocator(container);
            container.Register<IServiceProvider, TinyIoCServiceLocator>(ticl);
            var registrar = new RouteRegistrar(ticl);
            registrar.Register(typeof(AccessRequestCommandHandlers));

            ((TinyIoCServiceLocator)container.Resolve<IServiceProvider>()).SetRequestContainer(container);
        }
    }
}
