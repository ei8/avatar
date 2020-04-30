using System;
using System.Collections.Generic;
using System.Text;

namespace ei8.Avatar.Application
{
    public struct Constants
    {
        public struct Messages
        {
            public struct Exception
            {
                public const string PathInvalid = "Specified Resource Path cannot be null or empty.";
                public const string InvalidId = "Id must not be equal to '00000000-0000-0000-0000-000000000000'.";
                public const string UnauthorizedUserAccess = "User access not authorized.";
                public const string NeuronNotFound = "User Neuron not found.";
                public const string UnauthorizedLayerWriteTemplate = "User must be authorized to write to Layer '{0}'.";
                public const string UnauthorizedNeuronWriteTemplate = "User must be Layer Admin or Neuron Creator to modify Neuron '{0}'.";
            }
        }
    }
}
