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
        Task<ActionValidationResult> CreateNeuron(Guid neuronId, Guid regionId, Guid subjectId, CancellationToken token = default);

        Task<ActionValidationResult> UpdateNeuron(Guid neuronId, Guid subjectId, CancellationToken token = default);

        Task<ActionValidationResult> ReadNeurons(IEnumerable<Guid> neuronIds, Guid subjectId, CancellationToken token = default);
    }
}
