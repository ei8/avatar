using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace works.ei8.Cortex.Sentry.Domain.Model
{
    public interface IRegionPermitRepository
    {
        // TODO: change to GetAllApplicableByUserNeuronId - will include layers that have a Guid.Empty as UserNeuronId to indicate permits that apply to all users
        Task<IEnumerable<RegionPermit>> GetAllByUserNeuronId(Guid userNeuronId);

        Task Initialize();
    }
}
