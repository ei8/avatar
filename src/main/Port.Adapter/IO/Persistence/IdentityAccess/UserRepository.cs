using org.neurul.Common.Domain.Model;
using SQLite;
using System;
using System.Linq;
using System.Threading.Tasks;
using works.ei8.Avatar.Domain.Model;
using works.ei8.Avatar.Port.Adapter.Common;

namespace works.ei8.Avatar.Port.Adapter.IO.Persistence.IdentityAccess
{
    public class UserRepository : IUserRepository
    {
        private SQLiteAsyncConnection connection;

        public async Task<User> GetBySubjectId(Guid subjectId)
        {
            var results = this.connection.Table<User>().Where(e => e.SubjectId == subjectId);
            return (await results.ToListAsync()).SingleOrDefault();
        }

        public async Task Initialize()
        {
            this.connection = await UserRepository.CreateConnection<User>();

            //sample data creator - call Initialize from CustomBootstrapper to invoke
            //await this.connection.InsertAsync(new User()
            //{
            //    NeuronId = Guid.NewGuid(),
            //    SubjectId = Guid.NewGuid()
            //});
        }

        // TODO: Transfer to NeurUL.Common
        internal static async Task<SQLiteAsyncConnection> CreateConnection<TTable>() where TTable : new()
        {
            SQLiteAsyncConnection result = null;
            string databasePath = Environment.GetEnvironmentVariable(EnvironmentVariableKeys.UserDatabasePath);

            if (!databasePath.Contains(":memory:"))
                AssertionConcern.AssertPathValid(databasePath, nameof(databasePath));

            result = new SQLiteAsyncConnection(databasePath);
            await result.CreateTableAsync<TTable>();
            return result;
        }
    }
}
