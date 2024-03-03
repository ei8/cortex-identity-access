using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ei8.Cortex.IdentityAccess.Domain.Model
{
    public interface INeuronPermitRepository
    {
        Task InsertAsync(NeuronPermit permit);

        Task<NeuronPermit> GetAsync(Guid userNeuronId, Guid neuronId);

        Task<IEnumerable<Guid>> GetNeuronIdsByUserNeuronIds(IEnumerable<Guid> userNeuronIds, IEnumerable<Guid> filterNeuronIds = null);

        Task UpdateAsync(NeuronPermit permit);

        Task Initialize();
    }
}
