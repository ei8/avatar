using Nancy;
using Nancy.TinyIoc;
using org.neurul.Common.Http;
using works.ei8.Cortex.Graph.Client;
using works.ei8.Avatar.Application;
using works.ei8.Avatar.Domain.Model;
using works.ei8.Avatar.Port.Adapter.IO.Persistence.IdentityAccess;
using works.ei8.Avatar.Port.Adapter.IO.Process.Services;
using works.ei8.EventSourcing.Client.Out;

namespace works.ei8.Avatar.Port.Adapter.In.Api
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
