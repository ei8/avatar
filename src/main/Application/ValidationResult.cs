using System;
using System.Collections.Generic;
using System.Text;

namespace ei8.Avatar.Application
{
    public struct ValidationResult
    {
        public ValidationResult(IEnumerable<string> messages, bool success)
        {
            this.Messages = messages;
            this.Success = success;
        }

        public IEnumerable<string> Messages { get; private set; }

        public bool Success { get; private set; }
    }
}
