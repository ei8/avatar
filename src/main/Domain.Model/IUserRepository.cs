﻿using System;
using System.Threading.Tasks;

namespace works.ei8.Avatar.Domain.Model
{
    public interface IUserRepository
    {
        Task<User> GetBySubjectId(Guid subjectId);

        Task Initialize();
    }
}
