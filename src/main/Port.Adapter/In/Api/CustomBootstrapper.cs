using Nancy;
using Nancy.TinyIoc;
using neurUL.Common.Http;
using ei8.Cortex.Graph.Client;
using ei8.Avatar.Application;
using ei8.Avatar.Domain.Model;
using ei8.Avatar.Port.Adapter.IO.Persistence.IdentityAccess;
using ei8.Avatar.Port.Adapter.IO.Process.Services;
using ei8.EventSourcing.Client.Out;

namespace ei8.Avatar.Port.Adapter.In.Api
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
            container.Register<INeuronGraphQueryClient, HttpNeuronGraphQueryClient>();
            container.Register<IUserRepository, UserRepository>();
            container.Register<IRegionPermitRepository, RegionPermitRepository>();
            container.Register<IResourceRepository, ResourceRepository>();
            container.Register<ISettingsService, SettingsService>();
            container.Register<INotificationClient, HttpNotificationClient>();
            container.Register<IAuthorApplicationService, AuthorApplicationService>();
            container.Register<IResourceApplicationService, ResourceApplicationService>();
        }
    }
}
