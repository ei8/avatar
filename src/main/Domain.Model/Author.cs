using org.neurul.Common.Domain.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace works.ei8.Avatar.Domain.Model
{
    public class Author
    {
        public Author(User user, IEnumerable<RegionPermit> permits)
        {
            // TODO: Add TDD test
            AssertionConcern.AssertArgumentNotNull(user, nameof(user));
            AssertionConcern.AssertArgumentNotNull(permits, nameof(permits));

            this.User = user;
            this.Permits = permits;
        }

        public User User { get; private set; }

        public IEnumerable<RegionPermit> Permits { get; private set; }
    }
}
