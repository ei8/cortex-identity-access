using System;
using System.Threading;
using System.Threading.Tasks;

namespace ei8.Cortex.IdentityAccess.Domain.Model
{
    public interface INeuronValidationService
    {
        Task<bool> Exists(Guid neuronId, CancellationToken token = default);
    }
}
