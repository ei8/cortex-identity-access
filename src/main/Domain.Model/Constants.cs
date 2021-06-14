using System;
using System.Collections.Generic;
using System.Text;

namespace ei8.Cortex.IdentityAccess.Domain.Model
{
    public struct Constants
    {
        public struct Messages
        {
            public struct Exception
            {
                public const string InvalidId = "Id must not be equal to '00000000-0000-0000-0000-000000000000'.";
                public const string InvalidUserId = "User Id must not be null or empty.";
                public const string AnonymousUserExpected = "Anonymous User expected.";
                public const string UnauthorizedUserAccess = "User access not authorized.";
                public const string NeuronNotFound = "User Neuron not found.";
                public const string NeuronInactive = "User Neuron inactive.";
                public const string UnauthorizedRegionWriteTemplate = "User not authorized to write to Region '{0}'.";
                public const string UnauthorizedNeuronWriteTemplate = "User must be Region Admin or Neuron Creator to modify Neuron '{0}'.";
                public const string UnauthorizedRegionReadTemplate = "User not authorized to read from Region '{0}'.";
            }
        }
    }
}
