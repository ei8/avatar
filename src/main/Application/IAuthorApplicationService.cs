using System;
using System.Threading;
using System.Threading.Tasks;
using ei8.Avatar.Domain.Model;

namespace ei8.Avatar.Application
{
    public interface IAuthorApplicationService
    {
        Task<ValidationResult> ValidateWrite(Guid neuronId, Guid newNeuronRegionId, Guid subjectId, CancellationToken token = default(CancellationToken));

        Task<Author> GetAuthorBySubjectId(Guid subjectId, CancellationToken token = default(CancellationToken));
    }
}
