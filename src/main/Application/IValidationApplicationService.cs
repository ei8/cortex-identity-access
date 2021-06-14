using ei8.Cortex.IdentityAccess.Common;
using ei8.Cortex.IdentityAccess.Domain.Model;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ei8.Cortex.IdentityAccess.Application
{
    public interface IValidationApplicationService
    {
        Task<ActionValidationResult> CreateNeuron(Guid neuronId, Guid? regionId, string userId, CancellationToken token = default);

        Task<ActionValidationResult> UpdateNeuron(Guid neuronId, string userId, CancellationToken token = default);

        Task<ActionValidationResult> ReadNeurons(IEnumerable<Guid> neuronIds, string userId, CancellationToken token = default);
    }
}
