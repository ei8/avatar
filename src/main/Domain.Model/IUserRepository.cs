using System;
using System.Threading.Tasks;

namespace ei8.Avatar.Domain.Model
{
    public interface IUserRepository
    {
        Task<User> GetBySubjectId(Guid subjectId);

        Task Initialize();
    }
}
