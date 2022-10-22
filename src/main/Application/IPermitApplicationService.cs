using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ei8.Cortex.IdentityAccess.Application
{
    public interface IPermitApplicationService
    {
        Task GrantAccess(Guid neuronId, Guid userNeuronId, TimeSpan duration);

        Task CreateRequestAsync(Guid neuronId, Guid userNeuronId);
    }
}
