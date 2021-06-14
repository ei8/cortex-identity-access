using ei8.Cortex.IdentityAccess.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ei8.Cortex.IdentityAccess.Domain.Model
{
    public interface IValidationService
    {
        Task<ActionValidationResult> CreateNeuron(Guid neuronId, Guid? neuronRegionId, string userId, CancellationToken token = default);

        Task<ActionValidationResult> UpdateNeuron(Guid neuronId, string userId, CancellationToken token = default);

        Task<ActionValidationResult> ReadNeurons(IEnumerable<Guid> neuronIds, string userId, CancellationToken token = default);
    }
}
