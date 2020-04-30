using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ei8.Avatar.Domain.Model
{
    public interface IResourceRepository
    {
        Task<Resource> GetByPath(string path);

        Task Initialize();
    }
}
