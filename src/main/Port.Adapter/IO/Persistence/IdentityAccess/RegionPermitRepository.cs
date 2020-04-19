using org.neurul.Common.Domain.Model;
using SQLite;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using works.ei8.Avatar.Domain.Model;

namespace works.ei8.Avatar.Port.Adapter.IO.Persistence.IdentityAccess
{
    public class RegionPermitRepository : IRegionPermitRepository
    {
        private SQLiteAsyncConnection connection;

        public async Task<IEnumerable<RegionPermit>> GetAllByUserNeuronId(Guid userNeuronId)
        {
            var results = this.connection.Table<RegionPermit>().Where(e => e.UserNeuronId == userNeuronId);
            return (await results.ToArrayAsync());
        }

        public async Task Initialize()
        {
            this.connection = await UserRepository.CreateConnection<RegionPermit>();

            //sample data creator - call Initialize from CustomBootstrapper to invoke
            //await this.connection.InsertAsync(new LayerPermit()
            //{
            //    UserNeuronId = Guid.NewGuid(),
            //    LayerNeuronId = Guid.NewGuid(),
            //    CanWrite = true,
            //    CanRead = true
            //});
        }
    }
}
