using System;
using System.Threading;
using System.Threading.Tasks;
using works.ei8.Cortex.Sentry.Domain.Model;

namespace works.ei8.Cortex.Sentry.Application
{
    public interface IAuthorApplicationService
    {
        Task<ValidationResult> ValidateWrite(string avatarId, Guid neuronId, Guid newNeuronRegionId, Guid subjectId, CancellationToken token = default(CancellationToken));

        Task<Author> GetAuthorBySubjectId(string avatarId, Guid subjectId, CancellationToken token = default(CancellationToken));
    }
}
