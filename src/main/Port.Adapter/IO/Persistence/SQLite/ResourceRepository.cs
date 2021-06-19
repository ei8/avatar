using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ei8.Avatar.Domain.Model;
using ei8.Avatar.Port.Adapter.Common;
using neurUL.Common.Domain.Model;

namespace ei8.Avatar.Port.Adapter.IO.Persistence.SQLite
{
    public class ResourceRepository : IResourceRepository
    {
        private SQLiteAsyncConnection connection;

        public async Task<IEnumerable<Resource>> GetResources()
        {
            var results = this.connection.Table<Resource>();
            return await results.ToArrayAsync();
        }

        public async Task Initialize()
        {
            this.connection = await ResourceRepository.CreateConnection<Resource>();

            //sample data creator - call Initialize from CustomBootstrapper to invoke
            //await this.connection.InsertAsync(new Resource()
            //{
            //    Path = "/nuclei/d23",
            //    InUri = "http://192.168.8.131:60020",
            //    OutUri = "http://192.168.8.131:60021"
            //});
        }

        internal static async Task<SQLiteAsyncConnection> CreateConnection<TTable>() where TTable : new()
        {
            SQLiteAsyncConnection result = null;
            string databasePath = Environment.GetEnvironmentVariable(EnvironmentVariableKeys.ResourceDatabasePath);

            if (!databasePath.Contains(":memory:"))
                AssertionConcern.AssertPathValid(databasePath, nameof(databasePath));

            result = new SQLiteAsyncConnection(databasePath);
            await result.CreateTableAsync<TTable>();
            return result;
        }
    }
}
