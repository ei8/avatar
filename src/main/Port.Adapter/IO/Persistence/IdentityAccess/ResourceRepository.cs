using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using works.ei8.Avatar.Domain.Model;

namespace works.ei8.Avatar.Port.Adapter.IO.Persistence.IdentityAccess
{
    public class ResourceRepository : IResourceRepository
    {
        private SQLiteAsyncConnection connection;

        public async Task<Resource> GetByPath(string path)
        {
            var results = this.connection.Table<Resource>().Where(e => e.Path == path);
            return (await results.ToListAsync()).SingleOrDefault();
        }

        public async Task Initialize()
        {
            this.connection = await UserRepository.CreateConnection<Resource>();

            //sample data creator - call Initialize from CustomBootstrapper to invoke
            //await this.connection.InsertAsync(new Nucleus()
            //{
            //    Path = "/nuclei/d23/",
            //    InUri = "http://192.168.8.135:60020",
            //    OutUri = "http://192.168.8.135:60021"
            //});
        }
    }
}
