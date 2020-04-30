using Nancy;
using Nancy.TinyIoc;
// TODO: using ei8.Cortex.Diary.Nucleus.Application.EventStores;
//using ei8.Cortex.Diary.Nucleus.Application.Notifications;
//using ei8.Cortex.Diary.Nucleus.Port.Adapter.IO.Persistence.Events.SQLite;
//using dmIEventStore = ei8.Cortex.Diary.Nucleus.Domain.Model.IEventStore;

namespace ei8.Avatar.Port.Adapter.Out.Api
{
    public class CustomBootstrapper : DefaultNancyBootstrapper
    {
        public CustomBootstrapper()
        {
        }

        protected override void ConfigureRequestContainer(TinyIoCContainer container, NancyContext context)
        {
            base.ConfigureRequestContainer(container, context);

            // TODO: container.Register<dmIEventStore, EventStore>();
            //container.Register<INotificationApplicationService, NotificationApplicationService>();
            //container.Register<IEventStoreApplicationService, EventStoreApplicationService>();
        }
    }
}
