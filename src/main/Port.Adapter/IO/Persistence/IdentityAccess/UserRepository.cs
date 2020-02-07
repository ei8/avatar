using org.neurul.Common.Domain.Model;
using SQLite;
using System;
using System.Linq;
using System.Threading.Tasks;
using works.ei8.Cortex.Sentry.Domain.Model;
using works.ei8.Cortex.Sentry.Port.Adapter.Common;

namespace works.ei8.Cortex.Sentry.Port.Adapter.IO.Persistence.IdentityAccess
{
    public class UserRepository : IUserRepository
    {
        private SQLiteAsyncConnection connection;

        public async Task<User> GetBySubjectId(Guid subjectId)
        {
            var results = this.connection.Table<User>().Where(e => e.SubjectId == subjectId);
            return (await results.ToListAsync()).SingleOrDefault();
        }

        public async Task Initialize(string storeId)
        {
            AssertionConcern.AssertArgumentNotNull(storeId, nameof(storeId));
            AssertionConcern.AssertArgumentNotEmpty(storeId, $"'{nameof(storeId)}' cannot be empty.", nameof(storeId));

            this.connection = await UserRepository.CreateConnection<User>(storeId);

            //sample data creator - call Initialize from CustomBootstrapper to invoke
            //await this.connection.InsertAsync(new User()
            //{
            //    NeuronId = Guid.NewGuid(),
            //    SubjectId = Guid.NewGuid()
            //});
        }

        // TODO: Transfer to NeurUL.Common
        internal static async Task<SQLiteAsyncConnection> CreateConnection<TTable>(string storeId) where TTable : new()
        {
            SQLiteAsyncConnection result = null;
            string databasePath = string.Format(Environment.GetEnvironmentVariable(EnvironmentVariableKeys.UserDatabasePath), storeId);

            if (!databasePath.Contains(":memory:"))
                AssertionConcern.AssertPathValid(databasePath, nameof(databasePath));

            result = new SQLiteAsyncConnection(databasePath);
            await result.CreateTableAsync<TTable>();
            return result;
        }
    }
}
