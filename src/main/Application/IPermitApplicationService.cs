using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ei8.Cortex.IdentityAccess.Application
{
    public interface IPermitApplicationService
    {
        Task GrantNeuronPermitAccessAsync(Guid neuronId, Guid userNeuronId, TimeSpan duration);

        Task CreateNeuronPermitAsync(Guid neuronId, Guid userNeuronId, DateTime? expirationDate = null);

        Task<IEnumerable<Guid>> GetNeuronIdsByUserNeuronIds(IEnumerable<Guid> userNeuronIds, IEnumerable<Guid> filterNeuronIds = null);
    }
}
