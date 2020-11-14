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
        Task<ActionValidationResult> CreateNeuron(Guid neuronId, Guid? neuronRegionId, Guid subjectId, CancellationToken token = default);

        Task<ActionValidationResult> UpdateNeuron(Guid neuronId, Guid subjectId, CancellationToken token = default);

        Task<ActionValidationResult> ReadNeurons(IEnumerable<Guid> neuronIds, Guid subjectId, CancellationToken token = default);
    }
}
