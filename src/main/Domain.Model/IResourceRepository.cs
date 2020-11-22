using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ei8.Avatar.Domain.Model
{
    public interface IResourceRepository
    {
        Task<IEnumerable<Resource>> GetResources();

        Task Initialize();
    }
}
