using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using works.ei8.Avatar.Domain.Model;

namespace works.ei8.Avatar.Application
{
    public interface IResourceApplicationService
    {
        Task<Resource> GetByPath(string path, CancellationToken token = default(CancellationToken));
    }
}
