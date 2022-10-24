using ei8.Cortex.IdentityAccess.Application;
using ei8.Cortex.IdentityAccess.Domain.Model;
using ei8.Cortex.IdentityAccess.Port.Adapter.IO.Persistence.SQLite;
using ei8.Cortex.IdentityAccess.Port.Adapter.IO.Process.Services;
using Nancy;
using Nancy.TinyIoc;

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
        }
    }
}
