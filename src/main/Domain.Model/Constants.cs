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
                public const string UserNeuronNotFound = "User Neuron not found.";
                public const string UserNeuronInactive = "User Neuron inactive.";
                public const string NeuronNotFound = "Neuron not found in graph.";
                public const string UnauthorizedRegionWriteTemplate = "User not authorized to write to Region '{0}'.";
                public const string UnauthorizedNeuronWriteTemplate = "User must be Region Admin or Neuron Creator to modify Neuron '{0}'.";
                public const string UnauthorizedRegionReadTemplate = "User not authorized to read from Region '{0}'.";
                public const string ExpiredNeuronPermitTemplate = "User permit to read Neuron expired on '{0}'.";
                public const string NeuronPermitPendingOrDisabled = "Neuron permit is pending approval or has been disabled.";
                public const string NeuronPermitAlreadyExists = "Neuron permit for specified Neuron ID and User Neuron ID already exists.";
                public const string EnumerableNullOrEmpty = "Specified Enumerable is null or empty.";
            }
        }
    }
}
