using Nancy;
using Nancy.TinyIoc;
using org.neurul.Common.Http;
using works.ei8.Cortex.Graph.Client;
using works.ei8.Cortex.Sentry.Application;
using works.ei8.Cortex.Sentry.Domain.Model;
using works.ei8.Cortex.Sentry.Port.Adapter.IO.Persistence.IdentityAccess;

namespace works.ei8.Cortex.Sentry.Port.Adapter.In.Api
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
            // TODO: retrieve baseUrl from environmentvariables, ISettingsService similar to diary - nucleus and nucleus
            container.Register<IAuthorApplicationService>((c, n) => new AuthorApplicationService(
                string.Empty,
                c.Resolve<INeuronGraphQueryClient>(),
                c.Resolve<IUserRepository>(),
                c.Resolve<IRegionPermitRepository>()
                )
            );
        }
    }
}
