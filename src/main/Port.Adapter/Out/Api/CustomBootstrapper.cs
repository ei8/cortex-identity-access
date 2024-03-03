using Nancy;
using Nancy.TinyIoc;
using neurUL.Common.Http;
using ei8.Cortex.Graph.Client;
using ei8.Cortex.IdentityAccess.Application;
using ei8.Cortex.IdentityAccess.Domain.Model;
using ei8.Cortex.IdentityAccess.Port.Adapter.IO.Persistence.SQLite;
using ei8.Cortex.IdentityAccess.Port.Adapter.IO.Process.Services;
using ei8.EventSourcing.Client.Out;

namespace ei8.Cortex.IdentityAccess.Port.Adapter.Out.Api
{
    public class CustomBootstrapper : DefaultNancyBootstrapper
    {
        public CustomBootstrapper()
        {
        }

        protected override void ConfigureRequestContainer(TinyIoCContainer container, NancyContext context)
        {
            base.ConfigureRequestContainer(container, context);

            container.Register<IRequestProvider, RequestProvider>();
            container.Resolve<IRequestProvider>().SetHttpClientHandler(new System.Net.Http.HttpClientHandler());

            container.Register<INeuronGraphQueryClient, HttpNeuronGraphQueryClient>();
            container.Register<IAuthorRepository, AuthorRepository>();
            container.Register<ISettingsService, SettingsService>();
            container.Register<INeuronPermitRepository, NeuronPermitRepository>();
            container.Register<INotificationClient, HttpNotificationClient>();
            container.Register<IValidationService, ValidationService>();
            container.Register<IAuthorApplicationService, AuthorApplicationService>();
            container.Register<IValidationApplicationService, ValidationApplicationService>();
            container.Register<IPermitApplicationService, PermitApplicationService>();
        }
    }
}
