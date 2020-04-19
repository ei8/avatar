using Nancy;
using Nancy.Responses;
using Newtonsoft.Json;
// TODO: using works.ei8.Cortex.Diary.Nucleus.Application.EventStores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace works.ei8.Avatar.Port.Adapter.Out.Api
{
    // TODO: public class EventStoreModule : NancyModule
    //{
    //    public EventStoreModule(IEventStoreApplicationService eventStoreService) : base("/{avatarId}/eventsourcing/eventstore")
    //    {
    //        this.Get("/{aggregateid}", async (parameters) => {
    //                int version = 0;
    //                if (this.Request.Query.version.HasValue)
    //                    version = int.Parse(this.Request.Query.version);
    //                var notifs = await eventStoreService.Get(parameters.avatarId, Guid.Parse(parameters.aggregateId), version);
    //                return new TextResponse(JsonConvert.SerializeObject(notifs));
    //            }
    //        );
    //    }
    //}
}
